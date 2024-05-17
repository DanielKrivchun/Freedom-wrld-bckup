using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointManager : MonoBehaviour
{
    public List<_WayPoints> m_way_points;
    public bool m_randomized;

    public List<_PrePositions> prePositions;

    public List<Vector3> _GetMyPath()
    {

        List<Vector3> v = new List<Vector3>();

        int m_count = m_way_points.Count;
        int m_random_no = Random.Range(0, 5);

        for (int i = 0; i < m_count; i++)
        {
            if (m_randomized)
            {
                m_random_no = Random.Range(0, m_way_points[i].m_points.Length);
            }
            v.Add(m_way_points[i].m_points[m_random_no].position);
        }
#if UNITY_EDITOR

        _PrePositions pre = new _PrePositions();
        pre.m_positions = v;   
        
        prePositions.Add(pre);

        _GenratePathLines(v);
#endif

        return v;
    }

#if UNITY_EDITOR
    public List<GameObject> m_lines;


    public void _GenratePathLines()
    {
        List<Vector3> v = _GetMyPath();
        float m_d = 0;
    }

    public void _GenratePathLines(List<Vector3> m_v)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<LineRenderer>();
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        m_lines.Add(obj);
        LineRenderer m_line = obj.GetComponent<LineRenderer>();
        m_line.positionCount = m_v.Count;
        m_line.SetPositions(m_v.ToArray());

    }

    public void _Reset()
    {
        foreach (var item in m_lines)
        {
            if (item != null)
            {

                DestroyImmediate(item);
            }
        }

        m_lines.Clear();
    }

#endif

}

[System.Serializable]
public class _PrePositions
{
    public List<Vector3> m_positions;
}
