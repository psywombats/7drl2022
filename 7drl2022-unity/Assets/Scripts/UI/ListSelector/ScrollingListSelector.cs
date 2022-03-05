using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class ScrollingListSelector<T> : DynamicListSelector {
    
    [SerializeField] private GameObject upArrow = null;
    [SerializeField] private GameObject downArrow = null;

    protected List<T> data;
    protected Action<GameObject, T> populater;
    public int Offset { get; set; } = 0;

    private int maxCellCount = -1;
    protected int MaxCellCount {
        get {
            if (maxCellCount == -1) {
                maxCellCount = CellCount();
            }
            return maxCellCount;
        }
    }


    public override int Selection {
        set {
            if (CellCount() == 0) {
                Global.Instance().Debug.LogError("No selection possible");
            }
            if (selection >= 0 && selection < CellCount()) {
                GetCell(selection).SetSelected(false);
            }
            selection = value;
            if (selection < 0) {
                if (Offset > 0) {
                    Offset -= 1;
                    selection = 0;
                } else {
                    selection = CellCount() - 1;
                    Offset = Math.Max(0, data.Count - CellCount());
                }
                Repopulate();
            }
            if (selection == CellCount()) {
                if (Offset + selection < data.Count) {
                    Offset += 1;
                    selection -= 1;
                } else {
                    Offset = 0;
                    selection = 0;
                }
                Repopulate();
            }
            if (GetCell(selection).IsSelectable()) {
                GetCell(selection).SetSelected(true);
                FireSelectionChange();
            } else {
                Selection -= 1;
            }

        }
    }

    public override async Task<int> SelectItemAsync(Action<int> scanner = null, bool leavePointerEnabled = false) {
        var orig = await base.SelectItemAsync(scanner, leavePointerEnabled);
        if (orig < 0) return orig;
        else return orig + Offset;
    }

    public void SetAbsoluteSelection(int index) {
        selection = 1;
        Offset = index - 1;
        if (Offset < 0) {
            selection -= Offset;
            Offset -= Offset;
        }
        if (Offset > data.Count - CellCount()) {
            var off = data.Count - CellCount() - Offset;
            Offset += off;
            selection += off;
        }
        Repopulate();
        FireSelectionChange();
    }

    public void Populate(IEnumerable<T> data, Action<GameObject, T> populater) {
        this.data = new List<T>(data);
        this.populater = populater;
        Repopulate();
    }

    public T SelectedData() {
        return data[Offset + Selection];
    }

    private void Repopulate() {
        List.Populate(data.GetRange(Offset, Math.Min(MaxCellCount, data.Count - Offset)), populater);
        upArrow.SetActive(Offset > 0);
        downArrow.SetActive(Offset + CellCount() < data.Count);
    }

    protected override void InvokeScanner(Action<int> scanner) {
        scanner?.Invoke(Selection + Offset);
    }
}
