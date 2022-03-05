using System.Collections.Generic;

public class ListSelector : GenericSelector {

    public List<SelectableCell> cells;

    public override SelectableCell GetCell(int index) {
        return cells[index];
    }

    protected override IEnumerable<SelectableCell> GetCells() {
        return cells;
    }

    protected override int CellCount() {
        return cells.Count;
    }
}
