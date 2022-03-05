using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Has a payload to be evaluated/used when selected
/// </summary>
[RequireComponent(typeof(SelectableCell))]
public class CommandCell : MonoBehaviour {

    public string overrideCommand;

    public string CommandString => overrideCommand?.Length > 0 ? overrideCommand : GetComponent<Text>().text;

    private SelectableCell select;
    public SelectableCell Select {
        get {
            if (select == null) {
                select = GetComponent<SelectableCell>();
            }
            return select;
        }
    }
}
