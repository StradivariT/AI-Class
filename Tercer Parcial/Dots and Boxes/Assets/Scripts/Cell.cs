using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

public class Cell : MonoBehaviour {
    [SerializeField]
    private GameObject playerHighlightPrefab;

    [SerializeField]
    private GameObject playerSelectionPrefab;

    [SerializeField]
    private Sprite playerPoint;

    [SerializeField]
    private Sprite aiPoint;

    private GameMaster gameMaster;

    private Vector3 left;
    private Vector3 right;
    private Vector3 bottom;
    private Vector3 top;

    private Quaternion verticalRotation;

    private GameObject lineHighlight;

    private GameObject leftLine;
    private GameObject rightLine;
    private GameObject bottomLine;
    private GameObject topLine;

    public int Row { get; set; }
    public int Column { get; set; }

    private void Start() {
        this.gameMaster = (GameMaster)FindObjectOfType(typeof(GameMaster));

        this.left = new Vector3(this.transform.position.x - (this.gameMaster.CellSize / 2f), this.transform.position.y, 2f);
        this.right = this.left + (Vector3.right * this.gameMaster.CellSize);

        this.bottom = new Vector3(this.transform.position.x, this.transform.position.y - (this.gameMaster.CellSize / 2f), 2f);
        this.top = this.bottom + (Vector3.up * this.gameMaster.CellSize);

        this.verticalRotation = Quaternion.Euler(0, 0, 90);
    }

    private void OnMouseOver() {
        if (!this.gameMaster.IsPlayerTurn)
            return;

        CellSide side = this.GetMouseOverSide();
        if (this.IsSideOccupied(side)) {
            this.DestroyHighlight();
            return;
        }

        this.HighlightSide(this.playerHighlightPrefab, this.GetLineTransform(side));
    }

    public void HighlightSide(GameObject prefab, LineTransform line) {
        if (this.lineHighlight != null && this.lineHighlight.GetComponent<RectTransform>().position == line.Position) 
            return;

        Destroy(this.lineHighlight);

        this.lineHighlight = Instantiate(prefab, line.Position, line.Rotation);

        RectTransform lineHighlightRect = this.lineHighlight.GetComponent<RectTransform>();

        lineHighlightRect.SetParent(this.GetComponent<RectTransform>());
        lineHighlightRect.localScale = new Vector3(1, 0.333f);
    }

    private void OnMouseDown() {
        if (!this.gameMaster.IsPlayerTurn)
            return;

        CellSide side = this.GetMouseOverSide();
        if (this.IsSideOccupied(side))
            return;

        this.DestroyHighlight();

        this.SelectSide(this.playerSelectionPrefab, side);
    }

    public void SelectSide(GameObject prefab, CellSide side) {
        LineTransform line = this.GetLineTransform(side);

        GameObject selectionLine = Instantiate(prefab, line.Position, line.Rotation);

        RectTransform selectionLineRect = selectionLine.GetComponent<RectTransform>();
        selectionLineRect.SetParent(GetComponent<RectTransform>());
        selectionLineRect.localScale = new Vector3(1, 0.333f);

        Cell neighbourCell = null;
        switch(side) {
            case CellSide.Left:
                this.leftLine = selectionLine;
                if (this.Column > 0)
                    neighbourCell = this.gameMaster.Cells[this.Row][this.Column - 1];
                
                break;

            case CellSide.Right:
                this.rightLine = selectionLine;
                if (this.Column < this.gameMaster.Cells.Count - 1)
                    neighbourCell = this.gameMaster.Cells[this.Row][this.Column + 1];
                
                break;

            case CellSide.Bottom:
                this.bottomLine = selectionLine;
                if (this.Row < this.gameMaster.Cells.Count - 1)
                    neighbourCell = this.gameMaster.Cells[this.Row + 1][this.Column];

                break;

            default:
                this.topLine = selectionLine;
                if (this.Row > 0)
                    neighbourCell = this.gameMaster.Cells[this.Row - 1][this.Column];
                
                break;
        }

        bool isNeighboutClosed = false;
        if (neighbourCell != null)
            isNeighboutClosed = neighbourCell.IsUpdatedNeighbourClosed(side, selectionLine);

        if (this.IsClosed()) {
            this.SetOwner();
            return;
        }

        if (isNeighboutClosed)
            return;

        this.gameMaster.UpdateTurn();
    }

    private CellSide GetMouseOverSide() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

        float xDist = mousePosition.x - this.transform.position.x;
        float yDist = mousePosition.y - this.transform.position.y;

        float max = Mathf.Max(Mathf.Abs(xDist), Mathf.Abs(yDist));

        CellSide side;
        if (max == Mathf.Abs(xDist))
            if (xDist < 0)
                side = CellSide.Left;
            else
                side = CellSide.Right;
        else
            if (yDist < 0)
                side = CellSide.Bottom;
            else
                side = CellSide.Top;

        return side;
    }

    public LineTransform GetLineTransform(CellSide side) {
        LineTransform line = new LineTransform();

        switch (side) {
            case CellSide.Left:
                line.Position = this.left;
                line.Rotation = this.verticalRotation;
                break;

            case CellSide.Right:
                line.Position = this.right;
                line.Rotation = this.verticalRotation;
                break;

            case CellSide.Bottom:
                line.Position = this.bottom;
                line.Rotation = Quaternion.identity;
                break;

            default:
                line.Position = this.top;
                line.Rotation = Quaternion.identity;
                break;
        }

        return line;
    }

    private bool IsSideOccupied(CellSide side) {
        switch(side) {
            case CellSide.Left:
                if (this.leftLine != null)
                    return true;

                break;

            case CellSide.Right:
                if (this.rightLine != null)
                    return true;

                break;

            case CellSide.Bottom:
                if (this.bottomLine != null)
                    return true;

                break;

            case CellSide.Top:
                if (this.topLine != null)
                    return true;

                break;
        }

        return false;
    }

    public List<CellSide> GetFreeSides() {
        List<CellSide> sides = new List<CellSide>();

        if (this.leftLine == null)
            sides.Add(CellSide.Left);

        if (this.rightLine == null)
            sides.Add(CellSide.Right);

        if (this.bottomLine == null)
            sides.Add(CellSide.Bottom);

        if (this.topLine == null)
            sides.Add(CellSide.Top);

        return sides;
    }

    private void SetOwner() {
        Sprite point;

        if(this.gameMaster.IsPlayerTurn)
            point = this.playerPoint;
        else
            point = this.aiPoint;

        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<SpriteRenderer>().sprite = point;

        this.gameMaster.UpdateScore();
    }

    public bool IsUpdatedNeighbourClosed(CellSide side, GameObject line) {
        switch(side) {
            case CellSide.Left:
                this.rightLine = line;
                break;

            case CellSide.Right:
                this.leftLine = line;
                break;

            case CellSide.Bottom:
                this.topLine = line;
                break;

            default:
                this.bottomLine = line;
                break;
        }

        if(this.IsClosed()) {
            this.SetOwner();
            return true;
        }

        return false;
    }

    public List<CellSide> GetValidNeighbourSides() {
        List<CellSide> sides = new List<CellSide>();

        if (this.leftLine == null)
            if(this.Column == 0 || this.gameMaster.Cells[this.Row][this.Column - 1].GetFreeSides().Count > 2)
                sides.Add(CellSide.Left);

        if (this.rightLine == null)
            if (this.Column == this.gameMaster.Cells.Count - 1 || this.gameMaster.Cells[this.Row][this.Column + 1].GetFreeSides().Count > 2)
                sides.Add(CellSide.Right);

        if (this.bottomLine == null)
            if (this.Row == this.gameMaster.Cells.Count - 1 || this.gameMaster.Cells[this.Row + 1][this.Column].GetFreeSides().Count > 2)
                sides.Add(CellSide.Bottom);
        
        if (this.topLine == null)
            if (this.Row == 0 || this.gameMaster.Cells[this.Row - 1][this.Column].GetFreeSides().Count > 2)
                sides.Add(CellSide.Top);
        
        return sides;
    }

    private void OnMouseExit() {
        if (!this.gameMaster.IsPlayerTurn)
            return;

        this.DestroyHighlight();
    }

    public void DestroyHighlight() {
        if (this.lineHighlight != null)
            Destroy(this.lineHighlight);
    }

    public bool IsClosed() {
        return this.leftLine != null && this.rightLine != null && this.bottomLine != null && this.topLine != null;
    }
}