using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetwrokUI : MonoBehaviour
{
    public GameObject startUI;
    public GameObject countdownPanel;
    [Space]
    public GameObject connectingUI;
    [Space]
    public GameObject starGameButton;
    [Space]
    public TextMeshProUGUI countdownText;
    [Space]
    public NetworkSpawner spawner;

    public static NetwrokUI Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void _JoinRoom()
    {
        connectingUI.SetActive(true);
        spawner._GameMode(Fusion.GameMode.AutoHostOrClient);
    }

    public void _OpenStartUI()
    {
        startUI.SetActive(true);
        starGameButton.SetActive(true);
    }

    public void _StartRace()
    {
        spawner._StartGame();
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

        Debug.Log("Game Started Now");
    }
}
