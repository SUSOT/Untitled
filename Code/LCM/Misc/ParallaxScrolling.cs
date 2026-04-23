using UnityEngine;

namespace _01.Script.LCM.Misc
{
public class ParallaxScrolling : MonoBehaviour
{
    private Vector3 _startPos;
    private Camera _cam;
    [SerializeField] private float parallaxFactor;

    void Start()
    {
        _cam = Camera.main;
        _startPos = transform.position;
    }

    void FixedUpdate()
    {
        float distX = _cam.transform.position.x * parallaxFactor;
        float distY = _cam.transform.position.y * parallaxFactor;

        transform.position = new Vector3(_startPos.x + distX, _startPos.y + distY, transform.position.z);
    }
}
}





