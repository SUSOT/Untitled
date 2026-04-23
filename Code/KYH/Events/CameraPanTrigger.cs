using UnityEngine;
using System.Collections;

public class CameraPanTrigger : MonoBehaviour
{
    public PanDirection panDirection;
    public float panDistance = 3f;
    public float panTime = 0.35f;

    [Header("사라지는 시간,0초면 벗어나면 사라짐")]
    public int time;

    [SerializeField] private GameEventChannelSO cameraChannel;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SendPanEvent(false);
            if(time != 0)
            {
                StartCoroutine(DestroyRoutine());
            }    
        }
    }
    public IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(time);
        SendPanEvent(true);
        gameObject.GetComponent<CameraPanTrigger>().enabled = false;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SendPanEvent(true);
    }

    private void SendPanEvent(bool isRewind)
    {
        PanEvent evt = CameraEvents.PanEvent;
        evt.panTime = panTime;
        evt.distance = panDistance;
        evt.direction = panDirection;
        evt.isRewindToStart = isRewind;

        cameraChannel.RaiseEvent(evt);
    }
}
