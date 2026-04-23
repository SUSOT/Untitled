using UnityEngine;

public class TextEventTrigger : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO textChannel;

    public string errorText;
    public TextType textType;
    public KeyCode keySkipType;
    public bool isDefunct;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextEvent events = UIEvent.ErrorTextEvect;
            events.Text = errorText;
            events.textType = textType;
            events.isDefunct = isDefunct;
            events.TextSkipKey = keySkipType;

            textChannel.RaiseEvent(events);
            gameObject.SetActive(false);
        }
    }

    public void MawangImageError()
    {
        TextEvent events = UIEvent.ErrorTextEvect;
        events.Text = "마왕의 위치정보 폴더의 'item.txt'를 찾을 수 없습니다.";
        events.textType = TextType.Error;
        events.isDefunct = false;

        textChannel.RaiseEvent(events);
    }

    public void CutSceneError()
    {
        TextEvent events = UIEvent.ErrorTextEvect;
        events.Text = "'공성로봇 필살기'를 찾을 수 없습니다.";
        events.textType = TextType.Error;
        events.isDefunct = true;

        textChannel.RaiseEvent(events);
    }
}
