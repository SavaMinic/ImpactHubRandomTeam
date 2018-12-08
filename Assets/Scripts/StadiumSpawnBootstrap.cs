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
	[SerializeField]
	private GameObject fanLegPrefab;
    [SerializeField]
    private GameObject seatPrefab;

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
		startingHeight = 14f;
		transform = entityManager.CreateEntity(typeof(Position), typeof(Rotation));
	}

	private static readonly float3 Up = new float3(0,1,0);
	
    private static int currentLevel = 0;
    private int count = 0;

    public void InstantiateEntities(int level)
	{
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)currentLevel + 1);
        const float spread = 0.5f;

		var dx = (standsOuterRadius - standsInnerRadius) / (NumOfRows + 2);
		var circleRadians = math.radians(360f);
		
		var armOffset = new float3(0f, 1.3f, 0f);
		var legOffset = new float3(0f, 0.55f, 0f);
		var row = 0;
		for (int i = 1; i < NumOfRows -1 ; i++)
		{
			var radius = standsInnerRadius + dx * i;
			float dTheta = circleRadians / (radius * NumberOfObjectsInCircle);
			var column = 0;
			var maxColums = circleRadians / dTheta;
			for (float theta = 0; theta < circleRadians ; theta += dTheta)
			{
				var direction = new float3(math.sin(theta), 0.68f, math.cos(theta));
				var pos = (radius + 0.3f * level) * direction - new float3(0, startingHeight, 0) +
				          random.NextFloat3(new float3(spread, spread, spread), new float3(-spread, -0, -spread));
				var rot = quaternion.LookRotation(-new float3(direction.x, 0f, direction.z), Up);
				
				var euler = new Quaternion(rot.value.x, rot.value.y, rot.value.z, rot.value.w).eulerAngles;
				var isInteractable = column < 5 || column > maxColums - 5;
				
                var seatEntity = entityManager.Instantiate(seatPrefab);
                entityManager.SetComponentData(seatEntity, new Position { Value = pos + new float3(0, 0.3f, -0.0f)});
                entityManager.SetComponentData(seatEntity, new Rotation { Value = rot });

                var entity = entityManager.Instantiate(fanPrefab);
				entityManager.SetComponentData(entity, new Position { Value = pos });
				entityManager.SetComponentData(entity, new Body() { InitPosition = pos });
				entityManager.SetComponentData(entity, new Rotation { Value = rot });
                entityManager.SetComponentData(entity, new WavingFan { Level = currentLevel });
                entityManager.AddComponentData(entity, new SeatingData { Column = column, Row = row });
				// just for fans in this columns, always visible by camera
				if (isInteractable)
				{
					entityManager.AddComponentData(entity, new InteractiveTag());
				}

				var armsEntity = entityManager.Instantiate(fanArmsPrefab);
				entityManager.SetComponentData(armsEntity, new Position {Value = pos+armOffset});
				entityManager.SetComponentData(armsEntity, new Hands() { InitPosition = pos + armOffset, InitRotationEuler = rot });
				entityManager.SetComponentData(armsEntity, new Rotation {Value = rot});
                entityManager.SetComponentData(armsEntity, new WavingFan { Level = currentLevel });
				entityManager.SetComponentData(armsEntity, new Scale {Value = new float3(5, 5, 5)});
				
				var legEntity = entityManager.Instantiate(fanLegPrefab);
				entityManager.SetComponentData(legEntity, new Position {Value = pos + legOffset});
				entityManager.SetComponentData(legEntity, new Legs() { InitPosition = pos + legOffset, InitRotation = rot });
				entityManager.SetComponentData(legEntity, new Rotation {Value = rot});
				entityManager.SetComponentData(legEntity, new WavingFan { Level = currentLevel });
				entityManager.SetComponentData(legEntity, new Scale {Value = new float3(5, 5, 5)});
//				var attachEntity = entityManager.CreateEntity();
//				entityManager.AddComponentData(attachEntity, new Attach {Parent = entity, Child = armsEntity});
				
				GameController.I.AddNewFun(entity, level, isInteractable);
                count += 2;
				column++;
			}
			row++;
		} 
		
		standsInnerRadius = standsOuterRadius;
		standsOuterRadius += 10f;
        ++currentLevel;
        Debug.Log(count);
    }
}