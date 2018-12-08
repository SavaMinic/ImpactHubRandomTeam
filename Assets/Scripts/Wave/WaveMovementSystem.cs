using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RandomName.Wave
{
    public class WaveMovementSystem : JobComponentSystem
    {
        struct WavesGroup
        {
            public readonly int Length;
            public EntityArray Entities;
			
            public ComponentDataArray<Wave> Waves;
            public ComponentDataArray<Position> WavePositions;
        }
        
        [Inject] private WavesGroup _wavesGroup;
        
        [BurstCompile]
        struct WavingJob : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Wave> Waves;
            public ComponentDataArray<Position> WavePositions;
            
            public float deltaTime;
            
            public void Execute(int index)
            {
                // TODO: move wave
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_wavesGroup.Length == 0)
            {
                return inputDeps;
            }
			
            var handle = new WavingJob
            {
                Waves = _wavesGroup.Waves,
                WavePositions = _wavesGroup.WavePositions,
                deltaTime = Time.deltaTime,
                // schedule it and say how many entities are there
            }.Schedule(_wavesGroup.Length, 32, inputDeps);
			
            return handle;
        }
    }
}