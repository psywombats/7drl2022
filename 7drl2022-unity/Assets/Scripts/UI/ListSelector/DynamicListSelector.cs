using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ListView))]
public class DynamicListSelector : GenericSelector {

    private ListView list;
    protected ListView List {
        get {
            if (list == null) {
                list = GetComponent<ListView>();
            }
            return list;
        }
    }

    protected override int CellCount() {
        return List.transform.childCount;
    }

    public override SelectableCell GetCell(int index) {
        return List.transform.GetChild(index).GetComponent<SelectableCell>();
    }

    public void ClearCells() {
        foreach (var cell in GetCells()) {
            DestroyImmediate(cell.gameObject);
        }
    }

    protected override IEnumerable<SelectableCell> GetCells() {
        foreach (RectTransform childTransform in List.transform) {
            yield return childTransform.GetComponent<SelectableCell>();
        }
    }
}
