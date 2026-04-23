using UnityEngine;

public static class PlayerEvents
{
        public static readonly CounterSuccessEvent CounterSuccessEvent = new();
}

public class CounterSuccessEvent : GameEvent
{
    public Transform target;
}
