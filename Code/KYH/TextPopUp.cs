using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TextPopUp : MonoBehaviour
{
    //시간이 없습니다
    public TextMeshProUGUI text;
    public Image popUp;
    public Transform prevTrans;
    public Image uiPopUp;
    public Transform middle;
    public Image blackPanel;

    public void TextShow()
    {
        text.DOFade(1,1);
    }

    public void TextHide()
    {
        text.DOFade(0, 1);
    }

    public void TutorialPopUpMoveMiddle()
    {
        Sequence mySequence = DOTween.Sequence();

        uiPopUp.gameObject.SetActive(true);
        mySequence.Append(popUp.rectTransform.DOMove(middle.position, 1)).Join(popUp.GetComponent<CanvasGroup>().DOFade(0, 1f)).Append(uiPopUp.GetComponent<CanvasGroup>().DOFade(1, 0.1f)).Append
        (uiPopUp.rectTransform.DOScale(3f, 1f).SetEase(Ease.InBounce)).Join(blackPanel.DOFade(0.5f,0.5f));

        mySequence.Play();
        
    }

    public void TutorialPopUpCancel()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(uiPopUp.rectTransform.DOScale(1.518f, 1f).SetEase(Ease.InBounce)).Join(uiPopUp.GetComponent<CanvasGroup>().DOFade(0, 1f)).Append
        (popUp.rectTransform.DOMove(prevTrans.position, 1)).Join(popUp.GetComponent<CanvasGroup>().DOFade(1, 0.2f)).Join(blackPanel.DOFade(0f, 0.5f));
        uiPopUp.gameObject.SetActive(false);


        mySequence.Play();

    }
}
