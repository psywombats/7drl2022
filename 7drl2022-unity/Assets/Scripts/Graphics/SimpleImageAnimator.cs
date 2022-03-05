using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SimpleImageAnimator : MonoBehaviour {

    public List<Sprite> frames;
    public float frameDuration = 0.5f;

    public void Update() {
        UpdateSprite();
    }

    public void OnEnable() {
        UpdateSprite();
    }

    private void UpdateSprite() {
        if (frames != null) {
            float frameFloat = Time.time * frames.Count / frameDuration;
            int frame = ((int)Mathf.Floor(frameFloat)) % frames.Count;
            GetComponent<Image>().sprite = frames[frame];
        }
    }
}
