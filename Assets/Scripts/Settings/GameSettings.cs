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
    
    #endregion

    public List<float> RadiusPerLevel;
    public float debug1;
    public float debug2;

    public float GetRadiusPerLevel(int level)
    {
        level = Mathf.Clamp(level, 0, RadiusPerLevel.Count - 1);
        return RadiusPerLevel[level];
    }

}