using UnityEngine;
using SFB;
using Dummiesman;
using System.IO;
using System;

public class PC_Selection : MonoBehaviour
{
    public void OpenFileBrowser()
    {
        Debug.Log("[PC_Selection] OpenFileBrowser called.");

        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Select 3D Model",
            "",
            new[] { new ExtensionFilter("3D Models", "obj", "fbx", "prefab") },
            false
        );

        if (paths == null)
        {
            Debug.LogError("[PC_Selection] File browser returned null.");
            return;
        }

        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log("[PC_Selection] No file selected.");
            return;
        }

        string selectedPath = paths[0];
        string ext = Path.GetExtension(selectedPath).ToLowerInvariant();

        Debug.Log("[PC_Selection] Selected file: " + selectedPath);
        Debug.Log("[PC_Selection] Extension: " + ext);

        if (!File.Exists(selectedPath))
        {
            Debug.LogError("[PC_Selection] Selected file does not exist: " + selectedPath);
            return;
        }

        GameObject loadedRoot = null;

        if (ext == ".obj")
        {
            try
            {
                Debug.Log("[PC_Selection] Starting OBJ load...");
                loadedRoot = new OBJLoader().Load(selectedPath);
                Debug.Log("[PC_Selection] OBJ load finished.");
            }
            catch (Exception e)
            {
                Debug.LogError("[PC_Selection] OBJ load failed: " + e);
                return;
            }
        }
        else
        {
            Debug.LogError("[PC_Selection] Selected file type is not supported by OBJLoader at runtime: " + ext);
            Debug.LogError("[PC_Selection] Only .obj is supported by this loader.");
            return;
        }

        if (loadedRoot == null)
        {
            Debug.LogError("[PC_Selection] Loaded object is null.");
            return;
        }

        Debug.Log("[PC_Selection] Loaded root object: " + loadedRoot.name);

        MeshFilter meshFilter = loadedRoot.GetComponentInChildren<MeshFilter>(true);

        if (meshFilter == null)
        {
            Debug.LogError("[PC_Selection] No MeshFilter found in loaded object.");
            return;
        }

        if (meshFilter.sharedMesh == null)
        {
            Debug.LogError("[PC_Selection] MeshFilter found, but sharedMesh is null.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Debug.Log("[PC_Selection] Mesh found: " + mesh.name + " | Vertices: " + mesh.vertexCount);

        if (loadedRoot.transform.localToWorldMatrix.determinant < 0)
        {
            Debug.Log("[PC_Selection] Negative determinant detected. Fixing triangle winding...");

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] tris = mesh.GetTriangles(i);

                for (int t = 0; t < tris.Length; t += 3)
                {
                    int temp = tris[t];
                    tris[t] = tris[t + 1];
                    tris[t + 1] = temp;
                }

                mesh.SetTriangles(tris, i);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Vector3 s = loadedRoot.transform.localScale;
            loadedRoot.transform.localScale = new Vector3(
                Mathf.Abs(s.x),
                Mathf.Abs(s.y),
                Mathf.Abs(s.z)
            );

            Debug.Log("[PC_Selection] Triangle winding fixed and scale normalized.");
        }

        MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = meshFilter.GetComponentInChildren<MeshRenderer>(true);
        }

        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            Debug.Log("[PC_Selection] URP/Lit shader: " + (shader != null ? "FOUND" : "NULL"));

            if (shader == null)
            {
                shader = Shader.Find("Standard");
                Debug.Log("[PC_Selection] Standard shader: " + (shader != null ? "FOUND" : "NULL"));
            }

            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.color = Color.gray;
                renderer.material = mat;
                Debug.Log("[PC_Selection] Material assigned successfully.");
            }
            else
            {
                Debug.LogError("[PC_Selection] No suitable shader found in build.");
            }
        }
        else
        {
            Debug.LogError("[PC_Selection] No MeshRenderer found.");
        }

        Debug.Log("[PC_Selection] Model loaded successfully.");

        DOTSMeshSwapper.instance.testMesh = MeshExtractor.GetMeshFromGameObject(meshFilter.gameObject);
    }
}