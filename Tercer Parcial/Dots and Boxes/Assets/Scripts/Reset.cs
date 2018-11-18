using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {
    void OnMouseDown() {
        GameMaster.ResetGame();

        Destroy(this.gameObject);
    }
}
