using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinContent : MonoBehaviour
{
    public TextMeshProUGUI NameText;

    public void _SetupMyData(string _name)
    {
        NameText.text = _name;
    }

}
