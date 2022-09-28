using UnityEngine;

public class StartUI : MonoBehaviour, IGameLifecycle {
    private GameManager gameManager;

    void Start() {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();
        GameLifecycle.OnStartGame += this.Destroy;
    }


    public void StartGame() {
        this.gameManager.StartGame();
    }

    public void Destroy() {
        GameObject.Destroy(this.gameObject);
    }

    public void Reset() {
        Debug.Log("Reset StartUI here...");
    }
}
