using System;
using Unity.Entities;

[Serializable]
public struct Hand : IComponentData {

}

[UnityEngine.DisallowMultipleComponent]
public class HandComponent : ComponentDataWrapper<Hand> { }
