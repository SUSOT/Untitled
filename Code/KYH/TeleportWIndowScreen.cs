using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TeleportWIndowScreen : MonoBehaviour,IEntityComponent,IAfterInit
{
    
    [field:SerializeField] public Transform windowScreenTrans;
    [SerializeField] private Vector2 enterKnockbackForce;


    private Player _player;
    private Vector3 prevTrans;
    private EntityMover _mover;

    private CinemachineCamera prevCamera;
    public CinemachineCamera windowScreenCamera;
    public CinemachineCamera firstWindowScreenCamera;
    private bool isFirst;

    [SerializeField] private int activeCameraPriority = 15;
    [SerializeField] private int disableCameraPriority = 10;

    [SerializeField] private GameEventChannelSO cameraChannel;

    private bool isEnterWindow;

    [Header("Video")]
    private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoImage;

    private EntityHealth _health;

    private Coroutine coroutine;

    [SerializeField]
    private GameObject pp;

    private string prevSongName;

    public void Initialize(Entity entity)
    {
        _player = entity as Player;
        _mover = entity.GetCompo<EntityMover>();
        videoPlayer = videoImage.GetComponent<VideoPlayer>();
        _health = entity.GetCompo<EntityHealth>();
    }

    public void AfterInitialize()
    {
        _health.hp.OnValueChanged += CancelWindow;
        _player.PlayerInput.OnEnterWindowKeyPressed += HandleEnterWindowScreen;
    }

    public void CancelWindow(float prev, float next)
    {
        if(coroutine !=null)
        StopCoroutine(coroutine);
        isEnterWindow = false;
        _mover.CanManualMove = true;
    }

    public void VideoEvent()
    {
        videoPlayer.Play();
        videoImage.DOFade(1, 1);
    }

    public void VideoEnd()
    {
        videoImage.DOFade(0, 1);
    }


    private IEnumerator WindowEffect()
    {
        _mover.CanManualMove = false;
        prevTrans = _player.transform.position;
        _mover.EffectorPlayer.PlayEffect("ReadyEnterWindow",true);
        yield return new WaitForSeconds(2f);
        _mover.EffectorPlayer.PlayEffect("EnterWindow", true);
        AudioManager.Instance.PlaySound2D("WindowEnterSFX");
        AudioManager.Instance.PlaySound2D("WindowSceneBGM",0,true,SoundType.BGM);
        yield return new WaitForSeconds(0.25f);
        VideoEnd();
        _mover.EffectorPlayer.StopEffect("ReadyEnterWindow");
        _player.transform.position = windowScreenTrans.position;
        _mover.KnockBack(enterKnockbackForce, 0.5f);
        pp.SetActive(false);

        Vector2 exitDirection = Vector2.right;
        _player.MaxJumpCount = 8;
        _player.ResetJumpCount();
        prevCamera = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None)
                        .FirstOrDefault(cam => cam.Priority == activeCameraPriority);

        SwapCameraEvent swapEvt = CameraEvents.SwapCameraEvent;
        swapEvt.leftCamera = prevCamera;
        if(isFirst)
            swapEvt.rightCamera = firstWindowScreenCamera;
        else
            swapEvt.rightCamera = windowScreenCamera;
        swapEvt.moveDirection = exitDirection;
        swapEvt.isBattonFollow = false;
        _mover.CanManualMove = true;
        isFirst = false;

        cameraChannel.RaiseEvent(swapEvt);
    }

    private void OnDestroy()
    {
        _player.PlayerInput.OnEnterWindowKeyPressed -= HandleEnterWindowScreen;
    }

    public void HandleEnterWindowScreen()
    {
        if (_player.isLockedWindow|| isEnterWindow || !_mover.CanManualMove) return;
        isEnterWindow = true;
        _mover.CanManualMove = false;
        prevSongName = AudioManager.Instance.CurrentMainBGMName;
        AudioManager.Instance.StopAllLoopSound();
        coroutine = StartCoroutine(WindowEffect());
    }

    public void FirstInToWindow()
    {
        isEnterWindow = true;
        isFirst = true;
        prevSongName = AudioManager.Instance.CurrentMainBGMName;
        AudioManager.Instance.StopAllLoopSound();
        coroutine = StartCoroutine(WindowEffect());
    }

    public void ComebackPrevTrans()
    {
        AudioManager.Instance.StopLoopSound("WindowSceneBGM");
        if(prevSongName.Length > 0)
            AudioManager.Instance.PlaySound2D(prevSongName, 0, true, SoundType.BGM);
        prevSongName = null;

        pp.SetActive(true);
        isEnterWindow = false;
        _player.transform.position = prevTrans;
        _mover.KnockBack(enterKnockbackForce, 0.5f);
        prevTrans = Vector3.zero;

        _player.MaxJumpCount = 1;
        _player.ResetJumpCount();

        Vector2 exitDirection = Vector2.right;

        SwapCameraEvent swapEvt = CameraEvents.SwapCameraEvent;
        swapEvt.leftCamera = windowScreenCamera;
        swapEvt.rightCamera = prevCamera;
        swapEvt.moveDirection = exitDirection;
        swapEvt.isBattonFollow = false;

        cameraChannel.RaiseEvent(swapEvt);
    }
}
