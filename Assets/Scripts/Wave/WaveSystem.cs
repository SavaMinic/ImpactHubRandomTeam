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

		struct WavingFans
		{
			public readonly int Length;
			public EntityArray Entities;
			
			public ComponentDataArray<WavingFan> Fans;
		}
		
		[Inject] private WavingFans fans;
		
		[BurstCompile]
		struct WavingJob : IJobParallelFor
		{
			[ReadOnly] public EntityArray Entities;
			public ComponentDataArray<WavingFan> WavingsFans;
			public ComponentDataArray<WavingFan> Waves;
            
			public float deltaTime;
            
			public void Execute(int index)
			{
				// TODO: calculate where is the wave
				// TODO: calculate what is the amount of 
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if (fans.Length == 0)
			{
				return inputDeps;
			}
			
			return base.OnUpdate(inputDeps);
		}
	}

}

