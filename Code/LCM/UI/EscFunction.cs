using DG.Tweening;
using System.Collections;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

namespace _01.Script.LCM.UI
{
public class EscFunction : MonoBehaviour
{
    [SerializeField] private Image _escPanel;
    [SerializeField] private Image _checkPanel;
    [SerializeField] private Image _warningPanel;
    [SerializeField] private Image _whitePanel;
    
    [SerializeField] private Image _redPanel;
    [SerializeField] private TextMeshProUGUI _countDownText;
    
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    
    private bool _isAnotherPanelShow = false;

    
    public void SetBgmVolume()
    {
        _audioMixer.SetFloat("BGM", Mathf.Log10(_bgmSlider.value) * 20);
    }

    public void SetSfxVolume()
    {
        _audioMixer.SetFloat("SFX", Mathf.Log10(_sfxSlider.value) * 20);
    }
    
    private CanvasGroup _canvasGroup;
    
    private bool _isEnd = true;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowEscPanel();
        }
    }

    public void ShowEscPanel()
    {
        
        if(!IsEnd()) return;
        
        
        _isEnd = false;
        Time.timeScale = 0;
        _whitePanel.DOFade(0.5f, 1.5f).SetUpdate(true);
        _escPanel.rectTransform.DOMoveY(540f, 1.5f).SetEase(Ease.InOutBounce).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
        _canvasGroup.blocksRaycasts = true;
    }

    public void CloseEscPanel()
    {
        
        if(!IsEnd()) return;
        
        if(_isAnotherPanelShow) return;
        _isEnd = false;
        _whitePanel.DOFade(0f, 1.5f).SetUpdate(true);
        _escPanel.rectTransform.DOMoveY(-460f, 1.5f).SetEase(Ease.InOutBounce).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
        Time.timeScale = 1f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void CheckExitGamePanel()
    {
        
        
        if(!IsEnd()) return;
        
        _isEnd = false;
        _whitePanel.DOColor(Color.red, 1.5f).SetUpdate(true);
        _whitePanel.DOFade(0.5f, 1.5f).SetUpdate(true);
        _isAnotherPanelShow = true;
        _checkPanel.rectTransform.DOMoveY(540f, 1.5f).SetEase(Ease.InBounce).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
    }

    public void ExitGameBtn()
    {
        
        StartCoroutine(GemeQuitCoroutine());
    }

    private IEnumerator GemeQuitCoroutine()
    {
        Time.timeScale = 1f;
        _redPanel.gameObject.SetActive(true);
        _countDownText.gameObject.SetActive(true);
        for (int i = 3; i >= 0; i--)
        {
            _countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseExitGamePanel()
    {
        
        if(!IsEnd()) return;
        
        _isEnd = false;
        _isAnotherPanelShow = false;
        _whitePanel.DOColor(Color.white, 1.5f).SetUpdate(true);
        _whitePanel.DOFade(0.5f, 1.5f).SetUpdate(true);
        _checkPanel.rectTransform.DOMoveY(-460f, 1.5f).SetEase(Ease.InBounce).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
    }

    public void ShowWarningPanel()
    {
        if(!IsEnd()) return;
        
        _isEnd = false;
        _whitePanel.DOColor(Color.red, 1.5f).SetUpdate(true);
        _whitePanel.DOFade(1f, 1.5f).SetUpdate(true);
        _warningPanel.rectTransform.DOMoveY(540f, 1.5f).SetEase(Ease.InSine).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
    }

    public void CloseWarnigPanelAndExitPanel()
    {
        if(!IsEnd()) return;
        
        _isEnd = false;
        _whitePanel.DOColor(Color.white, 1.5f).SetUpdate(true);
        _whitePanel.DOFade(0.5f, 1.5f).SetUpdate(true);
        _isAnotherPanelShow = false;
        _checkPanel.rectTransform.DOMoveY(-460f, 1.5f).SetEase(Ease.InSine).SetUpdate(true);
        _warningPanel.rectTransform.DOMoveY(-460f, 1.5f).SetEase(Ease.InSine).SetUpdate(true).onComplete = () =>
        {
            _isEnd = true;
        };
    }

    private bool IsEnd()
    {
        return _isEnd;
    }
}
}





