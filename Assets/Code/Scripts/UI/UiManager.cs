using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("UI OBJECTS")]
    public GameObject InGmmeUI;
    public TextMeshProUGUI PetNameText;
    [Space]
    public RectTransform m_arrow;
    public Image Filler;
    [Header("SCRIPTABLE OBJECTS")]
    public PetConfigs PetConfigs;

    public float DowngradeSpeed = 1f;
    public float UpwordSpeed = 2f;
    [Space]
    public float SteminaDrain;

    [Header("FLOAT AND VALUES STEMINA BAR VALUE MANAGER")]
    public float Incrimental = 0.3F;
    public float Decrimental = 0.1F;

    [Space]
    public float m_reset_t;

    [Header("UI BUTTONS")]
    public Button TapButton;

    #region PRIVATE VARIABLES
    private float red_drain = 0.005f;
    private float yellow_drain = 0f;
    private float greem_drain = 0.01f;
    private float ypos;
    private float m_reset;
    private float m_diff;
    private float m_last_time = 0;
    private float m_currunt_time = 0;
    private float FillAmount;
    private float m_max_y_pos = 600f;
    private float MyStemina;
    private float CurrnutStemina;

    private bool m_clicking = false;
    private bool StaminaDrained;
    private bool GameStarted;

    private Vector2 m_initial_pos;
    private Vector2 m_pos;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        m_pos = m_arrow.anchoredPosition;
        m_last_time = Time.time;
        m_currunt_time = Time.time;
        m_initial_pos = m_arrow.anchoredPosition;
    }
    private void OnEnable()
    {
        TapButton.onClick.AddListener(_Tap);
        NetworkEventManager.e_config_updated += _ConfigUodated;
        NetworkEventManager.e_get_set_go += _StartStaminaBar;
        NetworkEventManager.e_win_event += _OnWon;
    }

    private void OnDisable()
    {
        TapButton.onClick.RemoveListener(_Tap);
        NetworkEventManager.e_config_updated -= _ConfigUodated;
        NetworkEventManager.e_get_set_go -= _StartStaminaBar;
        NetworkEventManager.e_win_event -= _OnWon;
    }


    private void Update()
    {
        if (!GameStarted) return;
        m_reset += Time.deltaTime;
        if (m_reset > m_reset_t)
        {
            m_clicking = false;
        }

        if (m_clicking)
        {
            _MoveArrow();
        }
        else
        {
            //value -= Decrimental;
            _MoveArrowToInitialPos();
        }

        _SetArrowBasedMultiplier();


        if (GameStarted)
        {
            _SteminaChanges();
        }
    }
    #endregion

    #region EVENT LISTENERS 

    /// <summary>
    /// ON GAME WON
    /// </summary>
    /// <param name="_no"></param>
    private void _OnWon(int _no)
    {
        GameStarted = false;
        InGmmeUI.SetActive(false);
    }

    /// <summary>
    /// WHEN PLAYER CONFIGURATION GETS UPDATED
    /// </summary>
    private void _ConfigUodated()
    {
        //Calculate max stemina here
        MyStemina = PetConfigs.maxstemina;
        CurrnutStemina = MyStemina;
        StaminaDrained = false;
        FillAmount = 1f;
        Filler.fillAmount = FillAmount;
        TapButton.gameObject.SetActive(true);
        //Debug.Log("_ConfigUodated");
    }

    /// <summary>
    /// STARTING TAP BUTTON STEMINA BAR
    /// </summary>
    private void _StartStaminaBar()
    {
        _ConfigUodated();
        PetNameText.text = RaceManager.instance.LocalPlayerNickname + "'s" + " Pet";
        GameStarted = true;
        InGmmeUI.SetActive(true);
    }
    #endregion

    #region BUTTON LISTENER
    /// <summary>
    /// TAP TAP BUTTON LISTENER
    /// </summary>
    private void _Tap()
    {
        m_reset = 0f;
        m_clicking = true;
        m_currunt_time = Time.time;
        m_diff = m_currunt_time - m_last_time;
        m_last_time = m_currunt_time;
    }
    #endregion

    #region ARROW AND STEMINA BAR CALCULATIONS
    void _SteminaChanges()
    {
        CurrnutStemina -= SteminaDrain;
        FillAmount = ((100f * CurrnutStemina) / MyStemina) / 100f;
        Filler.fillAmount = FillAmount;
    }

    void _MoveArrowToInitialPos()
    {
        m_arrow.anchoredPosition = Vector2.Lerp(m_arrow.anchoredPosition, m_initial_pos, DowngradeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// MOVE ARROW BASED ON SPEED
    /// </summary>
    void _MoveArrow()
    {
        if (m_diff > 1.5f)
        {
            m_diff = 1.5f;
        }

        //CALCULATION HERE
        m_pos.y = -(m_max_y_pos) * (m_diff) / (1.5f);
        m_arrow.anchoredPosition = Vector2.Lerp(m_arrow.anchoredPosition, m_pos, UpwordSpeed * Time.deltaTime);
    }

    /// <summary>
    /// MULTIPLAYER GETS SET OVER HER
    /// </summary>
    void _SetArrowBasedMultiplier()
    {
        if (FillAmount <= 0 && !StaminaDrained)
        {
            StaminaDrained = true;
            SteminaDrain = red_drain;
            PetConfigs.TapMultiplier = 0.8f;
            TapButton.gameObject.SetActive(false);
            return;
        }

        if (StaminaDrained) return;

        ypos = m_arrow.anchoredPosition.y;

        if (ypos >= -600f && ypos < -460f)
        {
            //Debug.Log("Bottom Red");
            PetConfigs.TapMultiplier = 0.8f;
            SteminaDrain = red_drain;
            return;
        }

        if (ypos >= -460f && ypos < -290f)
        {
            //Debug.Log("Yellow Red");
            PetConfigs.TapMultiplier = 1f;
            SteminaDrain = yellow_drain;
            return;
        }

        if (ypos > -290f && ypos < -160f)
        {
            //Debug.Log("Green Red");
            PetConfigs.TapMultiplier = 1.5f;
            SteminaDrain = greem_drain;
            return;
        }

        if (ypos > -160f && ypos < 0f)
        {
            //Debug.Log("Top Red");
            PetConfigs.TapMultiplier = 0.8f;
            SteminaDrain = red_drain;
            return;
        }
    }
    #endregion

}
