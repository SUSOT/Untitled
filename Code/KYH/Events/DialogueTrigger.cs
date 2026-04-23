using Unity.Cinemachine;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO dialogueChannel;

    public string[] dialogue;
    public bool isSpeakToStop;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dialogue.Length < 1) return;

        if(collision.CompareTag("Player"))
        {
            StartAConversation events = DialogueEvents.StartAConversation;
            events.dialogue = this.dialogue;
            events.isStop = isSpeakToStop;

            dialogueChannel.RaiseEvent(events);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
