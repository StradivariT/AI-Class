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

    public struct GoodCell {
        public List<CellSide> validSides;
        public Cell cell;

        public GoodCell(Cell cell, List<CellSide> sides) {
            this.cell = cell;
            this.validSides = sides;
        }
    }
}
