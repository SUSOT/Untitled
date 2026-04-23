using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TrapTileMap : MonoBehaviour
{
    public UnityEvent unityEvent;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        unityEvent?.Invoke();
        AudioManager.Instance.PlaySound2D("ExplosionBridge",0,false,SoundType.SfX);
        yield return new WaitForSeconds(0.5f);
        unityEvent?.Invoke();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 5f;
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySound2D("ExplosionInWater", 0, false, SoundType.SfX);
        Destroy(gameObject);
    }
}
