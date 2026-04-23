using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
{
    public float intensity;
    public float Second;

    [SerializeField]
    private GameEventChannelSO cameraEventSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Second == default) return;

        if (collision.CompareTag("Player"))
        {
            PerlinShake perlinShake = CameraEvents.CameraShakeEvent;
            perlinShake.second = Second;
            perlinShake.intensity = intensity;

            cameraEventSO.RaiseEvent(perlinShake);
        }
    }
}
