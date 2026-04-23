using UnityEngine;
using Unity.Cinemachine;

public class WindowSceneDirector : MonoBehaviour
{
    [SerializeField] private CinemachineCamera windowSceneCamera;
    private CinemachineBasicMultiChannelPerlin perlin;
    private int amplitudeGain;
    private int frequencyGain;

    private void Awake()
    {
        //perlin = windowSceneCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().;
    }

    public void CameraShake(float second)
    {

    }
}
