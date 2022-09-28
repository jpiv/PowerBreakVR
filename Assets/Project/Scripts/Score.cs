using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour {
    public TextMeshPro textMesh;
    private int score = 0;

    void Start() {
        this.UpdateText();
    }

    void UpdateText() {
        this.textMesh.text = score.ToString();
    }
    
    public void AddScore(int inc) {
        this.score += inc;
        this.UpdateText();
    }
}
