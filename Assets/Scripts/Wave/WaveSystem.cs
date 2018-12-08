using System;
using System.Collections;
using System.Collections.Generic;
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
	}
	
	[Serializable]
	public struct Wave : IComponentData
	{
		// what level of stadium
		public int Level;
		// how fast does wave move
		public float Speed;
	}
    
	// just a wrapper for showing inside unity
	public class WavingFanComponent : ComponentDataWrapper<WavingFan> { }
	public class WaveComponent : ComponentDataWrapper<Wave> { }
	
	
	public class WaveSystem : JobComponentSystem
	{

		struct FansGroup
		{
			public readonly int Length;
			public EntityArray Entities;
			
			public ComponentDataArray<WavingFan> Fans;
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
			public ComponentDataArray<Position> WavePositions;
            
			public float deltaTime;
            
			public void Execute(int index)
			{
				var fan = Fans[index];
				var waveAmount = 0f;
				for (int i = 0; i < WavePositions.Length; i++)
				{
					// TODO: CALCULATE MAXIMUM FOR EACH WAVE
					waveAmount = 0.5f;
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
				WavePositions = _wavesGroup.WavePositions,
				// schedule it and say how many entities are there
			}.Schedule(_fansGroup.Length, 32, inputDeps);
			
			return handle;
		}
	}

}

