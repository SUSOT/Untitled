using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FolderCutSceneUIMover : MonoBehaviour
{
    public RawImage folderScreenImage;
    public RawImage playerScreenImage;


    public void Act1()
    {
        folderScreenImage.DOFade(1,0.1f);
    }

    public void Act2()
    {
        folderScreenImage.GetComponent<RectTransform>().transform.DOLocalMoveX(-432, 0.4f);
        playerScreenImage.DOFade(1,0.2f);
    }

    public void Act3()
    {
        playerScreenImage.DOFade(0, 0.1f);
        folderScreenImage.DOFade(0, 0.1f);
    }
}
