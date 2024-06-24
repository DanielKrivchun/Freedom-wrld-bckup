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
    [Space]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI PlayerCount;
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
    public Button ContinueButton;
    public Button ExitButton;

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
        NetworkEventManager.e_playercount += _Playerjoined;
        NetworkEventManager.e_player_left += _PlayerLeft;

        LeaveButton.onClick.AddListener(_OnLeaveButton);
        ContinueButton.onClick.AddListener(_ContinueRace);
        ExitButton.onClick.AddListener(_YesLeave);

    }

    private void OnDisable()
    {

        NetworkEventManager.e_win_event -= _OnGameWon;
        NetworkEventManager.e_playercount -= _Playerjoined;
        NetworkEventManager.e_player_left -= _PlayerLeft;

        LeaveButton.onClick.RemoveListener(_OnLeaveButton);
        ContinueButton.onClick.RemoveListener(_ContinueRace);
        ExitButton.onClick.RemoveListener(_YesLeave);
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


    private void _Playerjoined(int _no)
    {
        PlayerCount.text = _no.ToString();
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
        yield return new WaitForSecondsRealtime(1.5f);
        NetworkEventManager._EventFocusOnPlayer(1);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        NetworkEventManager._EventFocusOnPlayer(2);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        NetworkEventManager._EventFocusOnPlayer(3);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        NetworkEventManager._EventFocusOnPlayer(4);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1.5f);
        a--;
        countdownText.text = a.ToString();
        countdownPanel.SetActive(false);
        //Debug.Log("Game Started Now");
        spawner._StartGameForPlayers();
    }

    #region RPC Remote Procedure Call
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        Debug.Log("Started Game now");
        _StartCountDown();
    }
    #endregion



}
