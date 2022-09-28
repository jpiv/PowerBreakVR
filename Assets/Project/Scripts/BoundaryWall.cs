using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWall : MonoBehaviour {
    public GameObject impact;
    private Animator impactAnimation;

    void Start() {
        this.impactAnimation = this.impact.GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision col) {
        if (!GameUtils.isCollisionWith(col, "Ball")) return;
        ContactPoint contact = col.GetContact(0);
        Vector3 normal = contact.normal;
        this.impact.transform.position = contact.point + normal * -0.001f;
        this.impactAnimation.Play("Impact");
    }
}
