using UnityEngine;

public static class MeshExtractor
{
    public static Mesh GetMeshFromGameObject(GameObject obj)
    {
        if (obj == null) return null;

     
        MeshFilter meshFilter = obj.GetComponentInChildren<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            return meshFilter.sharedMesh;
        }

       
        SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            return skinnedMeshRenderer.sharedMesh;
        }

    
     
        return null;
    }
}
