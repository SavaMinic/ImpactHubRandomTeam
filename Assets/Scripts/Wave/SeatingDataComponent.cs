using System;
using Unity.Entities;

namespace RandomName.Wave
{

    [Serializable]
    public struct SeatingData : IComponentData
    {
        public int Row;
        public int Column;
    }
    
    // just a wrapper for showing inside unity
    public class SeatingDataComponent : ComponentDataWrapper<SeatingData> { }
}