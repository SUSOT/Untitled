using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : MonoBehaviour,IDamageable
{
    public delegate void OnDamageHandler(float damage, Vector2 direction, Vector2 knockBackPower, bool isPowerAttack, Entity dealer);
    public event OnDamageHandler OnDamage;

    public UnityEvent OnHit;
    public UnityEvent OnDead;

    public bool IsDead { get; set; } //사망처리 체크를 위한 거 해놓고
    public int DeadBodyLayer { get; private set; }

    protected Dictionary<Type, IEntityComponent> _components;

    protected virtual void Awake()
    {
        DeadBodyLayer = LayerMask.NameToLayer("DeadBody"); //레이어 값 셋팅

        _components = new Dictionary<Type, IEntityComponent>();
        AddComponentToDictionary();
        ComponentInitialize();
        AfterInitialize();
    }

    protected virtual void AddComponentToDictionary()
    {
        GetComponentsInChildren<IEntityComponent>(true).ToList().ForEach(compo => _components.Add(compo.GetType(), compo));
    }

    protected virtual void ComponentInitialize()
    {
        _components.Values.ToList().ForEach(compo => compo.Initialize(this));
    }

    protected virtual void AfterInitialize()
    {
        _components.Values.OfType<IAfterInit>().ToList().ForEach(compo => compo.AfterInitialize());
        OnHit.AddListener(HandleHit);
        OnDead.AddListener(HandleDead);
    }

    protected virtual void OnDestroy()
    {
        OnHit.RemoveListener(HandleHit);
        OnDead.RemoveListener(HandleDead);
    }

    protected abstract void HandleHit();
    protected abstract void HandleDead();

    public T GetCompo<T>(bool isDerived /*서브클래스 탐색 여부*/= false) where T : IEntityComponent //반드시 IEntityComponent를 구현해야 함
    {
        if (_components.TryGetValue(typeof(T), out IEntityComponent component))
            return (T)component;//GetCompo를 통해 들어온 제네릭이 딕셔너리에 추가되어 있는지 검사 후 있으면 반환

        if (isDerived == false) return default(T);//서브 클래스를 원하지 않으면 걍 기본값 내보냄 = 널?

        Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));//T의 파생클래스가 있는지 검사하고 있으면 가장 첫번째꺼를 뱉음
        if (findType != null)
            return (T)_components[findType];//찾았으면 그걸 뱉음

        return default(T);//없음 널 뱉음
    }

    public void ApplyDamage(float damage, Vector2 direction, Vector2 knockBackPower, bool isPowerAttack, Entity dealer)
            => OnDamage?.Invoke(damage, direction, knockBackPower, isPowerAttack, dealer);
}
