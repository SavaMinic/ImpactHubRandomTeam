using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class StadiumSpawnBootstrap : MonoBehaviour
{
	public static StadiumSpawnBootstrap Instance;
	
	public EntityManager entityManager;
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

	private float startingHeight;
	
	public void Awake()
	{
		Instance = this; // worst singleton ever but it works
		entityManager = World.Active.GetExistingManager<EntityManager>();
	}

	private void Start()
	{

		startingHeight = standsInnerRadius;
		var spawnEntity = entityManager.CreateEntity(typeof(SpawnEntitiesComponent));
		entityManager.SetComponentData(spawnEntity, new SpawnEntitiesComponent
		{
			Count = NumberOfObjectsInCircle,
			InnerRadius = standsInnerRadius,
			OuterRadius = standsOuterRadius,
			Rows = NumOfRows,
		});
		
//		InstantiateEntities(NumberOfObjectsInCircle, standsInnerRadius, standsOuterRadius, NumOfRows);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			InstantiateEntities();
		}
	}

	private static readonly float3 Up = new float3(0,1,0);
	
	public void InstantiateEntities( int count, float innerRadius, float outerRadius, int rows, Mesh mesh, Material material)
	{
		var dx = (outerRadius - innerRadius) / rows;
		
		for (float radius = innerRadius; radius <= outerRadius; radius+= dx)
		{
			float dTheta = 360f / (radius * count);
			for (float theta = 0; theta < 360; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction - new float3(0, innerRadius, 0);
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position {Value = pos});
				entityManager.SetComponentData(entity, new Rotation {Value = quaternion.LookRotation(-new float3(direction.x, 0, direction.z), Up)});
				entityManager.SetSharedComponentData(entity, new MeshInstanceRenderer {material = material, mesh = mesh});
			}
		}
	}
	
	public void InstantiateEntities()
	{
		var dx = (standsOuterRadius -standsInnerRadius) / NumOfRows;
		
		for (float radius = standsInnerRadius; radius <= standsOuterRadius; radius+= dx)
		{
			float dTheta = 360f / (radius * NumberOfObjectsInCircle);
			for (float theta = 0; theta < 360; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction - new float3(0, startingHeight, 0);
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position {Value = pos});
				entityManager.SetComponentData(entity, new Rotation {Value = quaternion.LookRotation(-new float3(direction.x, 0, direction.z), Up)});
				//entityManager.SetSharedComponentData(entity, new MeshInstanceRenderer {material = , mesh = mesh});
			}
		}
		
		standsInnerRadius = standsOuterRadius + 1f;
		standsOuterRadius += 10f;
	}

	public Entity InstantiateFanPrefab()
	{
		return entityManager.Instantiate(fanPrefab);
	}
}