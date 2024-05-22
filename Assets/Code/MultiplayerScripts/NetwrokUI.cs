using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetwrokUI : NetworkBehaviour
{
    public GameObject startUI;
    public GameObject WinUI;
    public GameObject countdownPanel;
    [Space]
    public GameObject connectingUI;
    [Space]
    public GameObject starGameButton;
    [Space]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI wintext;
    [Space]
    public RaceManager spawner;

    public static NetwrokUI Instance;

    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        NetworkEventManager.e_win_event += _OnGameWon;
    }

    private void OnDisable()
    {

        NetworkEventManager.e_win_event -= _OnGameWon;
    }

    private void _OnGameWon(int _no)
    {
        WinUI.SetActive(true);
        wintext.text = "Your numbe is  " + _no;
    }

    public void _JoinRoom()
    {
        connectingUI.SetActive(true);
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
        connectingUI.SetActive(false);
        startUI.SetActive(false);
        countdownPanel.SetActive(true);
        StartCoroutine(_StartedCountDown());
    }

    IEnumerator _StartedCountDown()
    {
        int a = 5;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1f);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1f);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1f);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1f);
        a--;
        countdownText.text = a.ToString();
        yield return new WaitForSecondsRealtime(1f);
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
