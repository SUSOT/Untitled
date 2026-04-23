using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    public PoolListSO poolList;
    private Dictionary<string, Pool> _pools;

    private void Awake()
    {
        _pools = new Dictionary<string, Pool>();
        foreach (PoolItemSO so in poolList.list)
        {
            CreatePool(so);
        }
    }

    private void CreatePool(PoolItemSO so)
    {
        IPoolable poolable = so.prefab.GetComponent<IPoolable>();
        if(poolable == null)
        {
            Debug.LogWarning($"Gameobject {so.prefab.name} has no IPoolable Script");
            return;
        }

        Pool pool = new Pool(poolable, transform, so.count);
        _pools.Add(poolable.PoolName, pool);
    }

    public IPoolable Pop(string itemName)
    {
        if (_pools.ContainsKey(itemName))
        {
            IPoolable item = _pools[itemName].Pop();
            item.ResetItem();
            return item;
        }
        Debug.LogError($"There is no pool {itemName}");
        return null;
    }

    public void Push(IPoolable item)
    {
        if (_pools.ContainsKey(item.PoolName))
        {
            _pools[item.PoolName].Push(item);
            return;
        }
        Debug.LogError($"There is no pool {item.PoolName}");
    }
}
