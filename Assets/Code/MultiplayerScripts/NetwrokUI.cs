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
    [Header("UI OBJECTS")]
    public GameObject startUI;
    public GameObject WinUI;
    public GameObject countdownPanel;
    [Header("Leave UI")]
    public GameObject LeaveUI;
    public GameObject LeavePopup;
    public GameObject LeaveText;
    public GameObject WinLeaveText;
    [Space]
    public GameObject LoadingPanel;
    [Header("JOIN ROOM")]
    public GameObject JoinRoomPanel;
    public GameObject StartGamePanel;
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
    public GameObject RankBG;
    public TextMeshProUGUI CurruntRankNo;
    [Header("WIN SCREEN")]
    public TextMeshProUGUI wintext;
    public TextMeshProUGUI Cointext;
    public TextMeshProUGUI Xptext;
    [Space]
    public GameObject wincontent;
    [Space]
    public List<TextMeshPro> WinnerNames;
    [Space]
    public RaceManager RaceManagerRef;
    [Space]
    [Header("Buttons")]
    [Header("JOIN ROOM")]
    public Button JoinRoomButton;
    [Header("START GAME & AI PLAYER")]
    public Button StartGameButton;
    public Button AIPlayerButton;
    [Space]
    public Button LeaveButton;
    public Button InGameLeave;
    public Button ContinueButton;
    public Button ExitButton;
    [Space]
    public Button TestButton;
    [Header("Reply")]
    public Button LeaveButtonWinUI;
    public Button ReplayButton;
    public Button Yes;
    public Button No;
    [Space]
    [Space]
    public float time;
    public Ease EaseRef;
    [Space]
    private bool requestedReplay;

    public static NetwrokUI Instance;

    private IEnumerator _ienumrator;

    private bool button_waiter;

    public bool LeavedGame;

    #region UNITY METHODS
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        //EVENTS ARE ADDED HERE
        NetworkEventManager.e_win_event += _OnGameWon;
        NetworkEventManager.e_get_set_go += _ResetOnStart;
        NetworkEventManager.e_player_left += _PlayerLeft;
        NetworkEventManager.e_updated_my_no += _UpdatedRank;

        NetworkEventManager.e_host_migration_done += _HostMigrated;
        NetworkEventManager.e_game_complete += _GameIsComplete;

        //BUTTON LISTENERS
        JoinRoomButton.onClick.AddListener(_JoinRoom);
        LeaveButton.onClick.AddListener(_OnLeaveButton);
        LeaveButtonWinUI.onClick.AddListener(_OnWinLeave);
        InGameLeave.onClick.AddListener(_OnLeaveButton);
        ContinueButton.onClick.AddListener(_ContinueRace);
        ExitButton.onClick.AddListener(_YesLeave);
        ReplayButton.onClick.AddListener(_ReplayButtonClick);
        Yes.onClick.AddListener(_Yes);
        No.onClick.AddListener(_No);
        StartGameButton.onClick.AddListener(_StartRace);
        AIPlayerButton.onClick.AddListener(_GenrateAIPlayer);
        TestButton.onClick.AddListener(_TestButton);

    }



    private void OnDisable()
    {
        NetworkEventManager.e_win_event -= _OnGameWon;
        NetworkEventManager.e_player_left -= _PlayerLeft;
        NetworkEventManager.e_get_set_go -= _ResetOnStart;
        NetworkEventManager.e_updated_my_no -= _UpdatedRank;

        NetworkEventManager.e_host_migration_done -= _HostMigrated;
        NetworkEventManager.e_game_complete -= _GameIsComplete;


        LeaveButtonWinUI.onClick.RemoveListener(_OnWinLeave);
        TestButton.onClick.RemoveListener(_TestButton);
        JoinRoomButton.onClick.RemoveListener(_JoinRoom);
        InGameLeave.onClick.RemoveListener(_OnLeaveButton);
        LeaveButton.onClick.RemoveListener(_OnLeaveButton);
        ContinueButton.onClick.RemoveListener(_ContinueRace);
        ExitButton.onClick.RemoveListener(_YesLeave);
        ReplayButton.onClick.RemoveListener(_ReplayButtonClick);
        Yes.onClick.RemoveListener(_Yes);
        No.onClick.RemoveListener(_No);
        StartGameButton.onClick.RemoveListener(_StartRace);
        AIPlayerButton.onClick.RemoveListener(_GenrateAIPlayer);

    }

    #endregion

    #region EVENT CALLBACKS
    /// <summary>
    /// CALLBACK WHEN GAME WIN
    /// </summary>
    /// <param name="_no"></param>
    private void _OnGameWon(int _no)
    {
        WinUI.SetActive(true);
        wintext.text = "You Finished: " + _no;
    }

    /// <summary>
    /// CALLBACK ON GAME COMPLETE
    /// </summary>
    private void _GameIsComplete()
    {
        if (RaceManager.instance.TotalRealPlayers >= 2)
        {
            ReplayButton.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// ON HOST MIGRATION
    /// </summary>
    private void _HostMigrated()
    {
        Debug.Log("Host is migrated to me ");

    }

    /// <summary>
    /// RESET NAMES ON PODIAM ON START OF THE GAME
    /// </summary>
    private void _ResetOnStart()
    {
        foreach (var item in WinnerNames)
        {
            item.text = "";
        }
    }

    /// <summary>
    /// RANK WILL BE UPDATED ON THIS
    /// </summary>
    /// <param name="_no"></param>
    private void _UpdatedRank(int _no)
    {
        if (!RankBG.activeSelf)
        {
            RankBG.SetActive(true);
        }
        CurruntRankNo.text = _no.ToString();
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

    #region BUTTON LISTENERS

    private async void _Yes()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(Yes.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        LoadingPanel.SetActive(true);
        _WaitAndDisableLoadingPanel();
        _DisableReplay();
        RPC_YesToReplay(RaceManagerRef.LocalPlayerNickname, RaceManagerRef.LocalnetworkID);
    }

    private void _DisableReplay()
    {
        ReplayCanvas.SetActive(false);
        WinUI.gameObject.SetActive(false);
    }

    private async void _No()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(No.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        Debug.Log("Remove me from server and update path numbers");
        _YesLeave();
    }

    private async void _ReplayButtonClick()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(ReplayButton.transform);
        await Utils._Waiter(200);
        button_waiter = false;
  
        if (RaceManager.instance.TotalRealPlayers <= 1)
        {
            NetworkEventManager._EventNetworkErrors(_Strings.LeftOnlyOnePlay);
            return;
        }

        LoadingPanel.SetActive(true);
        _WaitAndDisableLoadingPanel();
        _DisableReplay();
        requestedReplay = true;
        RPC_ReplayNotificationSend(RaceManagerRef.LocalPlayerNickname, RaceManagerRef.LocalnetworkID);
        NetworkEventManager._EventCameraChange(_CamState.Start);
    }


    private async void _WaitAndDisableLoadingPanel()
    {
        await Utils._Waiter(2500);
        LoadingPanel.SetActive(false);
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
    private async void _OnWinLeave()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(LeaveButton.transform);
        Utils._DoButtonAnimation(InGameLeave.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        WinLeaveText.SetActive(true);
        LeaveText.SetActive(false);
        LeavePopup.transform.localScale = Vector3.zero;
        LeaveUI.SetActive(true);
        Utils._DoUIPopup(LeavePopup.transform, time, EaseRef);
    }

    private async void _OnLeaveButton()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(LeaveButton.transform);
        Utils._DoButtonAnimation(InGameLeave.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        WinLeaveText.SetActive(false);
        LeaveText.SetActive(true);
        LeavePopup.transform.localScale = Vector3.zero;
        LeaveUI.SetActive(true);
        Utils._DoUIPopup(LeavePopup.transform, time, EaseRef);
        //LeavePopup.transform.DOScale(1f, 0.5f);
    }

    private async void _ContinueRace()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(ContinueButton.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        LeaveUI.SetActive(false);
    }

    private async void _YesLeave()
    {
        LeavedGame = true;
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(ExitButton.transform);
        await Utils._Waiter(200);
        await RaceManager.instance._LeaveGame();
        button_waiter = false;
        SceneManager.LoadScene(_Strings.PetCareScene);
    }


    #endregion

    #region ON GAME WIN CODE BLOCK
    /// <summary>
    /// WINNER PLAYER NAMES GET SET OVER HERE
    /// </summary>
    /// <param name="_s"></param>
    public void _SetupList(List<string> _s)
    {
        foreach (var item in _s)
        {
            Debug.Log(item);
        }

        for (int i = 0; i < _s.Count; i++)
        {
            WinnerNames[i].text = _s[i];
            WinnerNames[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// SET NAME OF PLAYER WHO FINISHES GAME ON PODIUM
    /// </summary>
    /// <param name="_no"></param>
    /// <param name="_name"></param>
    public void _SetMyNameOnPodium(int _no, string _name)
    {
        Debug.Log("No " + _no);
        WinnerNames[_no - 1].text = _name;
    }

    #endregion

    #region BUTTON LISTENERS

    /// <summary>
    /// PHOTON ROOM JOINING ON BUTTON
    /// </summary>
    public async void _JoinRoom()
    {
        if (button_waiter) return;
        LoadingPanel.SetActive(true);
        button_waiter = true;
        Utils._DoButtonAnimation(JoinRoomButton.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        JoinRoomPanel.SetActive(false);
        RaceManagerRef._StartGame(GameMode.AutoHostOrClient);
    }

    public async void _TestButton()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(TestButton.transform, EaseRef);
        await Utils._Waiter(200);
        button_waiter = false;

    }

    #endregion

    #region CONNECTED TO PHOTON OPEN START UI
    /// <summary>
    /// ON START OF THE GAME START UI GET;S ACTIVATED
    /// </summary>
    public void _OpenStartUI()
    {
        startUI.SetActive(true);
        StartGamePanel.SetActive(true);
        startUI.GetComponent<Image>().DOFade(0f, 0.2f);
    }

    /// <summary>
    /// CLIENT START UI GET'S FADED 
    /// </summary>
    public void _StartUIforClients()
    {
        startUI.GetComponent<Image>().DOFade(0f, 0.2f);
    }
    #endregion

    #region START GAME AND COUNTDOWN
    /// <summary>
    /// STARTING RACE OF THE GAME 
    /// </summary>
    public async void _StartRace()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(StartGameButton.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        Debug.Log("Start Race");
        RaceManagerRef.startgamenow = true;
        RPC_StartGame();
    }
    /// <summary>
    /// AI PLAYER GENRATION BUTTON
    /// </summary>
    private async void _GenrateAIPlayer()
    {
        if (button_waiter) return;
        button_waiter = true;
        Utils._DoButtonAnimation(AIPlayerButton.transform);
        await Utils._Waiter(200);
        button_waiter = false;
        //RaceManagerRef._GenrateAIPlayer();
    }

    /// <summary>
    /// COUNT DOWN CALLED BY RPC 
    /// </summary>
    public void _StartCountDown()
    {
        startUI.SetActive(false);
        countdownPanel.SetActive(true);
        StartCoroutine(_StartedCountDown());
        NetworkEventManager._EventStartCountDown();
        _ResetUI();
    }

    void _ResetUI()
    {
        wintext.text = "";
        Cointext.text = "";
        Xptext.text = "";
    }

    /// <summary>
    /// CHROUTINE FOR COUNTDOWN
    /// </summary>
    /// <returns></returns>
    IEnumerator _StartedCountDown()
    {
        NetworkEventManager._EventFocusOnPlayer(0);
        int a = 5;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        Utils._DoButtonAnimation(countdownText.transform);
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        Utils._DoButtonAnimation(countdownText.transform);
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        Utils._DoButtonAnimation(countdownText.transform);
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        Utils._DoButtonAnimation(countdownText.transform);
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        Utils._DoButtonAnimation(countdownText.transform);
        countdownText.text = a.ToString();
        countdownPanel.SetActive(false);
        requestedReplay = false;
        RaceManagerRef._StartGameForPlayers();
    }
    #endregion

    #region REPLAY METHOS
    /// <summary>
    /// RPC RECIVED 
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    public void _ReplayRPCRecived(string _name, string _id)
    {
        ReplayPopup.transform.localScale = Vector3.zero;
        ReplayCanvas.SetActive(true);
        ReplayText.text = _name + " Want's to play again!";
        ReplayPopup.transform.DOScale(1f, 0.5f);
    }
    #endregion

    #region RPC Remote Procedure Call
    /// <summary>
    /// START GAME RPC CALLED BY CLIENT OR HOST
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        Debug.Log("Started Game now");
        _StartCountDown();
    }

    /// <summary>
    /// REPLAY NOTIFICATION RPC CALL
    /// </summary>
    /// <param name="_pname"></param>
    /// <param name="_id"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReplayNotificationSend(string _pname, string _id)
    {
        Debug.Log("Recived Replay Notification ");
        _RecivedReplayNotification(_pname, _id);
        NetworkEventManager._EventCameraChange(_CamState.InitialCam);
    }

    /// <summary>
    /// YES TO REPLAY RPC CALL
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_YesToReplay(string _name, string _id)
    {
        if (Runner.IsServer)
        {
            NetworkEventManager._EventResetPlayerOnReplay(_name, _id);
        }
        else
        {
            NetworkEventManager._EventResetClientData(_name, _id);
        }
    }

    /// <summary>
    /// CALLBACK ON REPLAY NOTIFICATION
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    void _RecivedReplayNotification(string _name, string _id)
    {
        if (Runner.IsServer)
        {
            NetworkEventManager._EventResetPlayerOnReplay(_name, _id);
        }

        if (requestedReplay) return;
        _ReplayRPCRecived(_name, _id);
    }
    #endregion
}
