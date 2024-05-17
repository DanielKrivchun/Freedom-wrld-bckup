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

    }

    public void _StartCountDown()
    {
        StartCoroutine(_StartedCountDown());
    }

    IEnumerator _StartedCountDown()
    {
        yield return null;
    }
}
