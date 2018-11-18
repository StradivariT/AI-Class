using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

public class Cell : MonoBehaviour {
    private Vector3 left;
    private Vector3 right;
    private Vector3 bottom;
    private Vector3 top;

    private Quaternion verticalRotation;

    private GameObject playerHighlight;

    private GameObject leftLine;
    private GameObject rightLine;
    private GameObject bottomLine;
    private GameObject topLine;

    [SerializeField]
    private GameObject playerHighlightPrefab;

    [SerializeField]
    private GameObject aiHighlightPrefab;

    [SerializeField]
    private GameObject playerLinePrefab;

    [SerializeField]
    private GameObject aiLinePrefab;

    [SerializeField]
    private Sprite playerPoint;

    [SerializeField]
    private Sprite aiPoint;

    public int row;
    public int column;

    void Start() {
        this.left = new Vector3(this.transform.position.x - (GameMaster.cellSize / 2f), this.transform.position.y, 2f);
        this.right = this.left + (Vector3.right * GameMaster.cellSize);

        this.bottom = new Vector3(this.transform.position.x, this.transform.position.y - (GameMaster.cellSize / 2f), 2f);
        this.top = this.bottom + (Vector3.up * GameMaster.cellSize);

        this.verticalRotation = Quaternion.Euler(0, 0, 90);
    }

    void OnMouseOver() {
        if (!GameMaster.isPlayerTurn)
            return;

        CellSide side = this.GetSide();
        if (this.IsSideOccupied(side)) {
            if (this.playerHighlight != null)
                Destroy(this.playerHighlight);

            return;
        }

        GameObject prefab;
        if(!GameMaster.isPlayerTurn) {
            prefab = this.aiHighlightPrefab;
            return;
        } else 
            prefab = this.playerHighlightPrefab;

        LineTransform line = this.GetLineTransform(side);

        this.HighlightSide(prefab, line);
    }

    void OnMouseDown() {
        if (!GameMaster.isPlayerTurn)
            return;

        CellSide side = this.GetSide();
        if (this.IsSideOccupied(side))
            return;

        if(this.playerHighlight != null) {
            Destroy(this.playerHighlight);
        }

        GameObject prefab;
        if (GameMaster.isPlayerTurn)
            prefab = this.playerLinePrefab;
        else {
            prefab = this.aiLinePrefab;
            return;
        }

        this.SelectSide(prefab, side);
    }

    void OnMouseExit() {
        if (!GameMaster.isPlayerTurn)
            return;

        if (this.playerHighlight != null) {
            Destroy(this.playerHighlight);
        }
    }

    private CellSide GetSide() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

        float xDist = mousePosition.x - this.transform.position.x;
        float yDist = mousePosition.y - this.transform.position.y;

        float max = Mathf.Max(Mathf.Abs(xDist), Mathf.Abs(yDist));

        CellSide side;
        if (max == Mathf.Abs(xDist)) {
            if (xDist < 0)
                side = CellSide.Left;
            else
                side = CellSide.Right;
        } else {
            if (yDist < 0)
                side = CellSide.Bottom;
            else
                side = CellSide.Top;
        }

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

    public bool IsClosed() {
        return this.leftLine != null && this.rightLine != null &&
                   this.bottomLine != null && this.topLine != null;
    }

    private void SetOwner(bool isPlayer) {
        Sprite point;
        if(isPlayer) {
            point = this.playerPoint;
        } else {
            point = this.aiPoint;
        }

        GameMaster.UpdateScore();

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().sprite = point;
    }

    public void HighlightSide(GameObject prefab, LineTransform line) {
        if (this.playerHighlight != null) {
            if (this.playerHighlight.transform.position == line.Position)
                return;

            Destroy(this.playerHighlight);
        }

        this.playerHighlight = Instantiate(prefab, line.Position, line.Rotation);
        this.playerHighlight.transform.parent = GetComponent<Transform>();
    }

    public void DestroyHighlight() {
        if (this.playerHighlight != null)
            Destroy(this.playerHighlight);
    }

    public void SelectSide(GameObject prefab, CellSide side) {
        LineTransform line = this.GetLineTransform(side);

        GameObject playerLine = Instantiate(prefab, line.Position, line.Rotation);
        playerLine.transform.parent = GetComponent<Transform>();

        bool isNeighbourClosed = false;
        switch(side) {
            case CellSide.Left:
                this.leftLine = playerLine;
                if (this.column > 0)
                    isNeighbourClosed = GameMaster.cells[this.row, this.column - 1].UpdateNeighbourSide(side, playerLine);
                break;

            case CellSide.Right:
                this.rightLine = playerLine;
                 if (this.column < GameMaster.cells.GetLength(0) - 1)
                    isNeighbourClosed = GameMaster.cells[this.row, this.column + 1].UpdateNeighbourSide(side, playerLine);
                break;

            case CellSide.Bottom:
                this.bottomLine = playerLine;
                if (this.row < GameMaster.cells.GetLength(1) - 1)
                    isNeighbourClosed = GameMaster.cells[this.row + 1, this.column].UpdateNeighbourSide(side, playerLine);
                break;

            default:
                this.topLine = playerLine;
                if (this.row > 0)
                    isNeighbourClosed = GameMaster.cells[this.row - 1, this.column].UpdateNeighbourSide(side, playerLine);
                break;
        }

        if (this.IsClosed()) {
            this.SetOwner(GameMaster.isPlayerTurn);
            return;
        }

        if (isNeighbourClosed) {
            return;     
        }

        GameMaster.isPlayerTurn = !GameMaster.isPlayerTurn;
    }

    public bool UpdateNeighbourSide(CellSide side, GameObject line) {
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
            this.SetOwner(GameMaster.isPlayerTurn);
            return true;
        }

        return false;
    }

    public List<CellSide> GetValidNeighbourSides() {
        List<CellSide> sides = new List<CellSide>();

        if (this.leftLine == null)
            if(this.column == 0 || GameMaster.cells[this.row, this.column - 1].GetFreeSides().Count > 2)
                sides.Add(CellSide.Left);

        if (this.rightLine == null)
            if (this.column == GameMaster.cells.GetLength(0) - 1 || GameMaster.cells[this.row, this.column + 1].GetFreeSides().Count > 2)
                sides.Add(CellSide.Right);

        if (this.bottomLine == null)
            if (this.row == GameMaster.cells.GetLength(1) - 1 || GameMaster.cells[this.row + 1, this.column].GetFreeSides().Count > 2)
                sides.Add(CellSide.Bottom);

        if (this.topLine == null)
            if (this.row == 0 || GameMaster.cells[this.row - 1, this.column].GetFreeSides().Count > 2)
                sides.Add(CellSide.Top);

        return sides;
    }
}