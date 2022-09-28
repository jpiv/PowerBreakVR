using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour {

    void Start() {
        GameLifecycle.OnGameOver += this.OnGameOver;
        this.gameObject.SetActive(false);
    }

    void OnGameOver() {
        this.gameObject.SetActive(true);
    }
}
