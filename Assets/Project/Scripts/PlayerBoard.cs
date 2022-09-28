using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerBoard : MonoBehaviour {
    public float movementMultiplier = 1f;
    public float maxAngularVelocity = 1f;
    public float P = 1f;
    public float I = 1f;
    public float D = 1f;
    public float boost = 1f;
    [Header("Hit Vibration")]
    public float vibrateAmplitude = 1f;
    public float vibrateDuration = 0.5f;
    public GameObject controller;

    private Rigidbody body;
    private VectorPid headingControllerX;
    private VectorPid headingControllerY;
    private VectorPid headingControllerZ;
    private VectorPid angularVelocityController;

    void Start() {
        this.headingControllerX = new VectorPid(P, I, D);
        this.headingControllerY = new VectorPid(P, I, D);
        this.headingControllerZ = new VectorPid(P, I, D);
        this.angularVelocityController = new VectorPid(0.1f, 0.1f, 0.1f);
        this.body = this.GetComponent<Rigidbody>();
        this.body.maxAngularVelocity = this.maxAngularVelocity;
    }

    void UpdateBouncerAngle() {
        Quaternion boardAngle = this.body.transform.rotation;
        Quaternion controllerAngle = this.controller.transform.rotation;
        Transform controllerTransform = controller.GetComponent<ActionBasedController>().model.transform;
        float distance = Quaternion.Angle(boardAngle, controllerAngle);

        if (distance < 0.1f) {
            this.body.angularVelocity = Vector3.zero;
        } else {
            this.PID(transform.up, controllerTransform.up, this.headingControllerX);
            this.PID(transform.forward, controllerTransform.forward, this.headingControllerY);
            this.PID(transform.right, controllerTransform.right, this.headingControllerZ);
        }
    }

    void PID(Vector3 currentHeading, Vector3 desiredHeading, VectorPid controller) {
        var transformPos = transform.position;
        Debug.DrawRay(transformPos, desiredHeading, Color.magenta);
        Debug.DrawRay(transformPos, currentHeading * 15, Color.blue);
        var headingError = Vector3.Cross(currentHeading, desiredHeading);
        var headingCorrection = controller.Update(headingError, Time.deltaTime);

        this.body.AddTorque(headingCorrection, ForceMode.VelocityChange);
    }

    void UpdateBouncePos() {
        Vector3 boardPos = this.body.transform.position;
        Vector3 controllerPos = this.controller.transform.position;
        Vector3 distance = (controllerPos - boardPos);
        float magnitude = Mathf.Pow(distance.magnitude, 1) * this.movementMultiplier;

        if (Vector3.Dot(distance.normalized, this.body.velocity.normalized) < 0) {
            this.body.velocity = Vector3.zero;
        }
        this.body.AddForce(magnitude * distance.normalized, ForceMode.VelocityChange);

    }

    void OnCollisionEnter(Collision col) {
        if (GameUtils.isCollisionWith(col, "Ball")) {
            Ball ball = col.gameObject.GetComponent<Ball>();
            if (ball != null) {
                ball.Boost(this.boost);
                ball.AddIntensity();
                this.controller.GetComponent<ActionBasedController>().SendHapticImpulse(this.vibrateAmplitude, this.vibrateDuration);
            }
        }
    }

    void Update() {
        this.UpdateBouncePos();
        this.UpdateBouncerAngle();
    }

    void FixedUpdate() {
    }
}
