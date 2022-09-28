using UnityEngine;

public class Boundary : MonoBehaviour {
    public Collider frontCollider;
    public Collider backCollider;


    void Start() {
        
    }

    public Bounds GetBounds() {
        Bounds frontBounds = frontCollider.bounds;
        Bounds backBounds = backCollider.bounds;

        Vector3 center = new Vector3(
            frontBounds.center.x,
            frontBounds.center.y,
            (frontBounds.center.z - backBounds.center.z) / 2
        );
        Vector3 size = new Vector3(
            frontBounds.size.x,
            frontBounds.size.y,
            frontBounds.center.z - backBounds.center.z - (frontBounds.extents.z)
        );

        return new Bounds(center, size);
    }
}
