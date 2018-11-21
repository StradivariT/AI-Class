using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {
    [SerializeField]
    private GameObject squarePrefab;

    [SerializeField]
    private Cell cellPrefab;

    [SerializeField]
    [Range(2, 10)]
    private int gridSize;

    [SerializeField]
    private Text playerScoreText;

    [SerializeField]
    private Text aiScoreText;

    [SerializeField]
    private Text winnerText;

    [SerializeField]
    private GameObject squareParent;

    [SerializeField]
    private GameObject cellParent;

    [SerializeField]
    private GameObject resetButton;

    private float yZero;
    private float xZero;

    private float topMargin;

    public int PlayerScore { get; set; }
    public int AIScore { get; set; }

    public float CellSize { get; set; }
    public float SquareSize { get; set; }

    public bool IsPlayerTurn { get; set; }

    public List<List<Cell>> Cells;

	private void Start () {
        this.IsPlayerTurn = true;

        float viewHeight = Camera.main.orthographicSize * 2f;
        float viewWidth = viewHeight * Camera.main.aspect;

        this.xZero = this.transform.position.x - viewWidth/ 2f;
        this.yZero = this.transform.position.y + viewHeight / 2f;

        this.CellSize = viewWidth / (float)(this.gridSize + 2);
        this.topMargin = viewHeight - (float)(this.gridSize + 1) * this.CellSize;

        this.SquareSize = this.CellSize / 4f;

        this.SetupGame();
	}
	
    private void SetupGame() {
        this.Cells = new List<List<Cell>>();

        for (int i = 0; i <= this.gridSize; i++) {
            if (i != this.gridSize)
                this.Cells.Add(new List<Cell>());

            for (int j = 0; j <= this.gridSize; j++) {
                this.InstantiateSquare(j, i);

                if (i == this.gridSize || j == this.gridSize)
                    continue;

                this.Cells[i].Add(this.InstantiateCell(j, i));
            }
        }

        this.IsPlayerTurn |= Random.Range(0, 1000) > 499;
    }

    private void InstantiateSquare(int column, int row) {
        GameObject square = Instantiate(this.squarePrefab, new Vector3(this.GetGridX(column), this.GetGridY(row)), Quaternion.identity);

        RectTransform squareRect = square.GetComponent<RectTransform>();

        squareRect.SetParent(this.squareParent.transform);
        squareRect.localScale = new Vector3(this.SquareSize, this.SquareSize);
    }

    private Cell InstantiateCell(int column, int row) {
        float halfCellSize = this.CellSize / 2f;

        Cell cell = (Cell)Instantiate(this.cellPrefab, new Vector3(this.GetGridX(column) + halfCellSize, this.GetGridY(row) - halfCellSize, 2f), Quaternion.identity);

        cell.Column = column;
        cell.Row = row;

        RectTransform cellRect = cell.GetComponent<RectTransform>();

        cellRect.SetParent(this.cellParent.transform);
        cellRect.localScale = new Vector2(this.CellSize - this.SquareSize, this.CellSize - this.SquareSize);

        BoxCollider2D cellCollider = cell.GetComponent<BoxCollider2D>();

        cellCollider.size = new Vector2(1.333f, 1.333f);

        return cell;
    }

    private float GetGridX(int column) {
        return this.xZero + (this.CellSize * (float)(column + 1));
    }

    private float GetGridY(int row) {
        return this.yZero - (this.topMargin + (float)(this.CellSize * row));
    }

    public void UpdateTurn() {
        this.IsPlayerTurn = !this.IsPlayerTurn;
    }

    public void UpdateScore() {
        if(this.IsPlayerTurn) {
            this.PlayerScore++;
            this.playerScoreText.text = this.PlayerScore.ToString();
        } else {
            this.AIScore++;
            this.aiScoreText.text = this.AIScore.ToString();
        }

        if (this.PlayerScore + this.AIScore == this.gridSize * this.gridSize)
            this.EndGame();
    }

    private void EndGame() {
        this.IsPlayerTurn = true;

        if (this.PlayerScore > this.AIScore)
            this.winnerText.text = "Player Wins!";
        else if (this.PlayerScore < this.AIScore)
            this.winnerText.text = "AI Wins!";
        else
            this.winnerText.text = "Draw!";

        Instantiate(this.resetButton, new Vector3(this.transform.position.x, this.yZero - (this.topMargin / 5f)), Quaternion.identity);
    }

    public void ResetGame() {
        foreach (Transform square in this.squareParent.transform)
            Destroy(square.gameObject);

        foreach (Transform cell in this.cellParent.transform)
            Destroy(cell.gameObject);

        this.PlayerScore = 0;
        this.AIScore = 0;

        this.winnerText.text = "";
        this.playerScoreText.text = "0";
        this.aiScoreText.text = "0";

        this.SetupGame();
    }
}
