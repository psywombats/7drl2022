using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions {

    public static T SelectRandom<T>(this List<T> list) {
        var index = Random.Range(0, list.Count - 1);
        return list[index];
    }
}
