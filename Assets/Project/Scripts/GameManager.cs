using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour {
    public InputActionReference resetAction;
    public GameObject ballPrefab;
    public GameObject playerBoard;
    public GameObject playerBoardRenderer;
    public bool autoStart = false;

    void Start() {
        this.resetAction.action.performed += ResetAction;
        StartCoroutine(this.LateStart());
    }

    IEnumerator LateStart() {
        yield return new WaitForSeconds(1);
        if (this.autoStart) this.StartGame();
    }

    public void AddBall(Transform transform) {
        GameObject newBall = Instantiate(this.ballPrefab, transform.position, Quaternion.identity);
        newBall.GetComponent<Rigidbody>().velocity = new Vector3(0, -0.2f, -4);
    }

    public void StartGame() {
        // Setup game controllers
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("GameController");
        GameObject cameraOffset = GameObject.FindGameObjectWithTag("CameraOffset");

        foreach (GameObject controller in controllers) {
            GameObject boardRenderer = Instantiate(this.playerBoardRenderer, controller.transform.position, Quaternion.identity, controller.transform);
            controller.GetComponent<ActionBasedController>().model = boardRenderer.transform;
            controller.GetComponent<XRRayInteractor>().enabled = false;

            GameObject board = Instantiate(this.playerBoard, controller.transform.position, Quaternion.identity, cameraOffset.transform);
            board.GetComponent<PlayerBoard>().controller = controller;
        }

        GameLifecycle.StartGame();
    }

    void ResetAction(InputAction.CallbackContext context) {
        this.Reset();
    }

    public void Reset() {
        GameLifecycle.Reset();
    }

    public void GameOver() {
        GameLifecycle.GameOver();
    }
}
