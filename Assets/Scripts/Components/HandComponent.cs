using System;
using Unity.Mathematics;
using Unity.Entities;

[Serializable]
public struct Hand : IComponentData {

    public float3 InitPosition;
    public float3 InitRotation;

}

[UnityEngine.DisallowMultipleComponent]
public class HandComponent : ComponentDataWrapper<Hand> { }
