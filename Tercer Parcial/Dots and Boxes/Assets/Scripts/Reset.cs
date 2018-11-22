using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {
    private GameMaster gameMaster;

    private void Awake() {
        this.gameMaster = (GameMaster)FindObjectOfType(typeof(GameMaster));
    }

    private void OnMouseOver() {
        this.transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }

    private void OnMouseExit() {
        this.transform.localScale = Vector3.one;
    }

    private void OnMouseDown() {
        this.gameMaster.ResetGame();
        Destroy(this.gameObject);
    }
}
