using System.Collections;
using UnityEngine;

public class PlayBGMComponent : MonoBehaviour {

    [SerializeField] private string bgmKey = null;

    public void Start() {
        StartCoroutine(Routine());
    }

    private IEnumerator Routine() {
        yield return null;
        Global.Instance.Audio.PlayBGM(bgmKey);
    }
}
