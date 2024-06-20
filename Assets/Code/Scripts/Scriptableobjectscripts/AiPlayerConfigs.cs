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

}

[System.Serializable]
public class _AiPlayerConfigurations
{
    public float m_speed;
}
