using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour {

    public List<Sprite> frames;
    public float frameDuration = 0.5f;

    private float elapsed;

    public void Update() {
        elapsed += Time.deltaTime;
        UpdateSprite();
    }

    public void OnEnable() {
        UpdateSprite();
    }

    public void ResetFrame() {
        elapsed = 0.0f;
        UpdateSprite();
    }

    private void UpdateSprite() {
        if (frames != null) {
            float frameFloat = elapsed * frames.Count / frameDuration;
            int frame = ((int)Mathf.Floor(frameFloat)) % frames.Count;
            GetComponent<SpriteRenderer>().sprite = frames[frame];
        }
    }
}
