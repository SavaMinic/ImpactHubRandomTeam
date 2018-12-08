using System;
using Unity.Entities;

[Serializable]
public struct InteractiveTag : IComponentData
{
    
}

[UnityEngine.DisallowMultipleComponent]
public class InteractiveTagComponent : ComponentDataWrapper<InteractiveTag> { }