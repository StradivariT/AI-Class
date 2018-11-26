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

// Insane will do double cross strategy
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

    private List<ChainCell> chainCells;
    private bool isClosingInChain;

    private bool isThinking;

    private const string bestIndex = "best";
    private const string goodIndex = "good";
    private const string badIndex = "bad";

	private void Awake() {
        this.gameMaster = (GameMaster)FindObjectOfType(typeof(GameMaster));
	}

    private void Start() {
        this.isClosingInChain = false;
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

            if(this.difficulty == Difficulty.Insane && cellsByPriority[AI.goodIndex].Count == 0) {
                List<AICell> board = new List<AICell>();
                board.AddRange(cellsByPriority[AI.bestIndex]);
                board.AddRange(cellsByPriority[AI.badIndex]);

                this.FilterBadCells(board);

                AICell crossCell = this.ChainDoubleCross();
                if(crossCell.Cell != null) {
                    cellsByPriority[AI.bestIndex] = new List<AICell> { crossCell };
                }
            }
        } else if(cellsByPriority[AI.goodIndex].Count > 0) {
            selectedKey = AI.goodIndex;
        } else {
            selectedKey = AI.badIndex;

            if (this.difficulty == Difficulty.Hard || this.difficulty == Difficulty.Insane) {
                cellsByPriority[AI.badIndex] = new List<AICell> { this.FilterBadCells(cellsByPriority[AI.badIndex]) };
            }

            if (this.difficulty == Difficulty.Insane) {
                cellsByPriority[AI.badIndex] = new List<AICell> { this.DoubleCross() };
            }
        }

        int selectionIndex = (int)Random.Range(0, cellsByPriority[selectedKey].Count);
        Cell selection = cellsByPriority[selectedKey][selectionIndex].Cell;

        int sideIndex = (int)Random.Range(0, cellsByPriority[selectedKey][selectionIndex].Sides.Count);
        CellSide side = cellsByPriority[selectedKey][selectionIndex].Sides[sideIndex];

        selection.HighlightSide(this.lineHighlightPrefab, selection.GetLineTransform(side));
        StartCoroutine(MakeSelection(selection, side));
    }

    private AICell ChainDoubleCross() {
        int chainCount = 0;
        int chainIndex = 0;
        for (int i = 0; i < this.chainCells.Count; i++) {
            if (this.chainCells[i].length < 3 && !this.isClosingInChain)
                return new AICell(null, null);

            if (this.chainCells[i].length < 3 && this.isClosingInChain)
                chainIndex = i;

            if(this.chainCells[i].length > 2)
                chainCount++;
        }

        if (chainCount < 2) {
            if (!this.isClosingInChain)
                return new AICell(null, null);
        } else {
            if(!this.isClosingInChain) {
                this.isClosingInChain = true;
                return new AICell(null, null);
            }

            if (this.isClosingInChain)
                return new AICell(null, null);
        }

        this.isClosingInChain = false;

        AICell chainCell = this.chainCells[chainIndex].AICell;
        Cell neighbour = null;
        CellSide opposite = CellSide.Left;
        if(chainCell.Sides.Count == 1) {
            switch(chainCell.Sides[0]) {
                case CellSide.Left:
                    neighbour = gameMaster.Cells[chainCell.Cell.Row][chainCell.Cell.Column - 1];
                    opposite = CellSide.Right;
                    break;

                case CellSide.Right:
                    neighbour = gameMaster.Cells[chainCell.Cell.Row][chainCell.Cell.Column + 1];
                    opposite = CellSide.Left;
                    break;

                case CellSide.Bottom:
                    neighbour = gameMaster.Cells[chainCell.Cell.Row + 1][chainCell.Cell.Column];
                    opposite = CellSide.Top;
                    break;

                case CellSide.Top:
                    neighbour = gameMaster.Cells[chainCell.Cell.Row - 1][chainCell.Cell.Column];
                    opposite = CellSide.Bottom;
                    break;
            }

            List<CellSide> neighbourFreeSides = neighbour.GetFreeSides();
            if (neighbourFreeSides.Count == 1)
                return new AICell(null, null);

            neighbourFreeSides.Remove(opposite);
            return new AICell(neighbour, neighbourFreeSides);
        }

        List<CellSide> resultSides = chainCell.Sides;
        switch(chainCell.Sides[0]) {
            case CellSide.Left:
                if (chainCell.Cell.Column == 0)
                    resultSides = new List<CellSide> { CellSide.Left };
                else
                    resultSides.Remove(CellSide.Left);
                break;

            case CellSide.Right:
                if (chainCell.Cell.Column == gameMaster.Cells.Count - 1)
                    resultSides = new List<CellSide> { CellSide.Right };
                else
                    resultSides.Remove(CellSide.Right);
                break;

            case CellSide.Bottom:
                if (chainCell.Cell.Row == gameMaster.Cells.Count - 1)
                    resultSides = new List<CellSide> { CellSide.Bottom };
                else
                    resultSides.Remove(CellSide.Bottom);
                break;

            case CellSide.Top:
                if (chainCell.Cell.Row == 0)
                    resultSides = new List<CellSide> { CellSide.Top };
                else
                    resultSides.Remove(CellSide.Top);
                break;
        }

        return new AICell(chainCell.Cell, resultSides);
    }

    private AICell DoubleCross() {
        bool HasDoubleCross = false;
        int priorityCellIndex = 0;
        for (int i = 0; i < this.chainCells.Count; i++) {
            if (this.chainCells[i].length == 1)
                return chainCells[i].AICell;

            if (this.chainCells[i].length == 2) {
                HasDoubleCross = true;
                priorityCellIndex = i;
            }

            if(!HasDoubleCross)
                priorityCellIndex = i;
        }

        if (!HasDoubleCross)
            return this.chainCells[priorityCellIndex].AICell;

        AICell doubleCell = this.chainCells[priorityCellIndex].AICell;

        if(doubleCell.Cell.Row == 0)
            doubleCell.Sides.Remove(CellSide.Top);

        if (doubleCell.Cell.Row == gameMaster.Cells.Count - 1)
            doubleCell.Sides.Remove(CellSide.Bottom);

        if (doubleCell.Cell.Column == 0)
            doubleCell.Sides.Remove(CellSide.Left);

        if (doubleCell.Cell.Column == gameMaster.Cells.Count - 1)
            doubleCell.Sides.Remove(CellSide.Right);

        return doubleCell;
    }

    private AICell FilterBadCells(List<AICell> aiCells) {
        ShortestRegion shortestRegion = new ShortestRegion();
        shortestRegion.Length = 10000;

        List<Cell> visitedCells = new List<Cell>();
        this.chainCells = new List<ChainCell>();

        foreach(AICell aiCell in aiCells) {
            if (visitedCells.Contains(aiCell.Cell))
                continue;
            
            int regionLength = this.RegionCount(aiCell.Cell, aiCell.Sides, visitedCells);
            this.chainCells.Add(new ChainCell(aiCell, regionLength));

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
