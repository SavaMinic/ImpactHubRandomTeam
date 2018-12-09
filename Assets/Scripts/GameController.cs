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

	private List<Entity> interactableFans = new List<Entity>();
	private List<Entity> activeInteractibleFans = new List<Entity>();

	private float timeToGenerateInteraction;
	
	private List<Entity> entitiesToRemove = new List<Entity>();

	private float currentScore;

	private int consecutiveErrorCount;

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
				currentScore = 0;
				MainCanvas.I.RefreshProgressBarToEnd();
			}
			else
			{
				var progress = currentScore / ScoreForNextLevel;
				MainCanvas.I.RefreshProgressBar(progress);
			}
		}
	}

	private int ConsecutiveErrorCount
	{
		get { return consecutiveErrorCount; }
		set
		{
			consecutiveErrorCount = value;
			MainCanvas.I.RefreshConsecutiveErrorCount(consecutiveErrorCount);
			if (consecutiveErrorCount == GameSettings.I.MaxConsecutiveErrors)
			{
				EndGame(false);
			}
		}
	}
	
	public bool IsRunning { get; private set; }
	
	private float ScoreForNextLevel => GameSettings.I.GetScorePerLevel(MaxLevel);

	#endregion
    
	#region Mono

	private void Awake()
	{
		I = this;
	}

	private void Start()
	{
		timeToGenerateInteraction = GameSettings.I.StartTimeToGenerateInteraction;
		manager = World.Active.GetOrCreateManager<EntityManager>();
		StadiumSpawnBootstrap.Instance.InstantiateEntities(0);
		CameraController.I.OverviewCam(MaxLevel);

		IsRunning = true;
		CurrentScore = 0;
	}

	private void Update()
	{
		if (!Application.isPlaying)
			return;

		if (!IsRunning)
			return;
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
            ProgressToNextLevel();
			CurrentScore = 0f;
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
				ConsecutiveErrorCount++;
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

	public void EndGame(bool isWon)
	{
		IsRunning = false;
		MainCanvas.I.EndGame(isWon);
		CameraController.I.EndGame(isWon);
	}

	public void ProgressToNextLevel()
	{
		if (MaxLevel >= 7)
		{
			EndGame(true);
			return;
		}
		
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
			ConsecutiveErrorCount = 0;
		}
		else
		{
			// FAIL
			CurrentScore += GameSettings.I.DecreaseForMiss;
			ConsecutiveErrorCount++;
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
		if (isInteractable)
		{
			interactableFans.Add(fan);
		}
	}

	#endregion
}
