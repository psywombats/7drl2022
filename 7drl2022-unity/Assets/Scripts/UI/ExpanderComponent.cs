using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class ExpanderComponent : MonoBehaviour {

    [SerializeField] private GameObject contents = null;

    [SerializeField] private int stepSize = 16;
    [SerializeField] private float duration = 0.2f;

    private RectTransform rectTransform;
    public RectTransform RectTransform {
        get {
            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    public Vector2 minSize, maxSize;

    public void Show() {
        RectTransform.sizeDelta = maxSize;
        gameObject.SetActive(true);
        if (contents != null) {
            contents.SetActive(true);
        }
    }

    public void Hide() {
        RectTransform.sizeDelta = minSize;
        if (contents != null) {
            contents.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public IEnumerator ShowRoutine() {
        RectTransform.sizeDelta = minSize;
        if (contents != null) {
            contents.SetActive(false);
        }
        gameObject.SetActive(true);
        yield return CoUtils.StepResize2(
            x => RectTransform.sizeDelta = x, 
            RectTransform.sizeDelta, 
            maxSize, 
            stepSize, duration);
        if (contents != null) {
            contents.SetActive(true);
        }
    }

    public IEnumerator HideRoutine() {
        RectTransform.sizeDelta = maxSize;
        if (contents != null) {
            contents.SetActive(false);
        }
        yield return CoUtils.StepResize2(
            x => RectTransform.sizeDelta = x,
            RectTransform.sizeDelta,
            minSize,
            stepSize, duration);
        Hide();
    }
}
