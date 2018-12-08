using System;
using Unity.Entities;

[Serializable]
public struct Body : IComponentData {

}

[UnityEngine.DisallowMultipleComponent]
public class BodyComponent : ComponentDataWrapper<Body> { }
