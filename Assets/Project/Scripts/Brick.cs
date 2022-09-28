using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour {
    public GameObject explosion;
    public Gradient healthColorGradient;
    public GameObject vfx;
    public delegate void DestroyAction();
    public event DestroyAction OnDestroy;

    private Score scoreboard;
    private AudioSource audioSource;
    private Material material;
    private float health = 1f;
    private float maxHealth = 6f;

    void Start() {
        this.audioSource = GetComponent<AudioSource>();
        this.material = GetComponent<Renderer>().material;
        this.scoreboard = GameObject.FindObjectOfType<Score>();
        this.UpdateHealth();
    }

    public void SetHealth(int health) {
        this.health = (float) Mathf.Min(health, (int)this.maxHealth);
        this.UpdateHealth();
    }

    void UpdateHealth() {
        float normalizedHealth = (this.health - 1) / (this.maxHealth - 1);
        float opacity = 0.6f + 0.4f * normalizedHealth;
        Color brickColor = this.healthColorGradient.Evaluate(normalizedHealth);
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", brickColor);
        this.GetComponent<Renderer>().material.SetColor("_Color", brickColor);
        this.GetComponent<Renderer>().material.SetFloat("_Opacity", opacity);
    }

    void OnBrickHit(Ball ball) {
        this.GetComponent<Animator>().Play("Impact");
        this.health -= 1;
        this.UpdateHealth();
        if (this.health <= 0) {
            Instantiate(this.explosion, this.transform.position, Quaternion.identity);
            Instantiate(this.vfx, this.transform.position, Quaternion.identity);
            StartCoroutine(this.Destroy());
            this.scoreboard.AddScore(10);
        } else {
            this.audioSource.Play();
        }
    }

    void OnCollisionEnter(Collision col) {
        if (GameUtils.isCollisionWith(col, "Ball")) {
            Ball ball = col.gameObject.GetComponent<Ball>();
            this.OnBrickHit(ball);
        }
    }

    IEnumerator Destroy() {
        yield return new WaitForSeconds(0.15f);
        GameObject.FindObjectOfType<BrickController>().Remove(this.gameObject);
        this.OnDestroy();
    }
}
