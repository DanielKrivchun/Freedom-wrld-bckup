using System.Collections.Generic;
using UnityEngine;

public class PathPointManager : MonoBehaviour
{

    public List<_PrePositions> prePositions;


    public bool m_randomized;
    public int StraightNo;

    [Space]
    public AnimationCurve LineCurve;

#if UNITY_EDITOR
    public List<Vector3> _GetMyPath()
    {
        List<Vector3> v = new List<Vector3>();


        int a = 0;
        foreach (Transform child in PathParents[StraightNo].transform)
        {
            child.name = a.ToString();
            Debug.Log(child.gameObject.name);
            v.Add(child.position);
            a++;
        }

        _PrePositions pre = new _PrePositions();
        prePositions[StraightNo].m_positions = new List<Vector3>();
        pre.m_positions = v;
        prePositions[StraightNo].m_positions = v;

        _GenratePathLines(v);
        return v;
    }

#endif

#if UNITY_EDITOR
    public List<GameObject> Lines;
    [Space]
    public List<Transform> PathParents;

    public void _GenratePathLines()
    {
        List<Vector3> v = _GetMyPath();
        float m_d = 0;
    }

    public void _GenratePathLines(List<Vector3> m_v)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<LineRenderer>();
        obj.GetComponent<LineRenderer>().widthCurve = LineCurve;
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        Lines.Add(obj);
        LineRenderer m_line = obj.GetComponent<LineRenderer>();
        m_line.positionCount = m_v.Count;
        m_line.SetPositions(m_v.ToArray());

    }

    public void _Reset()
    {
        foreach (var item in Lines)
        {
            if (item != null)
            {
                DestroyImmediate(item);
            }
        }

        Lines.Clear();
    }

#endif

}

[System.Serializable]
public class _PrePositions
{
    public List<Vector3> m_positions;
}
