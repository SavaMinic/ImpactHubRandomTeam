using System;
using Unity.Entities;

[Serializable]
public struct InteractiveTag : IComponentData
{
    public int LookingForAttention;
}

[UnityEngine.DisallowMultipleComponent]
public class InteractiveTagComponent : ComponentDataWrapper<InteractiveTag> { }