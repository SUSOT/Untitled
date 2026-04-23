using UnityEngine;

public class ParallaxUITrigger : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO textChannel;

    public bool isFadeIn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - transform.position).normalized;
            ParallaxMoveEvent events = UIEvent.ParallaxMoveEvent;
            events.moveDirection = exitDirection;
            events.isFadeIn = isFadeIn;
            textChannel.RaiseEvent(events);
        }
    }

    public void UIFadeOut()
    {
        ParallaxMoveEvent events = UIEvent.ParallaxMoveEvent;
        events.isFadeIn = false;
        textChannel.RaiseEvent(events);
    }

    public void UIFadeIn()
    {
        ParallaxMoveEvent events = UIEvent.ParallaxMoveEvent;
        events.isFadeIn = true;
        textChannel.RaiseEvent(events);
    }
}
