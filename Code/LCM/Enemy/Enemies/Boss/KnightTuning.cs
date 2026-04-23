using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
[Serializable]
public class KnightMovementTuning
{
    public float runEnterDistance = 4f;
    public float runExitDistance = 4f;
    public float runSpeedBonus = 3f;
}

[Serializable]
public class KnightAttackTimingTuning
{
    public float attack1FirstSoundDelay = 0.3f;
    public float attack1SecondSoundDelay = 0.7f;

    public float attack2EffectDelay = 0.65f;
    public float attack2HorizontalBoost = 2f;

    public float attack3FirstWaveDelay = 0.6f;
    public float attack3SecondWaveDelay = 0.8f;

    public float attack4ChargeDuration = 1.6f;

    public float attack5FirstShotDelay = 0.6f;
    public float attack5SecondShotDelay = 0.4f;

    public float attack6ChargeDuration = 1.5f;
    public float attack6TrailStopDelay = 0.8f;
}

[Serializable]
public class KnightPhaseTuning
{
    public float phaseTwoHealthThresholdRatio = 0.5f;
    public float phaseTwoChargeDelay = 1.5f;
    public float phaseTwoExplosionDelay = 3f;
    public float phaseTwoRecoveryDelay = 2f;
}
}







