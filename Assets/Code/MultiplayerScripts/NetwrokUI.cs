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
    public GameObject connectingUI;
    [Space]
    public GameObject starGameButton;
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
    }

    private void OnDisable()
    {

        NetworkEventManager.e_win_event -= _OnGameWon;
        NetworkEventManager.e_playercount -= _Playerjoined;
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
