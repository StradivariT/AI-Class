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

    public struct AICell {
        public List<CellSide> Sides;
        public Cell Cell;

        public AICell(Cell cell, List<CellSide> sides) {
            this.Cell = cell;
            this.Sides = sides;
        }
    }
}
