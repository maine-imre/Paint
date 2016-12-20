﻿using Leap.Unity.Attributes;
using Leap.Unity.RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.UI.Constraint {

  public class LineConstraint : LinearConstraint {

    public enum LineAnchor { Beginning, Center, End }

    [Header("Line Configuration")]
    [Tooltip("The local-space axis the line constraint follows. This axis is independent of the constrained transform's local rotation.")]
    public Axis lineAxis = Axis.Z;
    [Tooltip("The line constraint can be specified relative to this.transform.localPosition as its beginning, center, or end.")]
    public LineAnchor lineAnchor = LineAnchor.Beginning;
    [MinValue(0.0001F)]
    [Tooltip("The total length of the line constraint. This value is NOT affected by this.transform.localScale.")]
    public float length = 0.02F;

    public override void EnforceConstraint() {
      this.transform.localPosition = EvaluatePositionAlongConstraint(GetConstraintProjection());
    }

    public float GetConstraintProjection(Vector3 localPosition) {
      Vector3 zeroToLocalPosition = localPosition - PositionAtZero;
      Vector3 zeroToOne = PositionAtOne - PositionAtZero;
      float unclampedProgress = Vector3.Dot(zeroToLocalPosition, zeroToOne) / zeroToOne.sqrMagnitude;
      return unclampedProgress.Clamp01();
    }

    public override float GetConstraintProjection() {
      return GetConstraintProjection(this.transform.localPosition);
    }

    public override Vector3 EvaluatePositionAlongConstraint(float progressAlongConstraint) {
      return Vector3.Lerp(PositionAtZero, PositionAtOne, progressAlongConstraint);
    }

    /// <summary>
    /// Readonly. Returns the local-space position of the beginning of this LineConstraint.
    /// </summary>
    public Vector3 PositionAtZero {
      get { return LocalCenter - LineVector; }
    }

    /// <summary>
    /// Readonly. Returns the local-space position of the end of this LineConstraint.
    /// </summary>
    public Vector3 PositionAtOne {
      get { return LocalCenter + LineVector; }
    }

    /// <summary>
    /// Readonly. Returns the defining vector for this LineConstraint in local space. (Radius-style: Half the length of the actual constraint.)
    /// </summary>
    public Vector3 LineVector {
      get { return LocalDirection * length / 2; }
    }

    /// <summary>
    /// Returns the local-space position of the center of this LineConstraint.
    /// </summary>
    private Vector3 LocalCenter {
      get {
        switch (lineAnchor) {
          case LineAnchor.Beginning:
            return constraintLocalPosition + LineVector;
          case LineAnchor.Center:
            return constraintLocalPosition;
          case LineAnchor.End: default:
            return constraintLocalPosition - LineVector;
        }
      }
    }

    private Vector3 LocalDirection {
      get {
        switch (lineAxis) {
          case Axis.X:
            return new Vector3(1, 0, 0);
          case Axis.Y:
            return new Vector3(0, 1, 0);
          case Axis.Z: default:
            return new Vector3(0, 0, 1);
        }
      }
    }

    private float LocalDirectionScale {
      get {
        switch (lineAxis) {
          case Axis.X:
            return this.transform.localScale.x;
          case Axis.Y:
            return this.transform.localScale.y;
          case Axis.Z: default:
            return this.transform.localScale.z;
        }
      }
    }

    #region Gizmos

    private const float GIZMO_THICKNESS = 0.01F;

    void OnDrawGizmos() {
      Gizmos.color = DefaultGizmoColor;
      Gizmos.DrawLine(this.transform.parent.TransformPoint(PositionAtZero),
                      this.transform.parent.TransformPoint(PositionAtOne ));
    }

    #endregion

  }

}