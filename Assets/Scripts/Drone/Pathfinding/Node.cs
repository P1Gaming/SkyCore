using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPathfinding
{
    public struct Node
    {
        public Vector3Int coords; // Position relative start. Use ints to avoid issues with comparing nearly identical floats
        public Vector3Int cameFrom;

        public float g; // G cost. Distance along best known path.

        public float h; // H cost. Distance from end.

        public float F => g + h; // Total cost.


        public Node(Vector3Int coords, float g, float h, Vector3Int cameFrom)
        {
            this.coords = coords;
            this.g = g;
            this.h = h;
            this.cameFrom = cameFrom;
        }

        public override string ToString()
        {
            return "{NODE: " + $"positionRelativeStart: {coords}, cameFrom: {cameFrom}, g: {g}, h: {h}, f: {F}" + "}";
        }
    }
}