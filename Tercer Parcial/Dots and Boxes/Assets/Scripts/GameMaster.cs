using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {
    public static bool isPlayerTurn;
    public static float cellSize;

    public static Cell[,] cells;

    public static int playerScore;
    public static int aiScore;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject square;

    [SerializeField]
    private Cell cell;

    [SerializeField]
    [Range(3, 10)]
    private int gridSize;

    [SerializeField]
    private Text playerScoreText;

    [SerializeField]
    private Text aiScoreText;

    private float height;
    private float width;
    private float lineWidth;
    private float squareScale;
    private float topMargin;
    private float yZero;
    private float xZero;

    private static GameMaster gameMaster;

	void Start () {
        GameMaster.gameMaster = this;

        if (Random.Range(0, 1000) > 499)
            GameMaster.isPlayerTurn = true;

        this.Setup();
	}
	
    void Setup() {
        this.height = this.mainCamera.orthographicSize * 2f;
        this.width = this.height * this.mainCamera.aspect;

        this.xZero = (this.transform.position.x - this.width / 2);
        this.yZero = (this.transform.position.y + this.height / 2);

        GameMaster.cellSize = this.width / (float)(this.gridSize + 2);
        this.lineWidth = GameMaster.cellSize / 12f;

        this.squareScale = this.lineWidth * 3f;

        this.topMargin = this.height - (float)(this.gridSize + 1) * GameMaster.cellSize;

        GameMaster.cells = new Cell[this.gridSize, this.gridSize];
        for (int i = 0; i <= this.gridSize; i++) {
            for (int j = 0; j <= this.gridSize; j++) {
                GameObject newSquare = Instantiate(this.square, new Vector3(this.GetGridX(j), this.GetGridY(i)), Quaternion.identity);
                newSquare.GetComponent<Transform>().localScale = new Vector3(this.squareScale, this.squareScale);

                if (i == this.gridSize || j == this.gridSize) {
                    continue;
                }

                GameMaster.cells[i, j] = (Cell)Instantiate(this.cell, new Vector3(this.GetGridX(j) + (GameMaster.cellSize / 2), this.GetGridY(i) - (GameMaster.cellSize / 2), 2), Quaternion.identity);
                GameMaster.cells[i, j].GetComponent<Transform>().localScale = new Vector2(GameMaster.cellSize - this.squareScale, GameMaster.cellSize - this.squareScale);
                //GameMaster.cells[i, j].GetComponent<BoxCollider2D>().size = new Vector2(1f + this.lineWidth, 1f + this.lineWidth);
                GameMaster.cells[i, j].column = j;
                GameMaster.cells[i, j].row = i;
            }
        }
    }

    float GetGridX(int column) {
        return this.xZero + (GameMaster.cellSize * (float)(column + 1));
    }

    float GetGridY(int row) {
        return this.yZero - (this.topMargin + (float)(GameMaster.cellSize * row));
    }

    public static void UpdateScore() {
        if(GameMaster.isPlayerTurn) {
            GameMaster.playerScore++;
        } else {
            GameMaster.aiScore++;
        }

        GameMaster.gameMaster.UpdateScoreText();
    }

    public void UpdateScoreText() {
        if(GameMaster.isPlayerTurn) {
            this.playerScoreText.text = GameMaster.playerScore.ToString();
        } else {
            this.aiScoreText.text = GameMaster.aiScore.ToString();
        }
    }
}
