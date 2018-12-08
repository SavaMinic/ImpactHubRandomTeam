using System;
using System.Collections;
using System.Collections.Generic;
using RandomName.Wave;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RandomName.Wave
{
	
	[Serializable]
	public struct WavingFan : IComponentData
	{
		public float Value;
        public int Level;

	}
	
	[Serializable]
	public struct Wave : IComponentData
	{
		public float Angle;
		public float3 Center;
		public float Radius;
		
		// what level of stadium
		public int Level;
		// how fast does wave move
		public float Speed;
	}
	
	
	public class WaveSystem : JobComponentSystem
	{

		struct FansGroup
		{
			public readonly int Length;
			public EntityArray Entities;
			
			public ComponentDataArray<WavingFan> Fans;
			public ComponentDataArray<Position> FansPositions;
		}

		struct WavesGroup
		{
			public readonly int Length;
			public EntityArray Entities;
			
			public ComponentDataArray<Wave> Waves;
			public ComponentDataArray<Position> WavePositions;
		}
		
		[Inject] private FansGroup _fansGroup;
		[Inject] private WavesGroup _wavesGroup;
		
		[BurstCompile]
		struct WavingJob : IJobParallelFor
		{
			public ComponentDataArray<WavingFan> Fans;
            [ReadOnly] public ComponentDataArray<Wave> Waves;
            [ReadOnly] public ComponentDataArray<Position> WavePositions;
			[ReadOnly] public ComponentDataArray<Position> FansPositions;

			private float CalculateWaveToFanWavingAmount(float3 wavePosition, float3 fanPosition, int level)
			{
                float2 origin = math.distance(fanPosition.xz, float2.zero) * math.normalize(wavePosition.xz);
                var distance = math.distance(origin, fanPosition.xz);
                return math.unlerp(10f, 0f, distance);
                //return (math.dot(math.normalize(wavePosition.xz), math.normalize(fanPosition.xz)) + 1) / 2;
            }
            
			public void Execute(int index)
			{
				var fan = Fans[index];
				var fanPosition = FansPositions[index].Value;
				var waveAmount = 0f;
				// find maximum wave influence on fan
				for (int i = 0; i < WavePositions.Length; i++)
				{
                    var wave = Waves[i];
                    if (fan.Level == wave.Level) {
					    var amount = CalculateWaveToFanWavingAmount(WavePositions[i].Value, fanPosition, fan.Level);
					    if (amount > waveAmount)
					    {
						    waveAmount = amount;
					    }
                    }
                }
				fan.Value = waveAmount;
				Fans[index] = fan;
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if (_fansGroup.Length == 0)
			{
				return inputDeps;
			}
			
			var handle = new WavingJob
			{
				Fans = _fansGroup.Fans,
				FansPositions = _fansGroup.FansPositions,
				WavePositions = _wavesGroup.WavePositions,
                Waves = _wavesGroup.Waves,
				// schedule it and say how many entities are there
			}.Schedule(_fansGroup.Length, 32, inputDeps);
			
			return handle;
		}
	}

}

