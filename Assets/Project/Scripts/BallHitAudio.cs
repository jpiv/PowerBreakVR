using UnityEngine;

public class BallHitAudio : MonoBehaviour {
    private AudioSource audioSource;

    void Start() {
        this.audioSource = GetComponent<AudioSource>();        
    }

    void OnCollisionEnter(Collision col) {
        if (GameUtils.isCollisionWith(col, "Ball")) {
            this.audioSource.Play();
        }
    }
}
