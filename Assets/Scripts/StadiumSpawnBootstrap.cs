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
		startingHeight = standsInnerRadius;
	}

	private void Start()
	{
		InstantiateEntities();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			InstantiateEntities();
		}
	}

	private static readonly float3 Up = new float3(0,1,0);
	
	public void InstantiateEntities()
	{
		var dx = (standsOuterRadius -standsInnerRadius) / NumOfRows;
		var circleRadians = math.radians(360f);
		
		for (float radius = standsInnerRadius; radius <= standsOuterRadius; radius+= dx)
		{
			float dTheta = circleRadians / (radius * NumberOfObjectsInCircle);
			for (float theta = 0; theta < circleRadians ; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction - new float3(0, startingHeight, 0);
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position {Value = pos});
				entityManager.SetComponentData(entity, new Rotation {Value = quaternion.LookRotation(-new float3(direction.x, 0, direction.z), Up)});
//				entityManager.SetSharedComponentData(entity, new MeshInstanceRenderer {material = material, mesh = mesh});
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