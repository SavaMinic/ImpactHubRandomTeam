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

    public Transform camTarget;

    public float verticalDiffPerLevel = 5f;
    public float horizontalDiffPerLevel = 8f;

    #endregion
    
    #region Mono

    private void Awake()
    {
        I = this;
        SetActiveCamera(overviewCams[0]);
    }

    #endregion
    
    #region Public

    public void FocusOnFan(float3 pos, int level)
    {
        SetActiveCamera(fanCam);
        camTarget.position = pos;
        fanCam.transform.position = Vector3.up * (pos.y + verticalDiffPerLevel * (level + 1));
        // go back a little
        fanCam.transform.position -= new Vector3(pos.x, 0f, pos.z).normalized * (level * horizontalDiffPerLevel);
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
