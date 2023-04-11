using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public static class NavmeshPathExtensions
{
    private static List<SerializableVector3> m_cachedSerializableVectorList = new List<SerializableVector3>();
    private static Vector3[] m_cachedCornerArray = new Vector3[1024];

    public static List<SerializableVector3> ToSerializableVectorList(this NavMeshPath path)
    {
        m_cachedSerializableVectorList.Clear();

        int _cornerCount = path.GetCornersNonAlloc(m_cachedCornerArray);

        for (int i = 0; i < _cornerCount; i++)
            m_cachedSerializableVectorList.Add(m_cachedCornerArray[i]);

        return m_cachedSerializableVectorList;
    }
}
