using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

// Best-first search
// Priority 1 - Choose the square that will grant point
// Priority 2 - Don't choose the square that will give a way a point/s
// Priority 3 - No other option to choose, would give away point/s
public class AI : MonoBehaviour {
    [SerializeField]
    private float waitTime;

    [SerializeField]
    private GameObject lineHighlight;

    [SerializeField]
    private GameObject lineSelection;

    private bool isThinking;

    private List<Cell> bestCells;
    private List<GoodCell> goodCells;
    private List<Cell> badCells;

	void Start () {
        this.bestCells = new List<Cell>();
        this.goodCells = new List<GoodCell>();
        this.badCells = new List<Cell>();
	}
	
	void Update () {
        if (GameMaster.isPlayerTurn || this.isThinking)
            return;

        this.isThinking = true;
        this.EvaluateCells();
	}

    private void EvaluateCells() {
        this.bestCells = new List<Cell>();
        this.goodCells = new List<GoodCell>();
        this.badCells = new List<Cell>();

        for (int i = 0; i < GameMaster.cells.GetLength(0); i++) {
            for (int j = 0; j < GameMaster.cells.GetLength(1); j++) {
                if (GameMaster.cells[i, j].IsClosed())
                    continue;

                List<CellSide> freeSides = GameMaster.cells[i, j].GetFreeSides();

                switch(freeSides.Count) {
                    case 1:
                        this.bestCells.Add(GameMaster.cells[i, j]);
                        break;

                    case 3:
                    case 4:
                        List<CellSide> validNeighbourSides = GameMaster.cells[i, j].GetValidNeighbourSides();
                        if(validNeighbourSides.Count == 0) {
                            this.badCells.Add(GameMaster.cells[i, j]);
                            break;
                        }

                        this.goodCells.Add(new GoodCell(GameMaster.cells[i, j], validNeighbourSides));
                        break;

                    case 2:
                        this.badCells.Add(GameMaster.cells[i, j]);
                        break;
                }
            }
        }

        Cell selected;
        CellSide side;
        if(this.bestCells.Count > 0) {
            selected = this.bestCells[(int)Random.Range(0, this.bestCells.Count)];
            side = selected.GetFreeSides()[(int)Random.Range(0, selected.GetFreeSides().Count)];
        } else if (this.goodCells.Count > 0) {
            int selectedIndex = (int)Random.Range(0, this.goodCells.Count);

            selected = this.goodCells[selectedIndex].cell;
            side = this.goodCells[selectedIndex].validSides[(int)Random.Range(0, this.goodCells[selectedIndex].validSides.Count)];
        } else {
            selected = this.badCells[(int)Random.Range(0, this.badCells.Count)];
            side = selected.GetFreeSides()[(int)Random.Range(0, selected.GetFreeSides().Count)];
        }

        LineTransform transf = selected.GetLineTransform(side);

        selected.HighlightSide(this.lineHighlight, transf);

        StartCoroutine(MakeMove(selected, side));
    }

    IEnumerator MakeMove(Cell selected, CellSide side) {
        yield return new WaitForSeconds(this.waitTime);

        selected.DestroyHighlight();
        selected.SelectSide(this.lineSelection, side);
        this.isThinking = false;
    }
}
