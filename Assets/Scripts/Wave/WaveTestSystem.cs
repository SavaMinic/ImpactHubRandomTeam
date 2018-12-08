using System;
using System.Collections;
using System.Collections.Generic;
using RandomName.Wave;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace RandomName.Wave
{
    public class WaveTestSystem : JobComponentSystem
    {
        struct FansGroup
        {
            public readonly int Length;
            public EntityArray Entities;
			
            [ReadOnly] public ComponentDataArray<WavingFan> Fans;
            public ComponentDataArray<Scale> FanScales;
        }
        [Inject] private FansGroup _fansGroup;
        
        [BurstCompile]
        struct WavingTestJob : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<WavingFan> Fans;
            public ComponentDataArray<Scale> FanScales;
            
            public void Execute(int index)
            {
                var amount = Fans[index].Value * 2f + 1f;
                var scale = FanScales[index];
                scale.Value = new float3(amount, amount, amount);
                FanScales[index] = scale;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_fansGroup.Length == 0)
            {
                return inputDeps;
            }
			
            var handle = new WavingTestJob
            {
                Fans = _fansGroup.Fans,
                FanScales = _fansGroup.FanScales,
                // schedule it and say how many entities are there
            }.Schedule(_fansGroup.Length, 32, inputDeps);
			
            return handle;
        }
    }
}