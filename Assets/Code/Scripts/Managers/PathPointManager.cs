using System.Collections.Generic;
using UnityEngine;

public class PathPointManager : MonoBehaviour
{

    public List<_PrePositions> prePositions;


    public bool m_randomized;
    public int StraightNo;

    [Space]
    public AnimationCurve LineCurve;

    public List<Vector3> _GetMyPath()
    {

        List<Vector3> v = new List<Vector3>();

        int m_count = WayPoints.Count;
        int m_random_no = Random.Range(0, 5);


        foreach (Transform child in PathParents[StraightNo].transform)
        {
            Debug.Log(child.gameObject.name);
            v.Add(child.position);
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
    public List<GameObject> Lines;
    public List<Transform> Transforms;
    public List<_WayPoints> WayPoints;
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

    public void _GenratePathPoints()
    {
        WayPoints = new List<_WayPoints>();

        _WayPoints W = new _WayPoints();

        int a = 0;
        int b = 0;
        foreach (var item in Transforms)
        {
            item.name = a.ToString();

            W = new _WayPoints();
            W.m_points = new List<Transform>();


            foreach (Transform i in item.transform)
            {
                Debug.Log(b + "  " + i.name);

                W.m_points.Add(i);
            }
            WayPoints.Add(W);

            b = 0;
            foreach (var e in W.m_points)
            {
                e.SetParent(PathParents[b]);
                b++;
            }

            a++;
        }


    }

#endif

}

[System.Serializable]
public class _PrePositions
{
    public List<Vector3> m_positions;
}
