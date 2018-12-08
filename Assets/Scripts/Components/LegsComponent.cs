using System;
using Unity.Mathematics;
using Unity.Entities;

[Serializable]
public struct Legs : IComponentData {

    public float3 InitPosition;
    public quaternion InitRotation;

}

[UnityEngine.DisallowMultipleComponent]
public class LegsComponent : ComponentDataWrapper<Legs> { }
