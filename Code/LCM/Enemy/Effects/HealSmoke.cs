using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.Effects
{
public class HealSmoke : MonoBehaviour
{
    [SerializeField] private Vector2 _checkBoxSize;
    [SerializeField] private LayerMask _whatIsEnemy;

    [SerializeField] private float _healAmount;
    [SerializeField] private float _waitTime;

    private Coroutine _healCoroutine;

    private void OnEnable()
    {
        _healCoroutine = StartCoroutine(HealLoopCoroutine());
    }

    private void OnDisable()
    {
        if (_healCoroutine == null)
            return;

        StopCoroutine(_healCoroutine);
        _healCoroutine = null;
    }

    private IEnumerator HealLoopCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_waitTime);

        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _checkBoxSize, 0, _whatIsEnemy);
            for (int i = 0; i < colliders.Length; i++)
            {
                EntityHealth health = colliders[i].GetComponentInChildren<EntityHealth>();
                if (health != null)
                    health.TakeHeal(_healAmount);
            }

            yield return wait;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _checkBoxSize);
        Gizmos.color = Color.white;
    }
#endif
}
}







