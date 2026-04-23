using UnityEngine;

public interface IPoolable
{
    public string PoolName { get; }
    public GameObject ObjectPrefab { get; }
    public void ResetItem();
}
