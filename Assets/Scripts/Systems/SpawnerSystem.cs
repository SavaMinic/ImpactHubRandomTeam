using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpawnerSystem : JobComponentSystem {
	
//	private struct SpawnCommandGroup
//	{
//		public ComponentDataArray<SpawnEntitiesComponent> SpawnEntities;
//		public EntityArray Entities;
//	}
//
//	[Inject] private SpawnCommandGroup spawnGroup;
//	
//	protected override JobHandle OnUpdate(JobHandle inputDeps)
//	{
//		var length = spawnGroup.SpawnEntities.Length;
//		for (int i = 0; i < length; ++i)
//		{
//			var spawnCommand = spawnGroup.SpawnEntities[i];
//			StadiumSpawnBootstrap.Instance.InstantiateEntities(spawnCommand.Count, spawnCommand.InnerRadius, spawnCommand.OuterRadius, spawnCommand.Rows);
//			EntityManager.RemoveComponent(spawnGroup.Entities[i], typeof(SpawnEntitiesComponent));
//		}
//		return base.OnUpdate(inputDeps);
//	}
//	
	
}
