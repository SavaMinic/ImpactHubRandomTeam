using System;
using System.Collections;
using System.Collections.Generic;
using RandomName.Wave;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

	#region Static

	public static GameController I { get; private set; }

	#endregion
    
	#region Fields
	
	private EntityManager manager;

	private Dictionary<int, List<Entity>> fansPerLevel;
	private List<Entity> interactableFans = new List<Entity>();

	#endregion

	#region Properties

	public Entity SelectedFan { get; private set; }

	public Vector3 SelectedFanPosition
	{
		get
		{
			return manager.GetComponentData<Position>(SelectedFan).Value;
		}
	}

	public int MaxLevel { get; private set; }

	#endregion
    
	#region Mono

	private void Awake()
	{
		I = this;
		fansPerLevel = new Dictionary<int, List<Entity>>();
	}

	private void Start()
	{
		manager = World.Active.GetOrCreateManager<EntityManager>();
		StadiumSpawnBootstrap.Instance.InstantiateEntities(0);
		CameraController.I.OverviewCam(MaxLevel);
	}

	private void Update()
	{
		if (!Application.isPlaying)
			return;

		if (Input.GetKeyDown(KeyCode.Q))
		{
			var midLevel = Mathf.FloorToInt(MaxLevel / 2f);
			SelectNewFan(midLevel);
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MaxLevel++;
			StadiumSpawnBootstrap.Instance.InstantiateEntities(MaxLevel);
			CameraController.I.OverviewCam(MaxLevel);
		}
	}

	#endregion
    
	#region Public

	public void AddNewFun(Entity fan, int level, bool isInteractable)
	{
		if (!fansPerLevel.ContainsKey(level))
		{
			fansPerLevel.Add(level, new List<Entity>());
		}
		fansPerLevel[level].Add(fan);

		if (isInteractable)
		{
			interactableFans.Add(fan);
		}
	}

	public void SelectNewFan(int level)
	{
		if (!fansPerLevel.ContainsKey(level) || fansPerLevel[level].Count == 0)
			return;

		var fans = fansPerLevel[level];
		SelectedFan = fans[UnityEngine.Random.Range(0, fans.Count)];

		var position = manager.GetComponentData<Position>(SelectedFan).Value;
		CameraController.I.FocusOnFan(position, MaxLevel);
	}

	#endregion
}
