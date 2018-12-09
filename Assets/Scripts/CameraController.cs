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

    public CinemachineVirtualCamera dollyTrackCam;
    public AnimationCurve dollyTrackCurve;
    public AnimationCurve dollyBigTrackCurve;

    public CinemachineSmoothPath smallPath;
    public CinemachineSmoothPath bigPath;

    #endregion
    
    #region Mono

    private void Awake()
    {
        I = this;
        SetActiveCamera(overviewCams[0]);
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (dollyTrackCam.Priority == 100)
        {
            var duration = GameSettings.I.DemoMode ? 30f : 20f;
            var curve = GameSettings.I.DemoMode ? dollyBigTrackCurve : dollyTrackCurve;
            var path = curve.Evaluate(Time.time % duration);
            dollyTrackCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = path;
        }
        
    }

    #endregion
    
    #region Public

    
    public void EndGame(bool isWon)
    {
        var path = GameSettings.I.DemoMode ? bigPath : smallPath;
        dollyTrackCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_Path = path;
        SetActiveCamera(dollyTrackCam);
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
        dollyTrackCam.Priority = cam == dollyTrackCam ? 100 : 1;
        for (int i = 0; i < overviewCams.Count; i++)
        {
            overviewCams[i].Priority = cam == overviewCams[i] ? 100 : 1;
        }
    }
    
    #endregion
}
