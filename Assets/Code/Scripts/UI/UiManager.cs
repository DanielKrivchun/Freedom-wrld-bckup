using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UiManager : MonoBehaviour
{
    private int clickCount = 0;
    private float startTime;

    private float m_diff;

    float m_last_time = 0;
    float m_currunt_time = 0;

    [Space]
    public RectTransform m_arrow;





    //MAX POSITION WILL GO HERE
    //LOW POS WILL BE ALWAYS ZERO AS ACNCOR SET
    private float m_max_y_pos = 600f;

    private bool m_clicking;

    private Vector2 m_pos;

    private void Start()
    {
        m_pos = m_arrow.anchoredPosition;
        m_last_time = Time.time;
        m_currunt_time = Time.time;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            m_clicking = true;
            m_currunt_time = Time.time;
            m_diff = m_currunt_time - m_last_time;
            m_last_time = m_currunt_time;
            _MoveArrow();
        }
    }
    void _MoveArrow()
    {

        if (m_diff > 1.5f)
        {
            m_diff = 1.5f;
        }

        //CALCULATION HERE
        m_pos.y = -(m_max_y_pos) * (m_diff) / (1.5f);

        Debug.Log(m_diff + "               " + m_pos.y);
        m_arrow.DOAnchorPos(m_pos, m_diff);
    }

}
