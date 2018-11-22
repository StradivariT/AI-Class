using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

// Best-first search, production rules sort of thing?
// Priority 1 - Choose the square that will grant point

// Priority 2 - Don't choose the square that will give a way a point/s
//   Check if neighbour will give away a point if a line is put there

// Priority 3 - No other option to choose, would give away point/s
//   Give away as little points as possible

// TODO - Assert double cross
// TODO - If there are more than one "broken" chain of length 3 or more, leave two
public class AI : MonoBehaviour {
    [SerializeField]
    [Range(0, 5)]
    private float selectionWaitTime;

    [SerializeField]
    private GameObject lineHighlightPrefab;

    [SerializeField]
    private GameObject lineSelectionPrefab;

    [SerializeField]
    private Difficulty difficulty;

    private GameMaster gameMaster;

    private bool isThinking;

    private const string bestIndex = "best";
    private const string goodIndex = "good";
    private const string badIndex = "bad";

	private void Awake() {
        this.gameMaster = (GameMaster)FindObjectOfType(typeof(GameMaster));
	}
	
	private void Update() {
        if (this.gameMaster.IsPlayerTurn || this.isThinking || this.gameMaster.Cells == null)
            return;

        this.isThinking = true;
        StartCoroutine(this.DetermineSelection());
	}

    private IEnumerator DetermineSelection() {
        yield return new WaitForSeconds(0.1f);

        Dictionary<string, List<AICell>> cellsByPriority = new Dictionary<string, List<AICell>> {
            { AI.bestIndex, new List<AICell>() },
            { AI.goodIndex, new List<AICell>() },
            { AI.badIndex, new List<AICell>() }
        };

        for (int i = 0; i < this.gameMaster.Cells.Count; i++) {
            for (int j = 0; j < this.gameMaster.Cells.Count; j++) {
                if (this.gameMaster.Cells[i][j].IsClosed())
                    continue;
                
                List<CellSide> freeSides = this.gameMaster.Cells[i][j].GetFreeSides();
                switch(freeSides.Count) {
                    case 1:
                        cellsByPriority[AI.bestIndex].Add(new AICell(this.gameMaster.Cells[i][j], freeSides));
                        break;

                    case 3:
                    case 4:
                        if (this.difficulty == Difficulty.Easy) {
                            cellsByPriority[AI.goodIndex].Add(new AICell(this.gameMaster.Cells[i][j], freeSides));
                            break;
                        }

                        List<CellSide> validNeighbourSides = this.gameMaster.Cells[i][j].GetValidNeighbourSides();
                        if(validNeighbourSides.Count == 0) {
                            cellsByPriority[AI.badIndex].Add(new AICell(this.gameMaster.Cells[i][j], freeSides));
                            break;
                        }

                        cellsByPriority[AI.goodIndex].Add(new AICell(this.gameMaster.Cells[i][j], validNeighbourSides));
                        break;

                    case 2:
                        cellsByPriority[AI.badIndex].Add(new AICell(this.gameMaster.Cells[i][j], freeSides));
                        break;
                }
            }
        }

        string selectedKey;
        if(cellsByPriority[AI.bestIndex].Count > 0) {
            selectedKey = AI.bestIndex;
        } else if(cellsByPriority[AI.goodIndex].Count > 0) {
            selectedKey = AI.goodIndex;
        } else {
            selectedKey = AI.badIndex;

            if (this.difficulty == Difficulty.Hard) {
                cellsByPriority[AI.badIndex] = new List<AICell> { this.FilterBadCells(cellsByPriority[AI.badIndex]) };
            }

            //if (this.difficulty == Difficulty.Insane) {
            //    cellsByPriority[AI.badIndex] = new List<AICell> { this.GetBestMove() };
            //}
        }

        int selectionIndex = (int)Random.Range(0, cellsByPriority[selectedKey].Count);
        Cell selection = cellsByPriority[selectedKey][selectionIndex].Cell;

        int sideIndex = (int)Random.Range(0, cellsByPriority[selectedKey][selectionIndex].Sides.Count);
        CellSide side = cellsByPriority[selectedKey][selectionIndex].Sides[sideIndex];

        selection.HighlightSide(this.lineHighlightPrefab, selection.GetLineTransform(side));
        StartCoroutine(MakeSelection(selection, side));
    }

    //private AICell GetBestMove() {
        
    //}

    private AICell FilterBadCells(List<AICell> aiCells) {
        ShortestRegion shortestRegion = new ShortestRegion();
        shortestRegion.Length = 10000;

        List<Cell> visitedCells = new List<Cell>();

        foreach(AICell aiCell in aiCells) {
            if (visitedCells.Contains(aiCell.Cell))
                continue;

            int regionLength = this.RegionCount(aiCell.Cell, aiCell.Sides, visitedCells);

            if (regionLength < shortestRegion.Length) {
                shortestRegion.AICell = aiCell;
                shortestRegion.Length = regionLength;
            }
        }

        return shortestRegion.AICell;
    }

    private int RegionCount(Cell cell, List<CellSide> openSides, List<Cell> visitedCells) {
        int regionCount = 1;

        visitedCells.Add(cell);

        foreach(CellSide side in openSides) {
            Cell neighbour = null;
            List<CellSide> sides = null;

            switch(side) {
                case CellSide.Left:
                    if (cell.Column == 0)
                        break;

                    neighbour = this.gameMaster.Cells[cell.Row][cell.Column - 1];
                    sides = neighbour.GetFreeSides();
                    sides.Remove(CellSide.Right);

                    break;

                case CellSide.Right:
                    if (cell.Column == this.gameMaster.Cells.Count - 1)
                        break;

                    neighbour = this.gameMaster.Cells[cell.Row][cell.Column + 1];
                    sides = neighbour.GetFreeSides();
                    sides.Remove(CellSide.Left);

                    break;
                    
                case CellSide.Bottom:
                    if (cell.Row == this.gameMaster.Cells.Count - 1)
                        break;

                    neighbour = this.gameMaster.Cells[cell.Row + 1][cell.Column];
                    sides = neighbour.GetFreeSides();
                    sides.Remove(CellSide.Top);

                    break;
                    
                case CellSide.Top:
                    if (cell.Row == 0)
                        break;

                    neighbour = this.gameMaster.Cells[cell.Row - 1][cell.Column];
                    sides = neighbour.GetFreeSides();
                    sides.Remove(CellSide.Bottom);

                    break;
            }

            if (neighbour == null)
                continue;

            regionCount += this.RegionCount(neighbour, sides, visitedCells);
        }

        return regionCount;
    }

    IEnumerator MakeSelection(Cell cell, CellSide side) {
        yield return new WaitForSeconds(this.selectionWaitTime);

        cell.DestroyHighlight();
        cell.SelectSide(this.lineSelectionPrefab, side);

        this.isThinking = false;
    }
}
