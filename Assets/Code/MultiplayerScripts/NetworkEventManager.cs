using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beamable.Api.Notification.PubNubOp;

public class NetworkEventManager : MonoBehaviour
{
    public delegate void SimpleDelegateEvents();
    public static event SimpleDelegateEvents e_get_set_go, e_config_updated, e_countdown_start;


    public delegate void IntDelegateEvents(int _no);
    public static event IntDelegateEvents e_win_event, e_playercount, e_focus_on_player;

    public delegate void CameraDelegateEvents(_CamState _CamState);
    public static event CameraDelegateEvents e_camera_cnage;


    #region _EVENT INVOKERS

    public static void _EventConfigUpdated()
    {
        if (e_config_updated != null)
        {
            e_config_updated();
        }
    }


    public static void _EventStartCountDown()
    {
        if (e_countdown_start != null)
        {
            e_countdown_start();
        }
    }

    public static void _EventFocusOnPlayer(int a)
    {
        if (e_focus_on_player != null)
        {
            e_focus_on_player(a);
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

    public static void _EventCameraChange(_CamState _state)
    {
        if (e_camera_cnage != null)
        {
            e_camera_cnage(_state);
        }
    }

    #endregion

}
