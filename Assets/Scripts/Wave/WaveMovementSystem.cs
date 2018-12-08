using System;
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
            public ComponentDataArray<Wave> Waves;
            public ComponentDataArray<Position> WavePositions;
            
            public float DeltaTime;
            
            public void Execute(int index)
            {
                var wave = Waves[index];
                var wavePos = WavePositions[index];
                var radius = wave.Radius;
                
                var center = wave.Center;
                wave.Angle = wave.Angle + DeltaTime * wave.Speed;
                Waves[index] = wave;
                float x = center.x + math.cos(wave.Angle) * radius;
                float y = wavePos.Value.y;
                float z = center.y + math.sin(wave.Angle) * radius;
                
                wavePos.Value = new float3(x,y,z);
                WavePositions[index] = wavePos;
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
                DeltaTime = Time.deltaTime,
                // schedule it and say how many entities are there
            }.Schedule(_wavesGroup.Length, 32, inputDeps);
			
            return handle;
        }
    }
}