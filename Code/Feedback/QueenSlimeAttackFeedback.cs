using System.Collections.Generic;
using UnityEngine;

public class QueenSlimeAttackFeedback : Feedback
{
    public List<GameObject> _enemys;
    private int rand;
    public override void PlayFeedback()
    {
        rand = Random.Range(0, _enemys.Count);
        Instantiate(_enemys[rand], transform.position, Quaternion.identity);
    }

    public override void StopFeedback()
    {
        
    }
}
