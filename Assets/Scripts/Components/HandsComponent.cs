using System;
using Unity.Mathematics;
using Unity.Entities;

[Serializable]
public struct Hands : IComponentData {

    public float3 InitPosition;
    public float3 InitRotationEuler;

}

[UnityEngine.DisallowMultipleComponent]
public class HandsComponent : ComponentDataWrapper<Hands> { }
