using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class StadiumSpawnBootstrap : MonoBehaviour
{
	public static StadiumSpawnBootstrap Instance;
	
	private EntityManager entityManager;
	private EntityArchetype entityArchetype;

	[SerializeField]
	private GameObject fanPrefab;

	private MeshInstanceRendererComponent[] Meshes;

	[SerializeField]
	private int NumberOfObjectsInCircle;
	[SerializeField]
	private int NumOfRows;
	[SerializeField]
	private float standsInnerRadius;
	[SerializeField]
	private float standsOuterRadius;
	
	public void Awake()
	{
		Instance = this; // worst singleton ever but it works
		entityManager = World.Active.GetExistingManager<EntityManager>();
	}

	private void Start()
	{
		Meshes = GameObject.FindObjectsOfType<MeshInstanceRendererComponent>();
		foreach (var mesh in Meshes)
		{
			mesh.gameObject.SetActive(false);
		}

		var spawnEntity = entityManager.CreateEntity(typeof(SpawnEntitiesComponent));
		entityManager.SetComponentData(spawnEntity, new SpawnEntitiesComponent
		{
			Count = NumberOfObjectsInCircle,
			InnerRadius = standsInnerRadius,
			OuterRadius = standsOuterRadius,
			Rows = NumOfRows,
		});
		
		InstantiateEntities(NumberOfObjectsInCircle, standsInnerRadius, standsOuterRadius, NumOfRows);
	}

	private static readonly float3 Up = new float3(0,1,0);
	
	public void InstantiateEntities( int count, float innerRadius, float outerRadius, int rows)
	{
		var dx = (outerRadius - innerRadius) / rows;
		
		for (float radius = innerRadius; radius <= outerRadius; radius+= dx)
		{
			float dTheta = 360f / (radius * count);
			for (float theta = 0; theta < 360; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction;
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position {Value = pos});
				entityManager.SetComponentData(entity, new Rotation {Value = quaternion.LookRotation(-new float3(direction.x, 0, direction.z), Up)});
				//entityManager.SetSharedComponentData(entity, Meshes[0].Value);			
			}
		}
	}
}