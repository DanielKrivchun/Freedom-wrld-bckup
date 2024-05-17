using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetwrokUI : MonoBehaviour
{
    public GameObject countdownPanel;
    public GameObject startUI;
    [Space]
    public TextMeshProUGUI countdownText;


    public void _OpenStartUI()
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
