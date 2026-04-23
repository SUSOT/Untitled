using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class UIManager : MonoBehaviour,IEntityComponent
{
    [Header("Error")]
    [SerializeField] private GameEventChannelSO UIChannel;
    [SerializeField] private TextMeshProUGUI errorTextUI;

    [Header("Help")]
    [SerializeField] private TextMeshProUGUI helpTextUI;

    [SerializeField]
    private SpriteRenderer[] FadeParallax;

    private Player _player;
    private bool isHelpText;
    private KeyCode skipKey;

    public void Initialize(Entity entity)
    {
        UIChannel.AddListener<TextEvent>(HandleTextEvent);
        UIChannel.AddListener<ParallaxMoveEvent>(HandleParallaxMoveEvent);
        _player = entity as Player;

    }

    private void HandleParallaxMoveEvent(ParallaxMoveEvent obj)
    {
        print($"{obj.isFadeIn} / {obj.moveDirection.x}");
        if ((obj.isFadeIn && obj.moveDirection.x > 0) || (!obj.isFadeIn && obj.moveDirection.x < 0))
            Fade(true);
        else if ((obj.isFadeIn && obj.moveDirection.x < 0) || (!obj.isFadeIn && obj.moveDirection.x > 0))
            Fade(false);
    }

    private void Fade(bool isDirection)
    {
        if (isDirection)
        {
            for (int i = 0; i < FadeParallax.Length; i++)
            {
                FadeParallax[i].DOFade(1, 0.2f);
            }
        }
        else
        {
            for (int i = 0; i < FadeParallax.Length; i++)
            {
                FadeParallax[i].DOFade(0, 0.2f);
            }
        }
    }

    private void Update()
    {
        if(isHelpText && Input.GetKeyDown(skipKey))
        {
            isHelpText =false;
            StartCoroutine(FadeOutTextRoutine(helpTextUI));
        }
    }

    private void HandleTextEvent(TextEvent obj)
    {
        if(obj.textType == TextType.Error)
        {
            errorTextUI.color = Color.red;
            errorTextUI.text = $"Error : {obj.Text}";
            errorTextUI.DOFade(1, 1f);

            if (obj.isDefunct)
            {
                StartCoroutine(FadeOutTextRoutine(errorTextUI));
            }
        }
        else if (obj.textType == TextType.Help)
        {
            skipKey = obj.TextSkipKey;
            helpTextUI.text = obj.Text;
            helpTextUI.DOFade(1, 1f);
            isHelpText = true;
        }
        
    }

    public void ErrorTextClear()
    {
        errorTextUI.color = Color.green;
        errorTextUI.text = "해결되었습니다";
        StartCoroutine(FadeOutTextRoutine(errorTextUI));
    }

    private IEnumerator FadeOutTextRoutine(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(3f);
        text.DOFade(0, 1f);
    }

    
}
