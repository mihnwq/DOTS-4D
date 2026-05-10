using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CameraData : IComponentData
{

    public Entity targetEntity;

    public float mouseSensitivity;
    public float distanceFromTarget;
    public float minDistance;
    public float maxDistance;
    public float smallestClampDown;
    public float mapSmoothTime;
    public float minClampDown;


    public float rotationX;
    public float rotationY;
    public float3 currentRotation;
    public float3 smoothVelocity;
}
