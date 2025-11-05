using Cinemachine;
using UnityEngine;

public class CameraAutoFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        DontDestroyOnLoad(gameObject);
    }

    public void BindToPlayer(GameObject player)
    {
        if (vcam == null || player == null) return;
        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;
        Debug.Log($"CameraAutoFollow: 摄像机已绑定到玩家 {player.name}");
    }
}
