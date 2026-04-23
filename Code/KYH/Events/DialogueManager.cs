using UnityEngine;
using TMPro;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour, IEntityComponent
{
    [SerializeField] private GameEventChannelSO dialogueChannel;

    private Entity _entity;
    private Player _player;
    private EntityMover _mover;
    private EntityRenderer _renderer;

    public string[] currentDialogue { get; private set; }

    private TextMeshProUGUI playerChatText;
    public TextMeshProUGUI npcChatText;
    private string songName;
    private CanvasGroup textBoxCanvas;
    private Image textBoxImage;

    bool isSkip;
    bool isPrevAttackBoolValue = false;
    int talkNum;
    float typingSpeed = 0.05f;

    public bool turnPlayer;

    private NpcDialogueComponent talker;


    [SerializeField] private GameEventChannelSO cameraChannel;
    private PanDirection panDirection;
    private float panDistance = 3f;
    private float panTime = 0.35f;

    int cutSceneNum;
    private PlayableDirector[] director;

    private CinemachineCamera leftCamera;
    private CinemachineCamera rightCamera;

    private bool isSwap;

    #region �ϵ��ڵ��Դϴ� �ð��� �����
    public UnityEvent firstEvent;
    public UnityEvent secondEvent;
    public UnityEvent thirdEvent;

    public CanvasGroup hpBar;
    #endregion

    private void SendPanEvent(bool isRewind)
    {
        PanEvent evt = CameraEvents.PanEvent;
        evt.panTime = panTime;
        evt.distance = panDistance;
        evt.direction = panDirection;
        evt.isRewindToStart = isRewind;

        cameraChannel.RaiseEvent(evt);
    }


    public void Initialize(Entity entity)
    {
        _entity = entity;
        _player = _entity as Player;
        _mover = _entity.GetCompo<EntityMover>();
        _renderer = _entity.GetCompo<EntityRenderer>();

        _renderer.OnFlip += Flip;
        _player.PlayerInput.OnAttackKeyPressed += HandleClick;
        dialogueChannel.AddListener<StartAConversation>(HandleSpeak);
        dialogueChannel.AddListener<StartATalkEachOther>(HandleEachOtherSpeak);

        textBoxCanvas = GetComponentInChildren<CanvasGroup>();
        textBoxImage = textBoxCanvas.GetComponentInChildren<Image>();
        playerChatText = GetComponentInChildren<TextMeshProUGUI>();
    }

    

    private void OnDestroy()
    {
        _renderer.OnFlip -= Flip;
        _player.PlayerInput.OnAttackKeyPressed -= HandleClick;
        dialogueChannel.RemoveListener<StartAConversation>(HandleSpeak);
        dialogueChannel.RemoveListener<StartATalkEachOther>(HandleEachOtherSpeak);
    }

    private void HandleClick()
    {
        if (!isSkip) return;
        isSkip = false;

        if (talker == null)
            NextChat();
        else
            NextChatEachOther(turnPlayer);
    }

    private void NextChat()
    {
        playerChatText.text = null;
        talkNum++;

        if (talkNum == currentDialogue.Length)
        {
            EndTalk();
            return;
        }
        if (turnPlayer)
            AudioManager.Instance.PlaySound2D("HumanSpeak", 0, true, SoundType.SfX);
        else
            AudioManager.Instance.PlaySound2D("NpcSpeak", 0, true, SoundType.SfX);
        StartCoroutine(TypingRoutine(currentDialogue[talkNum]));
    }

    private void NextChatEachOther(bool isPlayerTurn)
    {
        if (isPlayerTurn)
            playerChatText.text = null;
        else
            npcChatText.text = null;

        talkNum++;

        if (talkNum == currentDialogue.Length)
        {
            EndTalk();
            return;
        }

        if (currentDialogue[talkNum].Contains("Event"))
        {
            director[cutSceneNum].Play();
            talkNum++;
            cutSceneNum++;
            HideChatBox(textBoxCanvas);
            HideChatBox(talker.textBoxCanvas);
        }
        else
        {
            
            StartCoroutine(EachOhterTypingRoutine(currentDialogue[talkNum]));
        }
    }

    public void CutSceneEnd()
    {
        print("dddd");
        
        if(currentDialogue != null)
        StartCoroutine(EachOhterTypingRoutine(currentDialogue[talkNum]));
    }

    private void Flip(bool LeftOrRight)
    {
        if(LeftOrRight)
            textBoxImage.transform.Rotate(0,180f,0);
        else
            textBoxImage.transform.Rotate(0, 0, 0);
    }


    private void EndTalk()
    {
        hpBar.DOFade(1, 0.2f);
        turnPlayer = true;
        isSwap = false;
        _mover.CanManualMove = true;
        _player.isDialogue = false;
        talkNum = 0;
        cutSceneNum = 0;
        HideChatBox(textBoxCanvas);
        if(talker != null)
        HideChatBox(talker.textBoxCanvas);
        talker = null;
    }
    public void ShowChatBox(CanvasGroup canvasGroup)
    {
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.2f);
    }
    private void HideChatBox(CanvasGroup canvasGroup)
    {
        DOTween.To(()=> canvasGroup.alpha,x => canvasGroup.alpha = x,0,0.2f);
    }

    private void HandleSpeak(StartAConversation events)
    {
        hpBar.DOFade(0,0.2f);
        turnPlayer = true;
        if (events.isStop)
        {
            _mover.CanManualMove = false;
            _mover.StopImmediately(true);
        }

        _player.isDialogue = true;
        currentDialogue = events.dialogue;
        if (turnPlayer)
            AudioManager.Instance.PlaySound2D("HumanSpeak", 0, true, SoundType.SfX);
        else
            AudioManager.Instance.PlaySound2D("NpcSpeak", 0, true, SoundType.SfX);
        StartCoroutine(TypingRoutine(currentDialogue[talkNum]));
    }

    private void HandleEachOtherSpeak(StartATalkEachOther obj)
    {
        hpBar.DOFade(0, 0.2f);

        _mover.CanManualMove = false;
        _mover.StopImmediately(true);

        _player.isDialogue = true;
        currentDialogue = obj.dialogue;
        talker = obj.talker;
        turnPlayer = obj.startTalkerIsPlayer;
        npcChatText = talker.npcChatText;

        songName = obj.changeSongName;

        panDistance = obj.npcDistance;
        panDirection = obj.npcDirection;
        panTime = obj.panTime;

        director = obj.director;

        leftCamera = obj.leftCamera;
        rightCamera = obj.rightCamera;

        print(leftCamera);

        if(!turnPlayer)
            SendPanEvent(false);

        
        StartCoroutine(EachOhterTypingRoutine(currentDialogue[talkNum]));
    }
    private void ChangeTalker()
    {
        if (turnPlayer)
        {
            HideChatBox(textBoxCanvas);
            playerChatText.text = null;
        }
        else
        {
            HideChatBox(talker.textBoxCanvas);
            npcChatText.text = null;
        }

        turnPlayer = !turnPlayer;

    }

    private IEnumerator EachOhterTypingRoutine(string talk)
    {
        if (talk.Contains("Next"))
        {
            talk = talk.Replace("Next", "");
            ChangeTalker();
            SendPanEvent(false);
        }

        if (talk.Contains("Song"))
        {
            talk = talk.Replace("Song", "");
            AudioManager.Instance.PlaySound2D(songName,0,true,SoundType.BGM);
        }

        if (talk.Contains("CameraChange"))
        {
            talk = talk.Replace("CameraChange", "");
            CameraSwap();
        }

        if (turnPlayer)
            AudioManager.Instance.PlaySound2D("HumanSpeak", 0, true, SoundType.SfX);
        else
            AudioManager.Instance.PlaySound2D("NpcSpeak", 0, true, SoundType.SfX);

        if (turnPlayer)
        {
            ShowChatBox(textBoxCanvas);
            playerChatText.text = null;
            SendPanEvent(true);
        }
        else
        {
            ShowChatBox(talker.textBoxCanvas);
            npcChatText.text = null;
        }


        if (talk.Contains("  ")) talk = talk.Replace("  ", "\n");

        for (int i = 0; i < talk.Length; i++)
        {
            if(turnPlayer)
            {
                playerChatText.text += talk[i];
                yield return new WaitForSeconds(typingSpeed);
            }
            else
            {
                npcChatText.text += talk[i];
                yield return new WaitForSeconds(talker.typingSpeed);
            }
        }
        AudioManager.Instance.StopLoopSound("NpcSpeak");
        AudioManager.Instance.StopLoopSound("HumanSpeak");
        isSkip = true;
    }

    private void CameraSwap()
    {
        SwapCameraEvent swapEvt = CameraEvents.SwapCameraEvent;
        swapEvt.leftCamera = leftCamera;
        swapEvt.rightCamera = rightCamera;
        if(isSwap)
            swapEvt.moveDirection = Vector2.left;
        else
            swapEvt.moveDirection = Vector2.right;

        isSwap = !isSwap;
        cameraChannel.RaiseEvent(swapEvt);
    }

    private IEnumerator TypingRoutine(string talk)
    {

        ShowChatBox(textBoxCanvas);
        playerChatText.text = null;

        if (talk.Contains("  ")) talk = talk.Replace("  ", "\n");
        if (talk.Contains("Act1"))
        {
            talk = talk.Replace("Act1", "");
            firstEvent?.Invoke();
        }
        if (talk.Contains("Act2"))
        {
            talk = talk.Replace("Act2", "");
            secondEvent?.Invoke();
        }
        if (talk.Contains("Act3"))
        {
            talk = talk.Replace("Act3", "");
            thirdEvent?.Invoke();
        }

        for (int i =0; i<talk.Length; i++)
        {
            playerChatText.text += talk[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        AudioManager.Instance.StopLoopSound("HumanSpeak");
        isSkip = true;
    }
}
