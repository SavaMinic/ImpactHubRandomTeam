using System;
using System.Collections;
using System.Collections.Generic;
using RandomName.UI;
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
	private List<Entity> activeInteractibleFans = new List<Entity>();

	private float timeToGenerateInteraction;
	
	private List<Entity> entitiesToRemove = new List<Entity>();

	private float currentScore;

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

	private float CurrentScore
	{
		get { return currentScore; }
		set
		{
			currentScore = Mathf.Max(0f, value);
			if (currentScore >= ScoreForNextLevel)
			{
				ProgressToNextLevel();
			}
			var progress = currentScore / ScoreForNextLevel;
			MainCanvas.I.RefreshProgressBar(progress);
		}
	}
	
	private float ScoreForNextLevel => GameSettings.I.GetScorePerLevel(MaxLevel);

	#endregion
    
	#region Mono

	private void Awake()
	{
		I = this;
		fansPerLevel = new Dictionary<int, List<Entity>>();
	}

	private void Start()
	{
		timeToGenerateInteraction = GameSettings.I.StartTimeToGenerateInteraction;
		manager = World.Active.GetOrCreateManager<EntityManager>();
		StadiumSpawnBootstrap.Instance.InstantiateEntities(0);
		CameraController.I.OverviewCam(MaxLevel);

		CurrentScore = 0;
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
            ProgressToNextLevel();
		}
		
		// check activeInteractibleFans if they are ok
		entitiesToRemove.Clear();
		for (int i = 0; i < activeInteractibleFans.Count; i++)
		{
			var entity = activeInteractibleFans[i];
			var haveWavePassed = manager.GetComponentData<InteractiveTag>(entity).WavePassed > 0;
			if (haveWavePassed)
			{
				// FAIL
				manager.SetComponentData(entity, new InteractiveTag { LookingForAttention = 0 });
				entitiesToRemove.Add(entity);
				// clear button for this entity
				MainCanvas.I.ClearButton(entity);
				CurrentScore += GameSettings.I.DecreaseForFailure;
			}
		}
		// remove them from list
		for (int i = 0; i < entitiesToRemove.Count; i++)
		{
			activeInteractibleFans.Remove(entitiesToRemove[i]);
		}
		
		// is it time?
		timeToGenerateInteraction -= Time.deltaTime / Time.timeScale;
		if (timeToGenerateInteraction <= 0 && activeInteractibleFans.Count < GameSettings.I.MaxInteractionAtOnce)
		{
			timeToGenerateInteraction = GameSettings.I.TimeToGenerateInteraction;
			// generate new interactions
			var cnt = GameSettings.I.GetInteractionCountPerLevel(MaxLevel);
			for (int i = 0; i < cnt; i++)
			{
				var randomEntity = interactableFans[Random.Range(0, interactableFans.Count)];
				var isInteractive = manager.GetComponentData<InteractiveTag>(randomEntity).LookingForAttention > 0;
				if (!isInteractive)
				{
					manager.SetComponentData(randomEntity, new InteractiveTag
					{
						LookingForAttention = 1,
						IgnoreTime = GameSettings.I.InteractIgnoreTime
					});
					activeInteractibleFans.Add(randomEntity);
					var pos = manager.GetComponentData<Position>(randomEntity).Value;
					MainCanvas.I.ShowInteractButton(pos, randomEntity, MaxLevel);
				}
				if (activeInteractibleFans.Count >= GameSettings.I.MaxInteractionAtOnce)
				{
					break;
				}
			}
		}
	}

	#endregion
    
	#region Public

	public void ProgressToNextLevel()
	{
		if (MaxLevel >= 7)
			return;
		
		currentScore = 0;
		MaxLevel++;
		StadiumSpawnBootstrap.Instance.InstantiateEntities(MaxLevel);
		CameraController.I.OverviewCam(MaxLevel);
		ClearAllInteractible();
		timeToGenerateInteraction = GameSettings.I.StartTimeToGenerateInteraction;
	}

	public void FanInteracted(Entity entity)
	{
		var fanAmount = manager.GetComponentData<WavingFan>(entity).Value * 100f;
		var isSuccess = fanAmount > GameSettings.I.WavingFanAmountForSuccess;
		MainCanvas.I.ClearButton(entity, isSuccess);
		if (isSuccess)
		{
			// SUCCESS
			CurrentScore += GameSettings.I.IncreaseForSuccess;
		}
		else
		{
			// FAIL
			CurrentScore += GameSettings.I.DecreaseForMiss;
		}
		activeInteractibleFans.Remove(entity);
		manager.SetComponentData(entity, new InteractiveTag { LookingForAttention = 0 });
	}

	public void ClearAllInteractible()
	{
		for (int i = 0; i < activeInteractibleFans.Count; i++)
		{
			manager.SetComponentData(activeInteractibleFans[i], new InteractiveTag { LookingForAttention = 0 });
		}
		activeInteractibleFans.Clear();
		MainCanvas.I.ClearAllInteractible(true);
	}

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
		SelectedFan = fans[Random.Range(0, fans.Count)];

		var position = manager.GetComponentData<Position>(SelectedFan).Value;
		CameraController.I.FocusOnFan(position, MaxLevel);
	}

	#endregion
}
