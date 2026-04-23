using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueComponent : MonoBehaviour
{
    public TextMeshProUGUI npcChatText;
    public CanvasGroup textBoxCanvas;

    public float typingSpeed = 0.05f;

    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void ComeNear()
    {
        _renderer.sortingLayerName = "Enemy";
        _renderer.color = Color.white;
        _renderer.flipX = true;
    }

    public void GoneFar()
    {
        _renderer.sortingLayerName = "Enemy";
        _renderer.color = Color.white;
        _renderer.flipX = false;
    }
}
