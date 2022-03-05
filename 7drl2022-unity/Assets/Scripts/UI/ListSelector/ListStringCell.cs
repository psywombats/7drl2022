using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListStringCell : SelectableCell {

    public Text text;

    public void Populate(string optionName) {
        text.text = optionName;
    }

    public override void SetSelectable(bool selectable) {
        base.SetSelectable(selectable);
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, selectable ? 1.0f : 0.5f);
    }
}
