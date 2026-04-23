using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Pool/List")]
public class PoolListSO : ScriptableObject
{
    public List<PoolItemSO> list;
}
