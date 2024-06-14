using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private int clickCount = 0;
    private float startTime;

    private float m_diff;

    float m_last_time = 0;
    float m_currunt_time = 0;

    [Space]
    public RectTransform m_arrow;

    private Vector2 m_initial_pos;
    //MAX POSITION WILL GO HERE
    //LOW POS WILL BE ALWAYS ZERO AS ACNCOR SET
    private float m_max_y_pos = 600f;

    public bool m_clicking;

    private Vector2 m_pos;
    public float DowngradeSpeed;
    public float UpwordSpeed;
    [Space]
    public float m_reset_t;

    [Space]
    public float Incrimental;
    public float Decrimental;
    [Space]
    public float value = 0;


    private float m_reset;
    private void Start()
    {
        m_pos = m_arrow.anchoredPosition;
        m_last_time = Time.time;
        m_currunt_time = Time.time;

        m_initial_pos = m_arrow.anchoredPosition;

    }

    private void Update()
    {
        m_reset += Time.deltaTime;
        if (m_reset > m_reset_t)
        {
            m_clicking = false;
        }

        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            m_reset = 0f;
            m_clicking = true;
            m_currunt_time = Time.time;
            m_diff = m_currunt_time - m_last_time;
            m_last_time = m_currunt_time;
        }

        //_ChckClcker();

        if (m_clicking)
        {
            _MoveArrow();
        }
        else
        {
            value -= Decrimental;
            _MoveArrowToInitialPos();
        }

    }

    void _ChckClcker()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
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

        //Debug.Log(m_diff + "               " + m_pos.y);
        m_arrow.anchoredPosition = Vector2.Lerp(m_arrow.anchoredPosition, m_pos, UpwordSpeed * Time.deltaTime);
        //m_arrow.DOAnchorPos(m_pos, m_diff);
    }

}
