using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using RandomName.Wave;
using UnityEngine;

public class GameOutroSystem : JobComponentSystem
{
	private struct FanGroup
	{
		[ReadOnly] public EntityArray Entities;
		[ReadOnly] public SharedComponentDataArray<FanComponent> Fans;
		public ComponentDataArray<WavingFan> WavingFans;
	}

	[Inject] private FanGroup fanGroup;

	private float period;
	private float progressTime;
	//private float periodTime

	protected override void OnCreateManager()
	{
		base.OnCreateManager();
		
		period = GameSettings.I.Period;

	}

	[BurstCompile]
	private struct OutroWavingFanSetter : IJobParallelFor
	{
		public float maxValue;
		public float currentValue;
		
		[ReadOnly] public EntityArray Entities;
		[ReadOnly] public SharedComponentDataArray<FanComponent> Fans;
		public ComponentDataArray<WavingFan> WavingFan;
		
		public void Execute(int index)
		{
			var fan = WavingFan[index];

			if ((Fans[index].Value & 1) == 0)
			{
				fan.Value = currentValue;
			}
			else
			{
				fan.Value = maxValue - currentValue;
			}
			fan.Value /= 600;

			WavingFan[index] = fan;
		}
	}
	
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		
		var job = new OutroWavingFanSetter()
		{
			WavingFan = fanGroup.WavingFans,
			maxValue = GameSettings.I.outroDebug, 
			Entities = fanGroup.Entities,
			currentValue = GameSettings.I.OutroWavingFanCurve.Evaluate(progressTime / period)
		};

		progressTime = (progressTime + Time.deltaTime) % period;

		return job.Schedule(fanGroup.Entities.Length, 64, inputDeps);
	}
}
