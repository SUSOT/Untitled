using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace _01.Script.LCM.Item
{
public class GameIcon : MonoBehaviour, ITakeable
{
    [SerializeField] private float _playerCheckRadius;

    [SerializeField] private LayerMask _whatIsPlayer;

    [SerializeField] private GameObject _interactionKey;

    private float _firstItemYPosition;

    [SerializeField] private float _offsetItemYPosition;

    [SerializeField] private float _moveTime;

    private Tweener _tweener;

    private bool _isAlreadyTake = false;

    
    [SerializeField] private string _sceneName;

    public UnityEvent OnCameraShakeing;

    [SerializeField] private ParticleSystem _explosionParticle;

    [SerializeField] private CinemachineCamera _camera;

    [SerializeField] private GameObject _blackBoard;
    
    private SpriteRenderer _boardRenderer;

    private void Awake()
    {
        _boardRenderer = _blackBoard.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _firstItemYPosition = transform.position.y;
        
        _tweener = transform.DOMoveY(_firstItemYPosition + _offsetItemYPosition, _moveTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        _boardRenderer.DOFade(0f, 2f);
        
        AudioManager.Instance.StopAllLoopSound();
        AudioManager.Instance.PlaySound2D("Unity",0,true,SoundType.BGM);
    }


    public void TakeItem()
    {
        _isAlreadyTake = true;
        _interactionKey.SetActive(false);
        _tweener.Kill();
        StartCoroutine(ItemEffect());
    }

    private IEnumerator ItemEffect()
    {
        OnCameraShakeing?.Invoke();
        _explosionParticle.Play();
        AudioManager.Instance.PlaySound2D("GameExplosion",0,false,SoundType.SfX);
        yield return new WaitForSeconds(1.5f);
        _explosionParticle.Play();
        OnCameraShakeing?.Invoke();
        AudioManager.Instance.PlaySound2D("GameExplosion",0,false,SoundType.SfX);
        yield return new WaitForSeconds(0.5f);
        _camera.Priority = 20;
        _boardRenderer.DOFade(1f, 4f);
        for (int i = 0; i < 70; i++)
        {
            _camera.Lens.OrthographicSize -= 0.1f;
            _camera.Lens.Dutch += 5f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(_sceneName);
    }

    public void ShowInteraction()
    {
        if(_isAlreadyTake) return;
        
        if (CanTakeItem())
        {
            _interactionKey.SetActive(true);
        }
        else
        {
            _interactionKey.SetActive(false);
        }
    }

    private bool CanTakeItem()
    {
        return Physics2D.OverlapCircle(transform.position, _playerCheckRadius, _whatIsPlayer);
    }

    private void Update()
    {
        if (CanTakeItem() && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if(_isAlreadyTake == false)
                TakeItem();
        }
        ShowInteraction();

    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerCheckRadius);
    }

#endif
}
}





