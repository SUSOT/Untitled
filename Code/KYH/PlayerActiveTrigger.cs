using UnityEngine;

public enum KeyLocked
{
    Window,
    Attack,
    Jump,
    Move
}

public class PlayerActiveTrigger : MonoBehaviour
{
    public KeyLocked keyLocked;
    public bool isOn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            bool cnt = keyLocked switch
            {
                KeyLocked.Window => collision.GetComponent<Player>().isLockedWindow = isOn,
                KeyLocked.Attack => HardCodinng(collision),
                KeyLocked.Jump => throw new System.NotImplementedException(),
                KeyLocked.Move => collision.GetComponent<Player>().MoveStopOrGo(isOn),
                _ => throw new System.NotImplementedException()
            };
        }
    }

    public bool HardCodinng(Collider2D collision)
    {
        collision.GetComponent<Player>().BannedAttack(isOn);
        return true;
    }
}
