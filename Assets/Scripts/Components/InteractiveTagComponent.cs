using System;
using Unity.Entities;

[Serializable]
public struct InteractiveTag : IComponentData
{
    public float IgnoreTime;
    // there can't be bools in IComponentData???
    public int LookingForAttention;
    public int WaveReachedTop;
    public int WavePassed;
}

[UnityEngine.DisallowMultipleComponent]
public class InteractiveTagComponent : ComponentDataWrapper<InteractiveTag> { }