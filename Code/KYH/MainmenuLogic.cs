using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainmenuLogic : MonoBehaviour
{
    public Image background;
    private RectTransform rectTransform;
    private float speed = 500f;
    private CinemachineCamera _camera;
    private bool isStart;
    public Image black;
    public GameObject pp;
    private void Awake()
    {
        rectTransform = background.GetComponent<RectTransform>();
        _camera = FindAnyObjectByType<CinemachineCamera>();
    }
    private void Start()
    {
        AudioManager.Instance.PlaySound2D("MainMenuBGM",0,true,SoundType.BGM);
    }
    private void Update()
    {
        if(!isStart)
        {
            Vector3 point = Input.mousePosition;

            if (point.x > 1100 && rectTransform.position.x > -1.34f)
            {
                print("��");
                rectTransform.Translate(new Vector3(-0.01f, 0, 0) * speed * Time.deltaTime);
            }
            if (point.x < 1000 && rectTransform.position.x < 1.36f)
            {
                print("��");
                rectTransform.Translate(new Vector3(0.01f, 0, 0) * speed * Time.deltaTime);
            }
            if (point.y < 550 && rectTransform.position.y < 0.7f)
            {
                print("�Ʒ�");
                rectTransform.Translate(new Vector3(0, 0.01f, 0) * speed * Time.deltaTime);
            }
            if (point.y > 450 && rectTransform.position.y > -0.77f)
            {
                print("��");
                rectTransform.Translate(new Vector3(0, -0.01f, 0) * speed * Time.deltaTime);
            }
        }
    }

    public void OnButtonDown()
    {
        if (isStart) return;
        StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        isStart = true;
        AudioManager.Instance.StopAllLoopSound();
        for (int i = 0; i < 70; i++)
        {
            if(_camera.Lens.OrthographicSize > 0)
                _camera.Lens.OrthographicSize -= 0.1f;
            _camera.Lens.Dutch += 5f;
            yield return new WaitForSeconds(0.01f);
        }
        pp.SetActive(false);
        black.DOFade(1,1).OnComplete(()=> SceneManager.LoadScene("LCM"));
    }
}
