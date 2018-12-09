using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Static

    public static CameraController I { get; private set; }

    #endregion
    
    #region Fields

    public CinemachineVirtualCamera fanCam;
    public List<CinemachineVirtualCamera> overviewCams;

    #endregion
    
    #region Mono

    private void Awake()
    {
        I = this;
        SetActiveCamera(overviewCams[0]);
    }

    #endregion
    
    #region Public

    
    public void EndGame(bool isWon)
    {
        SetActiveCamera(fanCam);
    }

    public void OverviewCam(int level)
    {
        level = Mathf.Clamp(level, 0, overviewCams.Count - 1);
        SetActiveCamera(overviewCams[level]);
    }
    
    #endregion
    
    #region Private

    private void SetActiveCamera(CinemachineVirtualCamera cam)
    {
        fanCam.Priority = cam == fanCam ? 100 : 1;
        for (int i = 0; i < overviewCams.Count; i++)
        {
            overviewCams[i].Priority = cam == overviewCams[i] ? 100 : 1;
        }
    }
    
    #endregion
}
