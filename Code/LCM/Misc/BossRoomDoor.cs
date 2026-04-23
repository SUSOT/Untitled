using System.Collections;
using System;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Misc
{
public class BossRoomDoor : MonoBehaviour
{
    [SerializeField] private float _needCount;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float _corTime;

    [SerializeField] private float _atkRadius;
    [SerializeField] private LayerMask _whatIsEnemy;

    public void Open()
    {
        _needCount--;
        if (_needCount == 0)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        var enemys = Physics2D.OverlapCircleAll(transform.position, _atkRadius, _whatIsEnemy);
        foreach (var enemy in enemys)
        {
            if (enemy.TryGetComponent(out global::_01.Script.LCM.Enemy.Core.Enemy enemyCompo))
            {
                enemyCompo.Dead();
            }
        }
        _particle.Play();
        yield return new WaitForSeconds(_corTime);
        _particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _atkRadius);
        Gizmos.color = Color.white;
    }
#endif
}
}





