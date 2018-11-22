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

    public enum Difficulty {
        Easy,
        Normal,
        Hard,
        Insane
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

    public struct ShortestRegion {
        public AICell AICell;
        public int Length;
    }
}
