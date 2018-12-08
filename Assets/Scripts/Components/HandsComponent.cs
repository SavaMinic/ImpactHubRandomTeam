using System;
using Unity.Mathematics;
using Unity.Entities;

[Serializable]
public struct Hands : IComponentData {

    public float3 InitPosition;
    public quaternion InitRotation;

}

[UnityEngine.DisallowMultipleComponent]
public class HandsComponent : ComponentDataWrapper<Hands> { }
