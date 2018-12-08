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

    public Transform camTarget;

    public float verticalDiffPerLevel = 5f;
    public float horizontalDiffPerLevel = 8f;

    #endregion
    
    #region Mono

    private void Awake()
    {
        I = this;
    }

    #endregion
    
    #region Public

    public void FocusOnFan(float3 pos, int level)
    {
        camTarget.position = pos;
        fanCam.transform.position = Vector3.up * (pos.y + verticalDiffPerLevel * (level + 1));
        // go back a little
        fanCam.transform.position -= new Vector3(pos.x, 0f, pos.z).normalized * (level * horizontalDiffPerLevel);
    }
    
    #endregion
}
