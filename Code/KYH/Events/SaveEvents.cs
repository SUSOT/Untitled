using UnityEngine;

public static class SaveEvents
{
    public static SaveEvent SaveEvent = new();
}

public class SaveEvent : GameEvent
{
    public Transform savePosition;
}
