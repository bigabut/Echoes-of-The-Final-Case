using Cinemachine;
using UnityEngine;

public class RoomCameraSwitcher : MonoBehaviour
{
    private static RoomCameraSwitcher instance;

    [SerializeField] private CinemachineVirtualCamera[] roomCameras;
    [SerializeField] private CinemachineVirtualCamera startCamera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (startCamera == null)
        {
            Debug.LogWarning("[RoomCameraSwitcher] Start Camera belum diisi!");
            return;
        }

        Activate(startCamera);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void SwitchTo(CinemachineVirtualCamera target, Transform followTarget, Vector3 positionDelta)
    {
        if (instance == null)
        {
            Debug.LogWarning("[RoomCameraSwitcher] Tidak ada RoomCameraSwitcher di scene!");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[RoomCameraSwitcher] Room Camera di pintu tujuan masih kosong!");
            return;
        }

        instance.Activate(target);
        target.OnTargetObjectWarped(followTarget, positionDelta);
    }

    private void Activate(CinemachineVirtualCamera target)
    {
        if (System.Array.IndexOf(roomCameras, target) < 0)
        {
            Debug.LogWarning("[RoomCameraSwitcher] " + target.name + " belum masuk daftar Room Cameras!");
        }

        foreach (CinemachineVirtualCamera cam in roomCameras)
        {
            if (cam != null)
            {
                cam.Priority = 0;
            }
        }

        target.Priority = 10;

        Debug.Log("[RoomCameraSwitcher] Kamera aktif: " + target.name);
    }

    public void OnCameraCut(CinemachineBrain brain)
    {
        Debug.Log("[Brain] Cut ke: " + brain.ActiveVirtualCamera.Name);
    }
}