using Unity.Cinemachine;
using UnityEngine;

public class CameraSwapTrigger : MonoBehaviour
{
    public CinemachineCamera leftCamera;
    public CinemachineCamera rightCamera;
    public bool isCameraFollowPlayer;
    public bool isForce;

    [SerializeField] private GameEventChannelSO cameraChannel;
    [SerializeField]private bool isOneTime;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (leftCamera is null || rightCamera is null) return;

        if (other.CompareTag("Player"))
        {
            Vector2 exitDirection = (other.transform.position - transform.position).normalized;
            SwapCameraEvent swapEvt = CameraEvents.SwapCameraEvent;
            swapEvt.isForceSwap = isForce;
            swapEvt.leftCamera = leftCamera;
            swapEvt.rightCamera = rightCamera;
            swapEvt.moveDirection = exitDirection;
            swapEvt.isBattonFollow = isCameraFollowPlayer;

            cameraChannel.RaiseEvent(swapEvt);

            if (isOneTime)
                gameObject.SetActive(false);
        }
    }
}
