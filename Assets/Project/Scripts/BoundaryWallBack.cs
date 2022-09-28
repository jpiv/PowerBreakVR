using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWallBack : MonoBehaviour {
    void Start() {
        
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Ball") {
            Ball ball = col.gameObject.GetComponent<Ball>();
            if (ball != null) {
                ball.Reset();
            }
        }
    }
}
