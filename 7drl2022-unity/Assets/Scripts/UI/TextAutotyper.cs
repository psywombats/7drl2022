using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TextAutotyper : MonoBehaviour, IInputListener {

    [SerializeField] public Text textbox;
    [SerializeField] public float charsPerSecond = 120f;
    [SerializeField] protected GameObject advanceArrow;
    [SerializeField] protected bool speedUpWhenHurried;

    public int LinesTyped { get; private set; } = 0;

    protected int typingStartIndex = 0;
    protected bool hurried;
    protected bool confirmed;

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        switch (eventType) {
            case InputManager.Event.Hold:
                if (command == InputManager.Command.Confirm) {
                    hurried = true;
                }
                break;
            case InputManager.Event.Down:
                if (command == InputManager.Command.Confirm) {
                    confirmed = true;
                }
                break;
        }
        return true;
    }

    public IEnumerator AutotypeRoutine() {
        return TypeRoutine(textbox.text, false);
    }

    public IEnumerator TypeRoutine(string text, bool waitForConfirm = true, float fixedDuration = 0f) {
        hurried = false;
        confirmed = false;
        float elapsed = 0.0f;
        float total;
        if (fixedDuration > 0) {
            total = fixedDuration;
        } else {
            total = (text.Length - typingStartIndex) / charsPerSecond;
        }
        textbox.GetComponent<CanvasGroup>().alpha = 1.0f;
        while (elapsed <= total && textbox != null) {
            elapsed += Time.deltaTime;
            int charsToShow = Mathf.FloorToInt(elapsed * charsPerSecond) + typingStartIndex;
            int cutoff = charsToShow > text.Length ? text.Length : charsToShow;
            textbox.text = text.Substring(0, cutoff);
            textbox.text += "<color=#aa000000>";
            textbox.text += text.Substring(cutoff);
            textbox.text += "</color>";
            yield return null;

            elapsed += Time.deltaTime * Global.Instance().SystemData.SettingTextSpeed.Value;
            if (hurried) {
                hurried = false;
                if (speedUpWhenHurried) {
                    elapsed += Time.deltaTime * 4;
                }
            }
            if (confirmed) {
                confirmed = false;
                if (!speedUpWhenHurried && fixedDuration == 0f) {
                    break;
                }
            }
        }
        textbox.text = text;

        if (waitForConfirm) {
            confirmed = false;
            if (advanceArrow != null) advanceArrow.SetActive(true);
            while (!confirmed) {
                yield return null;
            }
            if (advanceArrow != null) advanceArrow.SetActive(false);
        }
    }
}
