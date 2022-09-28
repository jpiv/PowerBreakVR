using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour, IGameLifecycle {
    public GameObject brickPrefab;
    public GameObject brickExtraBallPrefab;
    public int numRowsZ = 4;
    public float powerUpProbablility = 0.5f;
    public float brickProbablility = 0.2f;
    public float tickerInterval = 0.5f;
    public float ticksPerRound = 2f;
    public bool createBricksOnStart = false;

    private List<List<GameObject>> bricks = new List<List<GameObject>>();
    private bool playing = true;
    private GameManager gameManager;
    private int ticks = 0;

    void Start() {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();
        GameLifecycle.OnStartGame += this.StartGame;
        GameLifecycle.OnReset += this.Reset;
        if (this.createBricksOnStart) {
            this.CreateLayer();
        }
    }

    public void Reset() {
        foreach (List<GameObject> layer in this.bricks) { 
            foreach (GameObject brick in layer) {
                GameObject.Destroy(brick);
            }
        }
        this.bricks = new List<List<GameObject>>();
        this.CreateLayer();
        this.StartTicker();
    }

    public void StartGame() {
        this.CreateLayer();
        this.StartTicker();
    }

    void StartTicker() {
        StartCoroutine(this.Ticker());
    }

    IEnumerator Ticker() {
        while (this.playing) {
            this.CreateLayer();
            this.CheckEnd();
            this.ticks++;
            yield return new WaitForSeconds(this.tickerInterval);
            // this.tickerInterval = Mathf.Max(0.5f, this.tickerInterval * 0.9f);
        }
    }

    void CheckEnd() {
        if (this.bricks.Count == 0) return;
        List<GameObject> firstLayer = this.bricks[0];
        if (firstLayer != null && firstLayer[0] != null && firstLayer[0].transform.position.z <= 0) {
            this.playing = false;
            this.gameManager.GameOver();
        }
    }

    void CleanLayers() {
        List<List<GameObject>> iterList = new List<List<GameObject>>(this.bricks);
        if (this.bricks.Count == 0) return;
        foreach (List<GameObject> layer in iterList) {
            if (layer.Count == 0) {
                this.bricks.Remove(layer);
            }
        }
    }

    public void Remove(GameObject brick) {
        foreach (List<GameObject> layer in this.bricks) {
            if (layer.Contains(brick)) {
                layer.Remove(brick);
                GameObject.Destroy(brick);
            }
        }
        this.CleanLayers();
    }

    void ShiftBricks() {
        if (this.bricks.Count <= 0) return;
        foreach(List<GameObject> layer in this.bricks) { 
            foreach(GameObject brick in layer) {
                if (brick != null) {
                    brick.transform.position -= new Vector3(
                        0,
                        0,
                        brick.GetComponent<BoxCollider>().bounds.size.z
                    );
                }
            }
        }
    }

    GameObject CreateBrick(Vector3 pos) {
        float powerUpRng = Random.Range(0f, 1f);
        int healthRng = Random.Range(1, Mathf.FloorToInt(this.ticks / ticksPerRound));
        GameObject prefab = powerUpRng < this.powerUpProbablility ? this.brickExtraBallPrefab : this.brickPrefab;
        GameObject brickObj = Instantiate(prefab, pos, Quaternion.identity, this.transform);
        Brick brick = brickObj.GetComponent<Brick>();
        brick.SetHealth(healthRng);
        return brickObj;
    }

    void CreateLayer() {
        this.ShiftBricks();
        this.bricks.Add(new List<GameObject>());
        List<GameObject> currentLayer = this.bricks[this.bricks.Count - 1];
        GameObject exampleBrick = Instantiate(this.brickPrefab, new Vector3(100, 100, 100), Quaternion.identity);
        Bounds brickBounds = exampleBrick.GetComponent<BoxCollider>().bounds; 
        Boundary boundary = GameObject.Find("Boundary").GetComponent<Boundary>();
        Bounds boundaryBounds = boundary.GetBounds();
        Vector3 boundsLeft =
            boundaryBounds.center
            + Vector3.Scale(boundaryBounds.extents, new Vector3(-1, 1, 1))
            - Vector3.Scale(brickBounds.extents, new Vector3(-1, 1, 2));

        float buffer = 0.00f;
        float boundsWidth = boundaryBounds.size.x;
        float boundsHeight = boundaryBounds.size.y;
        float boundsDepth = boundaryBounds.size.z;
        float brickWidth = brickBounds.size.x;
        float brickHeight = brickBounds.size.y;
        float brickDepth = brickBounds.size.z;
        float bricksPerRow = Mathf.Floor((boundsWidth - buffer * 2) / brickWidth);
        float numRowsY = Mathf.Floor((boundsHeight - buffer * 2) / brickHeight);

        float spaceLeftX = boundsWidth - bricksPerRow * brickWidth;
        float spacesX = bricksPerRow + 1;
        float spacingX = spaceLeftX / spacesX;
        float spaceLeftY = boundsHeight - numRowsY * brickHeight;
        float spacesY = numRowsY + 1;
        float spacingY = spaceLeftY / spacesY;

        for (int i = 0; i < numRowsY; i++) {
            for (int j = 0; j < this.numRowsZ; j++) {
                for (int k = 0; k < bricksPerRow; k++) {
                    float brickRng = Random.Range(0f, 1f);
                    if (brickRng < this.brickProbablility) {
                        float xPos = ((k + 1) * spacingX) + (k * brickWidth) + buffer;
                        float yPos = (((i + 1) * spacingY) + (i * brickHeight)) * -1 - buffer;
                        float zPos = brickDepth * j * -1 - buffer;
                        Vector3 pos = boundsLeft + new Vector3(xPos, yPos, zPos);
                        currentLayer.Add(this.CreateBrick(pos));
                    }
                }
            }
        }
        GameObject.Destroy(exampleBrick);
    }
}
