using System.Collections;
using UnityEngine;

public static class GameUtils {
  public static Vector3 LocalTransformVec(Transform to, Transform from) {
    return to.InverseTransformVector(from.TransformVector(from.position));
  }

  public static bool isCollisionWith(Collision col, string tag) {
    return col.gameObject.tag == tag;
  }

  public static float AbsClamp(float val, float min, float max) {
    float abs = Mathf.Abs(val);
    float multiplier = val == 0f ? 1f : (val / Mathf.Abs(val));
    return Mathf.Clamp(abs, min, max) * multiplier;
  }
}

public class VectorPid {
  public float pFactor, iFactor, dFactor;

  private Vector3 integral;
  private Vector3 lastError;

  public VectorPid(float pFactor, float iFactor, float dFactor)
  {
    // P - Proportion -> Velocity
    this.pFactor = pFactor;
    // I - Interal -> Position
    this.iFactor = iFactor;
    // D - Derivative -> Acceleration
    this.dFactor = dFactor;
  }

  public Vector3 Update(Vector3 currentError, float timeFrame)
  {
      integral += currentError * timeFrame;
      var deriv = (currentError - lastError) / timeFrame;
      lastError = currentError;
      return currentError * pFactor
          + integral * iFactor
          + deriv * dFactor;
  }
}