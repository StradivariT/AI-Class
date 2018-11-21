using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

// Best-first search
// Priority 1 - Choose the square that will grant point
// Priority 2 - Don't choose the square that will give a way a point/s
//   Check if neighbour will give away a point if a line is put there
// Priority 3 - No other option to choose, would give away point/s
//   TODO - Give away as little points as possible
public class AI : MonoBehaviour {
    [SerializeField]
    private float waitTime;

    [SerializeField]
    private GameObject lineHighlightPrefab;

    [SerializeField]
    private GameObject lineSelectionPrefab;

    private GameMaster gameMaster;

    private bool isThinking;

    private const string bestIndex = "best";
    private const string goodIndex = "good";
    private const string badIndex = "bad";

	private void Start () {
        this.gameMaster = (GameMaster)FindObjectOfType(typeof(GameMaster));
	}
	
	private void Update () {
        if (this.gameMaster.IsPlayerTurn || this.isThinking)
            return;

        this.isThinking = true;
        this.DetermineSelection();
	}

    private void DetermineSelection() {
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
        if(cellsByPriority[AI.bestIndex].Count > 0)
            selectedKey = AI.bestIndex;
        else if(cellsByPriority[AI.goodIndex].Count > 0)
            selectedKey = AI.goodIndex;
        else
            selectedKey = AI.badIndex;

        int selectionIndex = (int)Random.Range(0, cellsByPriority[selectedKey].Count);
        Cell selection = cellsByPriority[selectedKey][selectionIndex].Cell;

        int sideIndex = (int)Random.Range(0, cellsByPriority[selectedKey][selectionIndex].Sides.Count);
        CellSide side = cellsByPriority[selectedKey][selectionIndex].Sides[sideIndex];

        selection.HighlightSide(this.lineHighlightPrefab, selection.GetLineTransform(side));
        StartCoroutine(MakeSelection(selection, side));
    }

    IEnumerator MakeSelection(Cell cell, CellSide side) {
        yield return new WaitForSeconds(this.waitTime);

        cell.DestroyHighlight();
        cell.SelectSide(this.lineSelectionPrefab, side);

        this.isThinking = false;
    }
}
