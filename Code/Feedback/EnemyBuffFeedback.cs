using System;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

public class EnemyBuffFeedback : Feedback
{
    [SerializeField] private ParticleSystem _buffParticle;
    [SerializeField] private float _buffDuration;
    private float _curTime = 0f;
    private Enemy _enemy;

    private bool _isTakeBuff = false;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    public override void PlayFeedback()
    {
        if (_enemy.gameObject.layer != LayerMask.NameToLayer("DeadBody"))
        {
            _enemy.EntityHealth.IsShield = true;
            _buffParticle.Play();
            _isTakeBuff = true;
        }
    }

    private void Update()
    {
        if (_isTakeBuff)
        {
            _curTime += Time.deltaTime;
            if (_curTime >= _buffDuration)
            {
                _buffParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _enemy.EntityHealth.IsShield = false;
                _isTakeBuff = false;
                _curTime = 0f;
            }
        }
    }

    public override void StopFeedback()
    {
        
    }
}
