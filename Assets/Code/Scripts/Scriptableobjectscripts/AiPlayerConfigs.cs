using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AiPlayerConfigs", menuName = "ScriptableObject/AiPlayerConfigs", order = 100)]

public class AiPlayerConfigs : ScriptableObject
{
    public List<_AiPlayerConfigurations> Configs;

    public float _GetMySpeed()
    {
        int a = Random.Range(0, Configs.Count);
        return Configs[a].m_speed;
    }

    public float _GetSPeedBasedOnPlayerSpeed(float _player_speed)
    {
        float min = _player_speed - Random.Range(0f, 3f);
        float max = _player_speed - Random.Range(0f, 3f);
        return Random.Range(min, max);
    }

}

[System.Serializable]
public class _AiPlayerConfigurations
{
    public float m_speed;
}
