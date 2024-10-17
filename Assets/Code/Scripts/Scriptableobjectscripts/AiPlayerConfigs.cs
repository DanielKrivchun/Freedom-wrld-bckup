using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AiPlayerConfigs", menuName = "ScriptableObject/AiPlayerConfigs", order = 100)]

public class AiPlayerConfigs : ScriptableObject
{
    public List<_AiPlayerConfigurations> Configs;


    private float maxSpeed = 0f;
    private float minSpeed = 0f;
    public float _GetMySpeed()
    {
        int a = Random.Range(0, Configs.Count);
        return Configs[a].m_speed;
    }

    //public float _GetSPeedBasedOnPlayerSpeed(float _player_speed)
    //{
    //    float min = _player_speed - Random.Range(0.5f, 3f);
    //    float max = _player_speed + Random.Range(0f, 2f);
    //    return Random.Range(min, max);
    //}

    public void _SetSpeedValues(float _player_speed)
    {
        Debug.Log(_player_speed);
        maxSpeed = _player_speed + 2f;
        minSpeed = _player_speed - 1.5f;

        if (minSpeed < 1f)
        {
            minSpeed = 1f;
        }

        float value = 0f;

        for (int i = 0; i < Configs.Count; i++)
        {
            value = Random.Range(minSpeed, maxSpeed);
            Configs[i].m_speed = value;
        }


    }
}

[System.Serializable]
public class _AiPlayerConfigurations
{
    public float m_speed;
}
