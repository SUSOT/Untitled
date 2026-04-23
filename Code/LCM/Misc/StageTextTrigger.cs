using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _01.Script.LCM.Misc
{
public class StageTextTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private bool _isTriggered = false;
    [SerializeField] private float _waitTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            StartCoroutine(Textcoroutine());
        }
    }

    private IEnumerator Textcoroutine()
    {
        _isTriggered = true;
        yield return _text.DOFade(1f, 1.5f).SetEase(Ease.InSine).WaitForCompletion();

        yield return new WaitForSeconds(_waitTime);
        
        yield return _text.DOFade(0f, 1.5f).SetEase(Ease.InSine).WaitForCompletion();
        
        Destroy(gameObject);
    }
}
}





