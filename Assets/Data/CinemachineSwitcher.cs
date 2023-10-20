using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECameraType
{
    PlayerCamera = 0,
    StrategyCamera = 1,
    TargetingCamera = 2,
}

[System.Serializable]
public class CameraEntry
{
    public ECameraType CameraType;
    public CinemachineVirtualCamera CinemachineCamera;
    public string CameraAnimationName;
}

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator mAnimator;
    public static CinemachineSwitcher Instance;
    [SerializeField] private List<CameraEntry> CameraEntries;
    private CameraEntry CurrentCameraEntry;
    [SerializeField] private CinemachineFreeLook FreeLookCam;
    [SerializeField] private CinemachineInputProvider InputProvider;
    [SerializeField] private float InitialXAxisMaxSpeed = 200;
    [SerializeField] private float InitialYAxisMaxSpeed = 2;


    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        Instance = this;
    }
    public void OnPlayerAdded(GameObject player)
    {
        SetFreeLookCamTarget(player);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SetFreeLookCamTarget(GameObject target)
    {
        FreeLookCam.m_LookAt = target.transform;
        FreeLookCam.m_Follow = target.transform;
    }

    public void SetCameraTarget(ECameraType camera_type, Transform target)
    {
        GetCameraEntry(camera_type).CinemachineCamera.LookAt = target;
        GetCameraEntry(camera_type).CinemachineCamera.Follow = target;
    }

    public void HandleStrategyModeActivated()
    {
        FreeLookCam.gameObject.SetActive(false);
    }

    public void ChangeCamera(ECameraType camera_type)
    {
        CameraEntry camera_entry = GetCameraEntry(camera_type);
        if (camera_entry == null)
        {
            return;
        }

        CurrentCameraEntry = GetCameraEntry(camera_type);
        //mAnimator.Play(camera_entry.CameraAnimationName);
    }

    public void EnableCameraControl(bool is_enabled)
    {
        InputProvider.enabled = is_enabled;
    }

    private void HandleMouseSensitivityChange(float sensitivity)
    {
        SetXSpeed(sensitivity * InitialXAxisMaxSpeed);
        SetYSpeed(sensitivity * InitialYAxisMaxSpeed);
    }

    public void SetXSpeed(float value)
    {
        FreeLookCam.m_XAxis.m_MaxSpeed = value;
    }

    public void SetYSpeed(float value)
    {
        FreeLookCam.m_YAxis.m_MaxSpeed = value;
    }
    private void HandleFpsMovdeDeactivated()
    {
        InputProvider.enabled = false;
    }

    private CameraEntry GetCameraEntry(ECameraType camera_type)
    {
        for (int i = 0; i < CameraEntries.Count; i++)
        {
            if (CameraEntries[i].CameraType == camera_type)
            {
                return CameraEntries[i];
            }
        }
        return null;
    }
}
