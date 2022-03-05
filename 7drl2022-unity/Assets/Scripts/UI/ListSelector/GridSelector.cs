using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridSelector : GenericSelector {

    [SerializeField] protected GridLayoutGroup grid = null;
    [SerializeField] protected int colCount = 2;

    protected int Col => Selection % colCount;
    protected int Row => Selection / colCount;

    protected override int CellCount() {
        return grid.transform.childCount;
    }

    public override SelectableCell GetCell(int index) {
        return grid.transform.GetChild(index).GetComponent<SelectableCell>();
    }

    protected override IEnumerable<SelectableCell> GetCells() {
        for (int i = 0; i < transform.childCount; i += 1) {
            yield return GetCell(i);
        }
    }

    protected override void MoveSelectionHorizontal(int delta) {
        Global.Instance().Audio.PlaySFX("cursor");
        int col = Col;
        int row = Row;
        col += delta;
        if (col < 0) col = colCount - 1;
        if (col >= colCount) col = 0;
        Selection = CellIndexAt(row, col);
    }

    protected override void MoveSelectionVertical(int delta) {
        Global.Instance().Audio.PlaySFX("cursor");
        int col = Col;
        int row = Row;
        row += delta;
        if (row < 0) {
            row = CellCount() / colCount;
            while (CellIndexAt(row, col) >= CellCount()) {
                row -= 1;
            }
        } else if (CellIndexAt(row, col) >= CellCount()) {
            row = 0;
        }
        Selection = CellIndexAt(row, col);
    }

    protected virtual int CellIndexAt(int row, int col) {
        return row * colCount + col;
    }
}
