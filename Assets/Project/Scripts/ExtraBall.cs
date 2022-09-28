using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBall : MonoBehaviour {
    private GameManager gameController;
    private bool ballCreated = false;
    private Brick brick;

    void Start() {
        this.brick = GetComponent<Brick>();
        this.gameController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>(); 
        this.brick.OnDestroy += this.OnDestroy;
    }

    void OnDestroy() {
        if (!this.ballCreated) {
            this.gameController.AddBall(this.transform);
            this.ballCreated = true;
        }
    }
}
