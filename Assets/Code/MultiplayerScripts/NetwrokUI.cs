using DG.Tweening;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetwrokUI : NetworkBehaviour
{
    public GameObject startUI;
    public GameObject WinUI;
    public GameObject countdownPanel;
    public GameObject LeaveUI;
    public GameObject LeavePopup;
    [Space]
    public GameObject LoadingPanel;
    [Space]
    public GameObject starGameButton;
    [Header("Notification")]
    public GameObject NotificationPanel;
    public RectTransform NofificationObj;
    public TextMeshProUGUI NotificationText;
    [Header("Replay")]
    public GameObject ReplayCanvas;
    public RectTransform ReplayPopup;
    public TextMeshProUGUI ReplayText;
    [Space]
    [Header("In-Game Text")]
    public TextMeshProUGUI countdownText;

    public TextMeshProUGUI CurruntRankNo;
    [Header("WIN SCREEN")]
    public TextMeshProUGUI wintext;
    public TextMeshProUGUI Cointext;
    public TextMeshProUGUI Xptext;
    [Space]
    public GameObject wincontent;
    [Space]
    public List<WinContent> WinnerList;
    [Space]
    public RaceManager spawner;
    [Space]
    [Header("Buttons")]
    public Button LeaveButton;
    public Button InGameLeave;
    public Button ContinueButton;
    public Button ExitButton;
    [Header("Reply")]
    public Button ReplayButton;
    public Button Yes;
    public Button No;
    [Space]
    private bool requestedReplay;

    public static NetwrokUI Instance;

    private IEnumerator _ienumrator;

    private void Awake()
    {
        Instance = this;
    }

    #region UNITY METHODS

    private void OnEnable()
    {
        NetworkEventManager.e_win_event += _OnGameWon;
        NetworkEventManager.e_player_left += _PlayerLeft;
        NetworkEventManager.e_updated_my_no += _UpdatedRank;

        LeaveButton.onClick.AddListener(_OnLeaveButton);
        InGameLeave.onClick.AddListener(_OnLeaveButton);
        ContinueButton.onClick.AddListener(_ContinueRace);
        ExitButton.onClick.AddListener(_YesLeave);
        ReplayButton.onClick.AddListener(_ReplayButtonClick);
        Yes.onClick.AddListener(_Yes);
        No.onClick.AddListener(_No);
    }



    private void OnDisable()
    {
        NetworkEventManager.e_win_event -= _OnGameWon;
        NetworkEventManager.e_player_left -= _PlayerLeft;
        NetworkEventManager.e_updated_my_no -= _UpdatedRank;

        InGameLeave.onClick.RemoveListener(_OnLeaveButton);
        LeaveButton.onClick.RemoveListener(_OnLeaveButton);
        ContinueButton.onClick.RemoveListener(_ContinueRace);
        ExitButton.onClick.RemoveListener(_YesLeave);
        ReplayButton.onClick.RemoveListener(_ReplayButtonClick);
        Yes.onClick.RemoveListener(_Yes);
        No.onClick.RemoveListener(_No);
    }

    private void _UpdatedRank(int _no)
    {
        CurruntRankNo.text = _no.ToString();
    }

    private void _Yes()
    {
        _DisableReplay();
        RPC_YesToReplay(RaceManager.instance.LocalPlayerNickname);
    }

    private void _DisableReplay()
    {
        ReplayCanvas.SetActive(false);
        WinUI.gameObject.SetActive(false);
    }

    private void _No()
    {
        Debug.Log("Remove me from server and update path numbers");
    }

    private void _ReplayButtonClick()
    {
        //RUN THIS ON SERVER
        //if (Runner.IsServer)
        //{
        //    NetworkEventManager._EventResetPlayerOnReplay(RaceManager.instance.LocalPlayerNickname);
        //}
        _DisableReplay();
        requestedReplay = true;
        RPC_ReplayNotificationSend(RaceManager.instance.LocalPlayerNickname);
        NetworkEventManager._EventCameraChange(_CamState.InitialCam);
    }

    private async void _PlayerLeft(string _s)
    {
        NotificationText.text = _s + " is left";
        NotificationPanel.SetActive(true);
        NofificationObj.DOAnchorPosY(-50f, 1f);

        if (_ienumrator != null)
        {
            StopCoroutine(_ienumrator);
        }

        _ienumrator = _DisableNotification();
        StartCoroutine(_ienumrator);
    }

    IEnumerator _DisableNotification()
    {
        yield return new WaitForSecondsRealtime(5f);
        NofificationObj.DOAnchorPosY(1000f, 1f);
        _ienumrator = null;
    }

    #endregion

    #region TEXT UPDATE

    public void _UpdatedText(float _coins, float _xp)
    {
        Cointext.text = "Coins Gained :" + _coins.ToString();
        Xptext.text = "XP Gained :" + _xp.ToString();
    }
    #endregion

    #region LOCAL BUTTONS AND METHDOS
    private void _OnLeaveButton()
    {
        LeavePopup.transform.localScale = Vector3.zero;
        LeaveUI.SetActive(true);
        LeavePopup.transform.DOScale(1f, 0.5f);
    }

    private void _ContinueRace()
    {
        LeaveUI.SetActive(false);
    }

    private void _YesLeave()
    {
        SceneManager.LoadScene("PetCareScene");
    }


    #endregion


    public void _SetupList(List<string> _s)
    {
        foreach (var item in _s)
        {
            Debug.Log(item);
        }

        for (int i = 0; i < _s.Count; i++)
        {
            WinnerList[i]._SetupMyData(_s[i]);
            WinnerList[i].gameObject.SetActive(true);
            WinnerList[i].transform.DOScale(1f, 0.5f);
        }

        wincontent.gameObject.SetActive(true);
    }



    private void _OnGameWon(int _no)
    {
        WinUI.SetActive(true);
        wintext.text = "You Finished: " + _no;
    }

    public void _JoinRoom()
    {
        spawner._StartGame(Fusion.GameMode.AutoHostOrClient);
    }

    public void _OpenStartUI()
    {
        startUI.SetActive(true);
        starGameButton.SetActive(true);
    }

    public void _StartRace()
    {
        Debug.Log("Start Race");
        RPC_StartGame();
    }

    public void _StartCountDown()
    {
        startUI.SetActive(false);
        countdownPanel.SetActive(true);
        StartCoroutine(_StartedCountDown());
        NetworkEventManager._EventStartCountDown();
    }

    IEnumerator _StartedCountDown()
    {
        NetworkEventManager._EventFocusOnPlayer(0);
        int a = 5;
        countdownText.text = a.ToString();
        //yield return new WaitForSecondsRealtime(1.5f);
        //NetworkEventManager._EventFocusOnPlayer(1);
        //a--;
        //countdownText.text = a.ToString();
        //yield return new WaitForSecondsRealtime(1.5f);
        //NetworkEventManager._EventFocusOnPlayer(2);
        //a--;
        //countdownText.text = a.ToString();
        //yield return new WaitForSecondsRealtime(1.5f);
        //NetworkEventManager._EventFocusOnPlayer(3);
        //a--;
        //countdownText.text = a.ToString();
        //yield return new WaitForSecondsRealtime(1.5f);
        //NetworkEventManager._EventFocusOnPlayer(4);
        //a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        countdownText.text = a.ToString();
        countdownPanel.SetActive(false);
        //Debug.Log("Game Started Now");
        spawner._StartGameForPlayers();
    }

    #region REPLAY METHOS
    public void _ReplayRPCRecived(string _name)
    {
        ReplayPopup.transform.localScale = Vector3.zero;
        ReplayCanvas.SetActive(true);
        ReplayText.text = _name + " Want's to play again!";
        ReplayPopup.transform.DOScale(1f, 0.5f);
    }
    #endregion

    #region RPC Remote Procedure Call
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        Debug.Log("Started Game now");
        _StartCountDown();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReplayNotificationSend(string _pname)
    {
        Debug.Log("Recived Replay Notification ");
        _RecivedReplayNotification(_pname);
        NetworkEventManager._EventCameraChange(_CamState.InitialCam);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_YesToReplay(string _name)
    {
        if (Runner.IsServer)
        {
            NetworkEventManager._EventResetPlayerOnReplay(_name);
        }
    }

    void _RecivedReplayNotification(string _name)
    {
        if (Runner.IsServer)
        {
            NetworkEventManager._EventResetPlayerOnReplay(_name);
        }

        if (requestedReplay) return;
        _ReplayRPCRecived(_name);
    }



    #endregion
}
