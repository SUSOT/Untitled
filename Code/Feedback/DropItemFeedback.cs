using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using _01.Script.LCM.Item;

public class DropItemFeedback : Feedback
{
    public List<PoolItemSO> _dropItems;

    [SerializeField] private float _dropProbability;

    private Entity _entity;
    private PlayerFeedback _feedback;

    private void Awake()
    {
        _entity = GetComponentInParent<Entity>();
        _feedback = GetComponent<PlayerFeedback>();
    }

    private void Start()
    {
        _entity.OnDead.AddListener(_feedback.PlayFeedbacks);
    }

    public override void PlayFeedback()
    {
        int rand = Random.Range(0, 101);
        if (rand <= _dropProbability)
        {
            int index = Random.Range(0, _dropItems.Count);
            var item = PoolManager.Instance.Pop(_dropItems[index].poolName) as PlayerTakeItem;
            item.transform.position = transform.position;
        }
    }

    public override void StopFeedback()
    {
        
    }
}
