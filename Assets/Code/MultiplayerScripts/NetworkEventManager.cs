using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beamable.Api.Notification.PubNubOp;

public class NetworkEventManager : MonoBehaviour
{
    public delegate void SimpleDelegateEvents();
    public static event SimpleDelegateEvents e_get_set_go, e_config_updated, e_countdown_start, e_activae_stumble;


    public delegate void IntDelegateEvents(int _no);
    public static event IntDelegateEvents e_win_event, e_focus_on_player, e_player_speed_change,e_updated_my_no;

    public delegate void CameraDelegateEvents(_CamState _CamState);
    public static event CameraDelegateEvents e_camera_change;

    public delegate void LookatEvent(Transform _t);
    public static event LookatEvent e_text_lookat;

    public delegate void StingEvent(string _s);
    public static event StingEvent e_player_left, e_reset_player;


    #region _EVENT INVOKERS

    public static void _EventUpdateMyNo(int _no)
    {
        if (e_updated_my_no != null)
        {
            e_updated_my_no(_no);
        }
    }

    public static void _EventResetPlayerOnReplay(string _pname)
    {
        if (e_reset_player != null)
        {
            e_reset_player(_pname);
        }
    }

    public static void _EvantActivateStuble()
    {
        if (e_activae_stumble != null)
        {
            e_activae_stumble();
        }
    }
    public static void _EventPlayerLeft(string _s)
    {
        if (e_player_left != null)
        {
            e_player_left(_s);
        }
    }

    public static void _EventOnStopPlayer(int _a)
    {
        if (e_player_speed_change != null)
        {
            e_player_speed_change(_a);
        }
    }

    public static void _EventTextLookat(Transform _t)
    {
        if (e_text_lookat != null)
        {
            e_text_lookat(_t);
        }
    }


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

    public static void _EventCameraChange(_CamState _state)
    {
        if (e_camera_change != null)
        {
            e_camera_change(_state);
        }
    }

    #endregion

}
