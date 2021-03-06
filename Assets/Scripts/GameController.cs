﻿using System;
using System.Collections;
using System.Collections.Generic;
using RandomName.Interactive;
using RandomName.UI;
using RandomName.Wave;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

	#region Static

	public static GameController I { get; private set; }

	#endregion
    
	#region Fields

	public AudioClip[] yeah;
	
	public AudioSource aSource;
	public AudioSource yeahAS;
	
	private EntityManager manager;

	private List<Entity> interactableFans = new List<Entity>();
	private List<Entity> activeInteractibleFans = new List<Entity>();

	private float timeToGenerateInteraction;
	
	private List<Entity> entitiesToRemove = new List<Entity>();

	private float currentScore;

	private int consecutiveErrorCount;

	private float enableEndGameTime = 2f;

	public GameObject stadium1;
	public GameObject stadium2;

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
			if (GameSettings.I.DemoMode)
			{
				consecutiveErrorCount = 0;
			}
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
		
		EnableStartingSystems();
	}

	private void Start()
	{
		timeToGenerateInteraction = GameSettings.I.StartTimeToGenerateInteraction;
		manager = World.Active.GetOrCreateManager<EntityManager>();
		StadiumSpawnBootstrap.Instance.InstantiateEntities(0);
		CameraController.I.OverviewCam(MaxLevel);

		IsRunning = true;
		CurrentScore = 0;
		
		stadium1.SetActive(!GameSettings.I.DemoMode);
		stadium2.SetActive(GameSettings.I.DemoMode);
	}
	
	private void OnDestroy()
	{
		if (World.Active == null) return;
		var entityManager = World.Active.GetOrCreateManager<EntityManager>();
		var entityArray = entityManager.GetAllEntities();
		foreach (var e in entityArray)
			entityManager.DestroyEntity(e);
		entityArray.Dispose();
	}

	private void Update()
	{
		if (!Application.isPlaying)
			return;

		if (!IsRunning)
		{
			enableEndGameTime -= Time.deltaTime / Time.timeScale;
			if (enableEndGameTime > 0)
				return;
			
			if (Input.anyKeyDown)
			{
				SceneManager.LoadScene("MainMenu");
			}
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SkipLevel();
		}

		// no need for interactions in demo mode
		if (GameSettings.I.DemoMode)
			return;
		
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

	public void SkipLevel()
	{
		if (MaxLevel == GameSettings.I.MaxLevel)
			return;
		ProgressToNextLevel();
		CurrentScore = 0f;
	}

	public void EndGame(bool isWon)
	{
		IsRunning = false;
		MainCanvas.I.EndGame(isWon);
		CameraController.I.EndGame(isWon);
		DisableAllSystems();
	}

	private void DisableAllSystems()
	{
		World.Active.GetExistingManager<WaveMovementSystem>().Enabled = false;
		World.Active.GetExistingManager<WaveSystem>().Enabled = false;
		World.Active.GetExistingManager<WaveReachingTopSystem>().Enabled = false;
		World.Active.GetExistingManager<GenerateInteractiveSystem>().Enabled = false;
		
		World.Active.GetExistingManager<GameOutroSystem>().Enabled = true;
	}
	
	private void EnableStartingSystems()
	{
		World.Active.GetExistingManager<WaveMovementSystem>().Enabled = true;
		World.Active.GetExistingManager<WaveSystem>().Enabled = true;
		World.Active.GetExistingManager<WaveReachingTopSystem>().Enabled = true;
		World.Active.GetExistingManager<GenerateInteractiveSystem>().Enabled = true;
		
		World.Active.GetExistingManager<GameOutroSystem>().Enabled = false;
	}

	public void SetDanceMode(bool danceOff)
	{
		if (danceOff) DisableAllSystems();
		else EnableStartingSystems();
	}

	public void ProgressToNextLevel()
	{
		if (MaxLevel >= GameSettings.I.MaxLevel)
		{
			EndGame(true);
			return;
		}

		aSource.volume += 0.05f;
		
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
			
			
			yeahAS.PlayOneShot(yeah[Random.Range(0, yeah.Length)]);
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
