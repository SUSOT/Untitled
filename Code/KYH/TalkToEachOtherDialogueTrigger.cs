using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class TalkToEachOtherDialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO dialogueChannel;

    public string[] dialogue;
    public NpcDialogueComponent talker;
    public bool IsStartTalkerPlayer;

    public float distance;
    public float panTime;
    public PanDirection direction;

    [SerializeField] private PlayableDirector[] director;

    public CinemachineCamera leftCamera;
    public CinemachineCamera rightCamera;

    public string songName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dialogue.Length < 1) return;

        if (collision.CompareTag("Player"))
        {
            StartATalkEachOther events = DialogueEvents.StartATalkEachOther;
            events.talker = talker;
            events.dialogue = dialogue;
            events.startTalkerIsPlayer = IsStartTalkerPlayer;
            events.panTime = panTime;
            events.npcDistance = distance;
            events.npcDirection = direction;
            events.director = director;
            events.leftCamera = leftCamera;
            events.rightCamera = rightCamera;
            events.changeSongName = songName;

            dialogueChannel.RaiseEvent(events);
        }
    }
    public void OnTalkEachOther()
    {
        StartATalkEachOther events = DialogueEvents.StartATalkEachOther;
        events.talker = talker;
        events.dialogue = dialogue;
        events.startTalkerIsPlayer = IsStartTalkerPlayer;
        events.panTime = panTime;
        events.npcDistance = distance;
        events.npcDirection = direction;
        events.director = director;
        events.leftCamera = leftCamera;
        events.rightCamera = rightCamera;

        dialogueChannel.RaiseEvent(events);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
