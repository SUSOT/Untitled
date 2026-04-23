using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace _01.Script.LCM.NPC
{
public abstract class Npc : Entity
{
    
    [SerializeField] protected float _chatRadius;

    [SerializeField] protected LayerMask _whatIsPlayer;
    
    protected GameObject _textBox;
    protected TextMeshPro _textBoxText;
    
    public List<string> NpcChatTexts = new List<string>();
    
    protected string _nowText;
    protected int _npcIndex;

    protected Text _text;

    protected bool _isTalking = false;
    
    [SerializeField] protected float _waitTime = 2f;
    
    protected override void Awake()
    {
        base.Awake();
        _textBox = GetComponentInChildren<Image>().gameObject;
        _textBoxText = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    
    protected virtual void Start()
    {
        _textBox.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _chatRadius);
    }
#endif
}
}





