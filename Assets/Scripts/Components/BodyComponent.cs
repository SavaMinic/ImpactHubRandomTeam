using System;
using Unity.Mathematics;
using Unity.Entities;

[Serializable]
public struct Body : IComponentData {

    public float3 InitPosition;

}

[UnityEngine.DisallowMultipleComponent]
public class BodyComponent : ComponentDataWrapper<Body> { }
