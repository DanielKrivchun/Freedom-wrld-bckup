using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointManager : MonoBehaviour
{

    public List<Transform> m_target_pos;

    public List<Vector3> _GetMyPath()
    {

        List<Vector3> v= new List<Vector3>();

        foreach (Transform t in m_target_pos)
        {
            v.Add(t.position);
        }

        return v;
    }


}
