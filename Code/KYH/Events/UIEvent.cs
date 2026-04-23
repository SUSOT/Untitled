using UnityEngine;

public enum TextType
{
    Help,
    Error
}

public static class UIEvent
{
    public static TextEvent ErrorTextEvect = new();
    public static ParallaxMoveEvent ParallaxMoveEvent = new();
}

public class TextEvent  : GameEvent
{
    public string Text;
    public bool isDefunct;
    public TextType textType;
    public KeyCode TextSkipKey;
}

public class ParallaxMoveEvent : GameEvent
{
    public bool isFadeIn;
    public Vector2 moveDirection;
}

