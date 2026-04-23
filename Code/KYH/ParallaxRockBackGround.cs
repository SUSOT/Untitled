using UnityEngine;

public class ParallaxRockBackGround : MonoBehaviour, IEntityComponent
{
    private EntityRenderer _renderer;

    public void Initialize(Entity entity)
    {
        _renderer = entity.GetCompo<EntityRenderer>();
        _renderer.OnFlip += Flip;
    }
    private void OnDestroy()
    {
        _renderer.OnFlip -= Flip;
    }

    private void Flip(bool LeftOrRight)
    {
        if (LeftOrRight)
            transform.Rotate(0, 180f, 0);
        else
            transform.Rotate(0, 0, 0);
    }
}