using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Bloated textbox ripped almost directly from Snowbound VN
/// </summary>
public class Textbox : TextAutotyper {

    public static readonly string SystemSpeaker = "SYSTEM";
    private static float OverlapSlop = 5f;

    public float animationSeconds = 0.2f;
    public float textClearSeconds = 0.1f;

    public Text namebox;
    public RectTransform mainBox;

    public float minTextHeight = 12;
    [HideInInspector] [SerializeField] protected Vector2 textMaxSize;
    [HideInInspector] [SerializeField] protected float nameboxHeight;

    public bool isDisplaying { get; private set; }

    public void Start() {
        textbox.text = "";
        mainBox.sizeDelta = new Vector2(mainBox.sizeDelta.x, 0.0f);
        advanceArrow.SetActive(false);
    }

    public void MemorizeSizes() {
        textMaxSize = mainBox.sizeDelta;
        nameboxHeight = namebox.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void MarkHiding() {
        isDisplaying = false;
    }

    public IEnumerator TestRoutine() {
        isDisplaying = true;
        while (true) {
            yield return DisableRoutine();
            yield return CoUtils.Wait(1.0f);
            yield return SpeakRoutine("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
                "eiusmod tempor incididunt ut labore et dolore magna aliqua.");
            yield return SpeakRoutine("Diaghilev", "Or thus spake the ancestors.");
            yield return SpeakRoutine("Diaghilev", "Etc.");
            yield return SpeakRoutine("Homasa", "Hello I am someone completely different");
            yield return SpeakRoutine("eom");
        }
    }

    public IEnumerator SpeakRoutine(string text) {
        yield return SpeakRoutine(SystemSpeaker, text);
    }

    public IEnumerator SpeakRoutine(string speakerName, string text, float fixedDuration = 0f) {
        if (text == null || text.Length == 0) {
            text = speakerName;
            speakerName = SystemSpeaker;
        }
        if (speakerName == SystemSpeaker) {
            SetNameboxEnabled(false);
        }
        if (!isDisplaying) {
            if (speakerName == SystemSpeaker) {
                SetNameboxEnabled(false);
            }
            namebox.text = speakerName;
            yield return EnableRoutine();
            if (speakerName != SystemSpeaker) {
                SetNameboxEnabled(true);
            }
        } else {
            yield return EraseTextRoutine(textClearSeconds);
            if (namebox.text != speakerName && speakerName != SystemSpeaker) {
                yield return NameboxSpeakerSwitchStartRoutine(animationSeconds);
                SetNameboxEnabled(speakerName != SystemSpeaker);
                namebox.text = speakerName;
                yield return NameboxSpeakerSwitchEndRoutine(animationSeconds);
            }
        }
        if (speakerName == SystemSpeaker) {
            namebox.text = "";
        }

        yield return TypeRoutine(text, fixedDuration: fixedDuration);
    }

    public virtual void Hide() {
        mainBox.anchoredPosition = new Vector3(mainBox.anchoredPosition.x, -minTextHeight * 2 + OverlapSlop);
        namebox.rectTransform.sizeDelta = new Vector2(mainBox.sizeDelta.x, 0.0f);
        mainBox.sizeDelta = new Vector2(mainBox.sizeDelta.x, minTextHeight);
    }

    public virtual void Show() {
        mainBox.anchoredPosition = new Vector3(mainBox.anchoredPosition.x, 0);
        namebox.rectTransform.sizeDelta = new Vector2(mainBox.sizeDelta.x, nameboxHeight);
        mainBox.sizeDelta = new Vector2(textMaxSize.x, textMaxSize.y);
    }

    private IEnumerator EnableRoutine() {
        isDisplaying = true;
        Global.Instance().Input.PushListener(this);

        yield return CoUtils.RunParallel(new IEnumerator[] {
            IsNameboxEnabled() ? ShowNameBoxRoutine(animationSeconds) : CoUtils.Wait(0.0f),
            ShowMainBoxRoutine(animationSeconds),
        }, this);
    }
    public IEnumerator DisableRoutine() {
        isDisplaying = false;
        yield return CoUtils.RunParallel(new IEnumerator[] {
            EraseTextRoutine(animationSeconds / 2.0f),
            HideNameBoxRoutine(animationSeconds),
            CloseMainBoxRoutine(animationSeconds),
        }, this);
        Global.Instance().Input.RemoveListener(this);
    }

    protected virtual IEnumerator ShowNameBoxRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.GetComponent<RectTransform>().DOSizeDelta(new Vector2(mainBox.sizeDelta.x, nameboxHeight), seconds));
    }
    protected virtual IEnumerator HideNameBoxRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.rectTransform.DOSizeDelta(new Vector2(namebox.rectTransform.sizeDelta.x, 0.0f), seconds));
    }

    protected virtual IEnumerator EraseTextRoutine(float seconds) {
        yield return CoUtils.RunTween(textbox.GetComponent<CanvasGroup>().DOFade(0.0f, seconds));
    }
    protected virtual IEnumerator EraseNameRoutine(float seconds) {
        yield return CoUtils.RunTween(namebox.GetComponent<CanvasGroup>().DOFade(0.0f, seconds));
    }

    protected virtual IEnumerator ShowMainBoxRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
            CoUtils.RunTween(mainBox.DOSizeDelta(new Vector2(mainBox.sizeDelta.x, textMaxSize.y), seconds)),
            CoUtils.RunTween(mainBox.DOLocalMoveY(0.0f, seconds, true)),
        }, this);
    }
    protected virtual IEnumerator CloseMainBoxRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
            CoUtils.RunTween(mainBox.DOSizeDelta(new Vector2(mainBox.sizeDelta.x, minTextHeight), seconds)),
            CoUtils.RunTween(mainBox.DOLocalMoveY(-minTextHeight * 2 + OverlapSlop, seconds, true)),
        }, this);
    }

    protected virtual IEnumerator NameboxSpeakerSwitchStartRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
                    CloseMainBoxRoutine(seconds),
                    HideNameBoxRoutine(seconds),
                }, this);
    }
    protected virtual IEnumerator NameboxSpeakerSwitchEndRoutine(float seconds) {
        yield return CoUtils.RunParallel(new IEnumerator[] {
                    ShowMainBoxRoutine(animationSeconds),
                    ShowNameBoxRoutine(animationSeconds),
                }, this);
    }

    protected virtual void SetNameboxEnabled(bool enabled) {
        namebox.enabled = enabled;
    }
    protected virtual bool IsNameboxEnabled() {
        return namebox.enabled;
    }
}
