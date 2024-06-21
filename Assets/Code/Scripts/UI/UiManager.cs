using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private float m_diff;

    float m_last_time = 0;
    float m_currunt_time = 0;

    [Space]
    public GameObject InGmmeUI;
    public RectTransform m_arrow;
    [Space]
    public Image Filler;
    [Space]
    public PetConfigs PetConfigs;

    public float MyStemina;
    public float CurrnutStemina;

    private Vector2 m_initial_pos;
    //MAX POSITION WILL GO HERE
    //LOW POS WILL BE ALWAYS ZERO AS ACNCOR SET
    private float m_max_y_pos = 600f;

    public bool m_clicking;

    private Vector2 m_pos;
    public float DowngradeSpeed;
    public float UpwordSpeed;
    [Space]
    public float SteminaDrain;


    private float red_drain = 0.005f;
    private float yellow_drain = 0f;
    private float greem_drain = 0.01f;

    [Space]
    public float m_reset_t;

    [Space]
    public float Incrimental;
    public float Decrimental;
    [Space]
    public float value = 0;
    [Space]
    public Button TapButton;


    private float ypos;

    private float m_reset;


    public bool GameStarted;


    private float FillAmount;
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

    private void _OnWon(int _no)
    {
        GameStarted = false;
        InGmmeUI.SetActive(false);
    }

    private void _ConfigUodated()
    {
        //Calculate max stemina here
        MyStemina = PetConfigs.maxstemina;
        CurrnutStemina = MyStemina;
        Debug.Log("_ConfigUodated");
    }

    private void _StartStaminaBar()
    {
        GameStarted = true;
        InGmmeUI.SetActive(true);
    }

    private void _Tap()
    {
        m_reset = 0f;
        m_clicking = true;
        m_currunt_time = Time.time;
        m_diff = m_currunt_time - m_last_time;
        m_last_time = m_currunt_time;
    }

    private void Start()
    {
        m_pos = m_arrow.anchoredPosition;
        m_last_time = Time.time;
        m_currunt_time = Time.time;
        m_initial_pos = m_arrow.anchoredPosition;
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
            value -= Decrimental;
            _MoveArrowToInitialPos();
        }

        _SetArrowBasedMultiplier();


        if (GameStarted)
        {
            _SteminaChanges();
        }
    }

    void _SteminaChanges()
    {
        CurrnutStemina -= SteminaDrain;
        FillAmount = ((100f * CurrnutStemina) / MyStemina) / 100f;
        Filler.fillAmount = FillAmount;
    }



    void _ChckClcker()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_reset = 0f;
            m_clicking = true;
            value += Incrimental;
        }
    }


    void _MoveArrowToInitialPos()
    {
        m_arrow.anchoredPosition = Vector2.Lerp(m_arrow.anchoredPosition, m_initial_pos, DowngradeSpeed * Time.deltaTime);
    }
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

    void _SetArrowBasedMultiplier()
    {
        ypos = m_arrow.anchoredPosition.y;

        if (ypos >= -600f && ypos < -460f)
        {
            Debug.Log("Bottom Red");
            PetConfigs.TapMultiplier = 0.8f;
            SteminaDrain = red_drain;
            return;
        }

        if (ypos >= -460f && ypos < -290f)
        {
            Debug.Log("Yellow Red");
            PetConfigs.TapMultiplier = 1f;
            SteminaDrain = yellow_drain;
            return;
        }

        if (ypos > -290f && ypos < -160f)
        {
            Debug.Log("Green Red");
            PetConfigs.TapMultiplier = 1.5f;
            SteminaDrain = greem_drain;
            return;
        }

        if (ypos > -160f && ypos < 0f)
        {
            Debug.Log("Top Red");
            PetConfigs.TapMultiplier = 0.8f;
            SteminaDrain = red_drain;
            return;
        }
    }

}
