using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using RandomName.Wave;

public class StadiumSpawnBootstrap : MonoBehaviour
{
	public static StadiumSpawnBootstrap Instance;
	
	public EntityManager entityManager;
	private EntityArchetype entityArchetype;

	[SerializeField]
	private GameObject fanPrefab;
	[SerializeField]
	private GameObject fanArmsPrefab;


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
	
    private int currentlevel = 0;
    private int count = 0;

    public void InstantiateEntities()
	{
		var dx = (standsOuterRadius -standsInnerRadius) / NumOfRows;
		var circleRadians = math.radians(360f);
		
		var armsOffset = new float3(0f, 1.2f, 0f);
		for (float radius = standsInnerRadius; radius <= standsOuterRadius; radius+= dx)
		{
			float dTheta = circleRadians / (radius * NumberOfObjectsInCircle);
			for (float theta = 0; theta < circleRadians ; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction - new float3(0, startingHeight, 0);
				var rot = quaternion.LookRotation(-new float3(direction.x, 0f, direction.z), Up);
				
				var euler = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w).eulerAngles;
				
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position { Value = pos });
				entityManager.SetComponentData(entity, new Body() { InitPosition = pos });
				entityManager.SetComponentData(entity, new Rotation { Value = rot });
                entityManager.SetComponentData(entity, new WavingFan { Level = currentlevel });

				var armsEntity = entityManager.Instantiate(fanArmsPrefab);
				entityManager.SetComponentData(armsEntity, new Position { Value = pos + armsOffset });
				entityManager.SetComponentData(armsEntity, new Hands() { InitPosition = pos + armsOffset, InitRotationEuler = euler });
				entityManager.SetComponentData(armsEntity, new Rotation { Value = rot });
                entityManager.SetComponentData(armsEntity, new WavingFan { Level = currentlevel });

                count += 2;
            }
		}
		
		standsInnerRadius = standsOuterRadius + 1f;
		standsOuterRadius += 10f;
        ++currentlevel;
        Debug.Log(count);
    }
}