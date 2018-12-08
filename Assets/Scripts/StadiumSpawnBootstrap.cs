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

	private Entity transform;

	private float startingHeight;
	
	public void Awake()
	{
		Instance = this; // worst singleton ever but it works
		
		entityManager = World.Active.GetExistingManager<EntityManager>();
		startingHeight = standsInnerRadius;
		transform = entityManager.CreateEntity(typeof(Position), typeof(Rotation));
	}

	private static readonly float3 Up = new float3(0,1,0);
	
    private int currentlevel = 0;
    private int count = 0;

    public void InstantiateEntities(int level)
	{
		var dx = (standsOuterRadius -standsInnerRadius) / NumOfRows;
		var circleRadians = math.radians(360f);
		
		var armsOffset = new float3(0f, 1.2f, 0f);
		var row = 0;
		for (float radius = standsInnerRadius; radius <= standsOuterRadius; radius+= dx)
		{
			float dTheta = circleRadians / (radius * NumberOfObjectsInCircle);
			var column = 0;
			var maxColums = circleRadians / dTheta;
			for (float theta = 0; theta < circleRadians ; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 1, math.cos(theta));
				var pos = radius * direction - new float3(0, startingHeight, 0);
				var rot = quaternion.LookRotation(-new float3(direction.x, 0f, direction.z), Up);
				
				var euler = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w).eulerAngles;
				var isInteractable = column < 5 || column > maxColums - 5;
				
				var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position { Value = pos });
				entityManager.SetComponentData(entity, new Body() { InitPosition = pos });
				entityManager.SetComponentData(entity, new Rotation { Value = rot });
                entityManager.SetComponentData(entity, new WavingFan { Level = currentlevel });
                entityManager.AddComponentData(entity, new SeatingData { Column = column, Row = row });
				// just for fans in this columns, always visible by camera
				if (isInteractable)
				{
					entityManager.AddComponentData(entity, new InteractiveTag());
				}

				var armsEntity = entityManager.Instantiate(fanArmsPrefab);
				entityManager.SetComponentData(armsEntity, new Position {Value = new float3(0, 1.3f / 5f, 0)});
				entityManager.SetComponentData(armsEntity, new Hands() { InitPosition = pos + armsOffset, InitRotationEuler = euler });
				entityManager.SetComponentData(armsEntity, new Rotation {Value = quaternion.identity});
                entityManager.SetComponentData(armsEntity, new WavingFan { Level = currentlevel });
				
				var attachEntity = entityManager.CreateEntity();
				entityManager.AddComponentData(attachEntity, new Attach {Parent = entity, Child = armsEntity});
				
				GameController.I.AddNewFun(entity, level, isInteractable);
                count += 2;
				column++;
			}
			row++;
		} 
		
		standsInnerRadius = standsOuterRadius + 1f;
		standsOuterRadius += 10f;
        ++currentlevel;
        Debug.Log(count);
    }
}