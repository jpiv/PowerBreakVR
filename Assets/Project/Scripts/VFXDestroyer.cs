using System.Collections;
using UnityEngine;

public class VFXDestroyer : MonoBehaviour {
    public float delay = 2f;

    void Start() {
        StartCoroutine(this.Kill());
    }

    IEnumerator Kill() {
        yield return new WaitForSeconds(this.delay);
        GameObject.Destroy(this.gameObject);
    }
}
