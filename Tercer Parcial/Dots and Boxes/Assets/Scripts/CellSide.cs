using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotsAndBoxes {
    public enum CellSide {
        Left,
        Right,
        Bottom,
        Top
    }

    public struct LineTransform {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
