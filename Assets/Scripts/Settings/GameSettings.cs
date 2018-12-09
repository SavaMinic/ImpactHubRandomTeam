using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameSettings : ScriptableObject
{
    #region Static 
    
    static GameSettings _instance = null;
    public static GameSettings I
    {
        get
        {
            if (!_instance)
                _instance = Resources.Load<GameSettings>("GameSettings");
            return _instance;
        }
    }

    public int MaxLevel;

    #endregion

    public List<float> RadiusPerLevel;
    public float debug1;
    public float debug2;

    public float GetRadiusPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, RadiusPerLevel.Count - 1);
        return RadiusPerLevel[level];
    }

    [Header("Gameplay")]
    public float WavingFanAmountForSuccess;
    public float InteractIgnoreTime;
    // starting from center
    public List<int> InteractibleFansPerLevel;
    
    public float GetInteractibleFansPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, InteractibleFansPerLevel.Count - 1);
        return InteractibleFansPerLevel[level];
    }

    [Header("Meshes")] 
    public Mesh Arms;
    public Mesh Body;
    public Mesh Legs;
    public Material[] Materials;
    
    public float IncreaseForSuccess;
    public float DecreaseForFailure;
    public float DecreaseForMiss;
    public List<float> ScorePerLevel;

    public bool DemoMode;
    public int MaxConsecutiveErrors;
    
    public float GetScorePerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, ScorePerLevel.Count - 1);
        return ScorePerLevel[level];
    }
    
    [Header("Interaction")]
    public float StartTimeToGenerateInteraction;
    public float TimeToGenerateInteraction;
    public List<int> InteractionCountPerLevel;
    public int MaxInteractionAtOnce = 10;
    public List<float> InteractionOffsetPerLevel;
    
    public float GetInteractionCountPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, InteractionCountPerLevel.Count - 1);
        return InteractionCountPerLevel[level];
    }
    
    public float GetInteractionOffsetPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, InteractionOffsetPerLevel.Count - 1);
        return InteractionOffsetPerLevel[level];
    }

    public AnimationCurve OutroWavingFanCurve;
    public float Period = 2f;
    public float outroDebug;
}