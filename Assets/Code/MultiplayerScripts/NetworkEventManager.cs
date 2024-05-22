using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEventManager : MonoBehaviour
{
    public delegate void SimpleDelegateEvents();
    public static event SimpleDelegateEvents e_get_set_go;


    public delegate void IntDelegateEvents(int _no);
    public static event IntDelegateEvents e_win_event;


    #region _EVENT INVOKERS
    public static void _EventStartGame()
    {
        if (e_get_set_go != null)
        {
            e_get_set_go();
        }
    }

    public static void _EventWon()
    {
        if (e_get_set_go != null)
        {
            e_get_set_go();
        }
    }
    #endregion

}
