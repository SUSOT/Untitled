using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerSignal : MonoBehaviour
{
    public string sceneName;

    public void NextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
