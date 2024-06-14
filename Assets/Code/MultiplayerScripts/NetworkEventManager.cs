using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beamable.Api.Notification.PubNubOp;

public class NetworkEventManager : MonoBehaviour
{
    public delegate void SimpleDelegateEvents();
    public static event SimpleDelegateEvents e_get_set_go,e_config_updated;


    public delegate void IntDelegateEvents(int _no);
    public static event IntDelegateEvents e_win_event, e_playercount;


    #region _EVENT INVOKERS

    public static void _EventConfigUpdated()
    {
        if (e_config_updated != null)
        {
            e_config_updated();
        }
    }


    public static void _EventStartGame()
    {
        if (e_get_set_go != null)
        {
            e_get_set_go();
        }
    }

    public static void _EventWon(int _no)
    {
        if (e_win_event != null)
        {
            e_win_event(_no);
        }
    }

    public static void _EventNewPlayerJoined(int _count)
    {
        if (e_playercount != null)
        {
            e_playercount(_count);
        }
    }

    #endregion

}
