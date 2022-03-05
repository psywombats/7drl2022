using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FontSwapperComponent : MonoBehaviour {

    private Text text;
    private Text Text {
        get {
            if (text == null) {
                text = GetComponent<Text>();
            }
            return text;
        }
    }

    public void OnValidate() {
        UpdateText();
    }

    public void OnEnable() {
        UpdateText();
    }

    private void UpdateText() {
        Text.font = Settings.Instance().systemFont.font;
        Text.fontSize = Settings.Instance().systemFont.size;
        Text.material = Settings.Instance().systemFont.material;
        Text.color = Settings.Instance().systemFont.color;
    }
}
