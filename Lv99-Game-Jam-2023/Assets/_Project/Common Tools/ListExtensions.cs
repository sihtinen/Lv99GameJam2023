using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public static class ListExtensions
{
    public static T GetRandomElement<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("ListExtensions.GetRandomElement: list is null or empty");
            return default;
        }

        Random.InitState(DateTime.Now.Millisecond);
        return list[Random.Range(0, list.Count)];
    }

    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        T _itemA = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = _itemA;
    }
}
