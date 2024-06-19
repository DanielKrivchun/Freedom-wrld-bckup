using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetwrokUI : NetworkBehaviour
{
    public GameObject startUI;
    public GameObject WinUI;
    public GameObject countdownPanel;
    [Space]
    public GameObject LoadingPanel;
    [Space]
    public GameObject starGameButton;
    [Header("Notification")]
    public GameObject NotificationPanel;
    public TextMeshProUGUI NotificationText;
    [Space]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI wintext;
    public TextMeshProUGUI PlayerCount;
    [Space]
    public GameObject wincontent;
    [Space]
    public List<WinContent> WinnerList;
    [Space]
    public RaceManager spawner;

    public static NetwrokUI Instance;

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
    }

    private void OnDisable()
    {

        NetworkEventManager.e_win_event -= _OnGameWon;
        NetworkEventManager.e_playercount -= _Playerjoined;
        NetworkEventManager.e_player_left -= _PlayerLeft;
    }

    private void _PlayerLeft(string _s)
    {
        NotificationText.text = _s;
        NotificationPanel.SetActive(true);
    }
    #endregion

    public void _LoadScene()
    {
        SceneManager.LoadScene("PetCareScene");
    }

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
        //LoadingPanel.SetActive(true);
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
        //LoadingPanel.SetActive(false);
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
        Debug.Log("Game Started Now");
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
