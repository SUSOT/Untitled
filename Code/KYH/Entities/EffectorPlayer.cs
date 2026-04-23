using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class EffectorPlayer : MonoBehaviour,IEntityComponent
{
    private Dictionary<string, ParticleSystem> _effectDictionary;
    private Animator _animationEffect;
    private Player _player;
    private ParticleSystem _currentAttackEffect;

    [SerializeField]
    private float DespawnEffSec;

    [TextArea]
    public string Descript;

    public void Initialize(Entity entity)
    {
        _player = entity as Player;
        _effectDictionary = new Dictionary<string, ParticleSystem>();
        GetComponentsInChildren<ParticleSystem>(true).ToList().ForEach(effect => _effectDictionary.Add(effect.name,effect));
        _animationEffect = GetComponentInChildren<Animator>(true);
    }

    public void PlayEffect(string effectName,bool ChildInPlayer = true)
    {
        ParticleSystem effect = _effectDictionary.GetValueOrDefault(effectName);
        Debug.Assert(effect != null, $"request attack data is not exist : {effect}");
        _currentAttackEffect = effect;

        if (!ChildInPlayer)
        {
            GameObject effectObj = Instantiate(effect,_player.transform).gameObject;
            effectObj.transform.SetParent(null);
            effectObj.GetComponent<ParticleSystem>().Play();
            StartCoroutine(RemoveEffectRoutine(effectObj));
        }
        else
            effect.Play();
    }

    public void PlayAnimationEffect(string effectName)
    {
        _animationEffect.Play(effectName);
    }

    public void StopEffect(string effectName)
    {
        ParticleSystem effect = _effectDictionary.GetValueOrDefault(effectName);
        Debug.Assert(effect != null, $"request attack data is not exist : {effect}");
        effect.Stop();
    }

    private IEnumerator RemoveEffectRoutine(GameObject instanceEffect)
    {
        yield return new WaitForSeconds(DespawnEffSec);
        Destroy(instanceEffect);
    }
}
