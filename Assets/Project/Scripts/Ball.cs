using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    private Rigidbody body;
    private Vector3 initialPosition;
    private ParticleSystem sparks;
    private TrailRenderer trail;
    private float intensity = 0f;
    private float baseMinVelocity;

    public Vector3 initialVelocity;
    public float sparkVelocityMultiplier;
    public float maxVelocity;
    public float minVelocity;
    public bool canReset = false;
    [Header("Heat Seeking")]
    public float curveMultiplier;
    public float seekingRange;
    public bool frozen = false;

    void Start() {
        this.initialPosition = this.transform.position;
        this.body = this.gameObject.GetComponent<Rigidbody>();
        this.sparks = GetComponent<ParticleSystem>();
        this.trail = GetComponent<TrailRenderer>();
        this.body.maxAngularVelocity = 0f;
        this.baseMinVelocity = this.minVelocity;
        GameLifecycle.OnReset += this.Reset;
        GameLifecycle.OnStartGame += this.Reset;
        GameLifecycle.OnGameOver += this.OnGameOver;
    }

    void FixedUpdate() {
        if (!this.frozen) {
            float maxXYVel = 2.1f;
            float zVel = this.body.velocity.z;
            float multiplier = zVel == 0f ? 1f : (zVel / Mathf.Abs(zVel));
            this.body.velocity = new Vector3(
                Mathf.Min(this.body.velocity.x, maxXYVel),
                Mathf.Min(this.body.velocity.y, maxXYVel),
                GameUtils.AbsClamp(this.body.velocity.z, this.minVelocity, this.maxVelocity)
            );

            // Curve
            this.CurveBall();
        }
    }

    void OnGameOver() {
        this.frozen = true;
        this.body.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision col) {
        this.ShootSparks(col);
    }

    void ShootSparks(Collision col) {
        if (GameUtils.isCollisionWith(col, "Wall")) return;
        ParticleSystem.MainModule main = this.sparks.main;

        main.startSpeed =  this.body.velocity.magnitude * this.sparkVelocityMultiplier;
        ParticleSystem.ShapeModule shape = this.sparks.shape;
        shape.position = this.body.velocity.normalized * -0.5f;
        shape.rotation = Quaternion.LookRotation(this.body.velocity * -1).eulerAngles;
        this.sparks.Play();
    }

    public void Reset(){
        if (this.canReset) {
            this.frozen = false;
            this.intensity = 0f;
            this.SetIntensity(this.intensity);
            this.transform.position = this.initialPosition;
            this.body.velocity = Vector3.zero;
            this.body.AddForce(this.initialVelocity, ForceMode.VelocityChange);
        } else {
            GameObject.Destroy(this);
        }
    }

    /**
    * Cast in direction
    *   If hit block guide
    *   If hit wall do nothing
    * On wall bounce cast again
    * Only cast if velocity is forward
    */
    void CurveBall() {
        // Only curve if the ball is heading away from you
        if (this.body.velocity.z <= 0) return;
        float maxDistance = 100f;
        Vector3 castDirection = this.body.velocity.normalized;
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, this.seekingRange, castDirection, maxDistance);

        RaycastHit? closestHit = null;
        float shortestDistance = Mathf.Infinity;
        foreach(RaycastHit hit in hits) {
            Vector3 normal = (castDirection * maxDistance) - this.transform.position;
            Vector3 intersection = Vector3.Project(hit.point, normal);
            float distance = (hit.point - intersection).magnitude;
            if (distance < shortestDistance && hit.collider.gameObject.CompareTag("Brick")) {
                closestHit = hit;
                shortestDistance = distance;
            }
        }

        if (closestHit != null) {
            Vector3 brickPos = ((RaycastHit)closestHit).collider.gameObject.transform.position;
            Vector3 force = (brickPos - this.transform.position).normalized * this.curveMultiplier;
            this.body.AddForce(force, ForceMode.Acceleration);
        }
    }

    public void Boost(float magnitude) {
        this.body.AddForce(this.body.velocity.normalized * magnitude, ForceMode.Impulse);
    }

    public void AddIntensity() {
        this.intensity += 1f;
        this.SetIntensity(this.intensity);
    }

    public void SetIntensity(float intensity) {
        float maxIntensity = 7;
        float intensityRatio = this.intensity / 10;
        float intensityAdd = 2f;
        this.intensity = Mathf.Min(this.intensity, maxIntensity);

        this.minVelocity = this.baseMinVelocity + (intensityAdd * intensityRatio);

        // Trail update
        Gradient intensityGradient = new Gradient();
        Gradient trailGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        Color baseColor = Constants.LIGHT_GREEN;
        Color finalColor = Constants.LIGHT_RED;

        colorKeys[0] = new GradientColorKey(baseColor, 0);
        colorKeys[1] = new GradientColorKey(finalColor, 1);
        alphaKeys[0] = new GradientAlphaKey(1, 0);
        alphaKeys[1] = new GradientAlphaKey(1, 1);

        intensityGradient.SetKeys(colorKeys, alphaKeys);

        float gradientTime = Mathf.Min(intensity / maxIntensity, 1);
        Color intensityColor = intensityGradient.Evaluate(gradientTime);
        trailGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(intensityColor, 0), new GradientColorKey(intensityColor, 1) },
            alphaKeys
        );
        this.trail.colorGradient = trailGradient;
    }
}
