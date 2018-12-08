using Unity.Collections;
using Unity.Entities;

public class SpawnerSystem : ComponentSystem {
	
#pragma warning disable 649
	struct Group {
		[ReadOnly] public SharedComponentDataArray<Spawner> spawner;
	}
	

	[Inject] Group group;
#pragma warning restore 649

	protected override void OnUpdate() {
//		var spawner = group.spawner[0];
//		
//		StadiumSpawnBootstrap.Instance.InstantiateEntities(spawner.count, spawner.innerRadius, spawner.outerRadius, spawner.rows, spawner.mesh, spawner.material);
		
		Enabled = false;
	}
	
}