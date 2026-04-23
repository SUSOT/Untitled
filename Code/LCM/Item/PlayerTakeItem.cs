using UnityEngine.InputSystem;
using UnityEngine;

namespace _01.Script.LCM.Item
{
public class PlayerTakeItem : MonoBehaviour, ITakeable, IPoolable
{
    private bool _isAlreadyTake = false;

    [SerializeField] private string _poolName;
    
    [SerializeField] private float _playerCheckRadius;

    [SerializeField] private LayerMask _whatIsPlayer;

    [SerializeField] private GameObject _interactionKey;
    
    [SerializeField] private ItemType _itemType;

    [SerializeField] private float _healAmount;
    
    public string PoolName => _poolName;
    public GameObject ObjectPrefab => gameObject;
    
    public void TakeItem()
    {
        var player = Physics2D.OverlapCircle(transform.position, _playerCheckRadius, _whatIsPlayer);
        player.GetComponent<Player>().TakeItem(_itemType, _healAmount);
        PoolManager.Instance.Push(this);
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

    public void ResetItem()
    {
        
    }
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerCheckRadius);
    }

#endif
}

public enum ItemType
{
    Heal,
    Shield,
    MoreHeal,
    Invincibility
}
}





