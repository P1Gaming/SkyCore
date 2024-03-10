using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPathfinding
{
    // TODO: the drone can get stuck when it's close to blocks and thinks every adjacent node is obstructed.
    // Detecting that and reducing the radius for the checkcapsule call might work.
    // I'm also not sure how it gets into that situation. It might always be when it sees a direct path to the player
    // or maybe something is going wrong with pathfinding.
    // It's probably obstructed at the start node, even though it's white in the visualization.

    public class AStarPathfinding
    {
        private const int MAX_NODES_TO_CLOSE = 100;

        private const int GRID_SPACING = 1;

        private float _agentRadius;
        private LayerMask _obstacleLayers;
        private Vector3 _start;
        private Vector3 _end;
        private AStarPathfindingVisualization _visualization;

        private BinaryHeap _open = new();
        private Dictionary<Vector3Int, Node> _closed = new(1000);
        private List<Vector3> _path = new();


        public AStarPathfinding(int agentColliderLayer, float agentRadius, bool visualize
            , GameObject pathNodeVisual, GameObject closedNodeVisual, GameObject obstructedNodeVisual)
        {
            _obstacleLayers = LayersWhichCollideWith(agentColliderLayer);
            _agentRadius = agentRadius;

            if (visualize)
            {
                Transform pathFolder = new GameObject("PATH NODES").transform;
                Transform closedFolder = new GameObject("CLOSED NODES").transform;
                Transform obstructedFolder = new GameObject("OBSTRUCTED NODES").transform;

                _visualization = new AStarPathfindingVisualization(pathNodeVisual, pathFolder
                    , closedNodeVisual, closedFolder, obstructedNodeVisual, obstructedFolder);
            }
        }

        public Vector3 CalcPathAndGetVectorToSomewhereOnIt(Vector3 start, Vector3 end)
        {
            _visualization?.Reset();

            // Add a physics check at the start where if it can go straight to the target, it does

            _open.Clear();
            _closed.Clear();

            _start = start;
            _end = end;

            if (CanTravelBetween(start, end))
            {
                return end - start;
            }

            _open.Insert(new Node(Vector3Int.zero, 0, (end - start).magnitude, Vector3Int.zero));

            while (_open.Count > 0 && _closed.Count < MAX_NODES_TO_CLOSE)
            {
                Node current = _open.ExtractNodeWithMinCost();

                _visualization?.ShowClosed(current.coords + _start);
                _closed.Add(current.coords, current);

                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(0, 0, 1));
                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(0, 0, -1));
                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(0, 1, 0));
                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(0, -1, 0));
                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(1, 0, 0));
                ConsiderNeighbor(ref current, GRID_SPACING * new Vector3Int(-1, 0, 0));
            }

            Node closestToEnd = new();
            bool foundClosestToEnd = false;
            float minDistanceFromEnd = float.PositiveInfinity;
            foreach (Node n in _closed.Values)
            {
                if (n.h < minDistanceFromEnd)
                {
                    foundClosestToEnd = true;
                    closestToEnd = n;
                    minDistanceFromEnd = n.h;
                }
            }

            if (!foundClosestToEnd)
            {
                return _end - _start;
            }

            RetracePathBackwards(closestToEnd);

            return _path.Count == 0 ? _end - _start : StraightLineFromStartToPointAlongPath(_start);
        }

        private void ConsiderNeighbor(ref Node node, Vector3Int displacement)
        {
            // The cost of the path to the neighbor (g) = the cost of the path to the prior node, plus the distance to
            // the neighbor. So it's the total length of the path to the node.
            float g = node.g + displacement.magnitude;


            Vector3Int neighborCoords = node.coords + displacement;
            Vector3 neighborPosition = neighborCoords + _start;
            Vector3 nodePosition = node.coords + _start;

            if (!_open.Contains(neighborCoords))
            {
                // Not in open, so add it unless there's already a better path to the position in closed.
                if (!_closed.TryGetValue(neighborCoords, out Node nodeInCoords) || g < nodeInCoords.g)
                {
                    if (!CanTravelBetween(nodePosition, neighborPosition))
                    {
                        // Cannot go from the node to this neighbor.
                        _visualization?.ShowObstructed(neighborPosition);
                        return;
                    }

                    Vector3 position = neighborCoords + _start;
                    float h = (_end - position).magnitude;
                    _open.Insert(new Node(neighborCoords, g, h, node.coords));
                }
            }
            else
            {
                if (!CanTravelBetween(nodePosition, neighborPosition))
                {
                    // Cannot go from the node to this neighbor.
                    _visualization?.ShowObstructed(neighborPosition);
                    return;
                }

                // It's in open, so if this is a better path to this position, change it.
                _open.ChangeCostIfLower(neighborCoords, node.coords, g);
            }
        }

        private void RetracePathBackwards(Node lastInPath)
        {
            // The path will exclude the start node, because that's at the start so trying to go there
            // would mean not moving anywhere.

            _path.Clear();

            Node node = lastInPath;
            while (node.coords != Vector3Int.zero)
            {
                _path.Add(node.coords + _start);
                node = _closed[node.cameFrom];
                if (_path.Count > 10000)
                {
                    throw new System.InvalidOperationException("stop infinite loop in RetracePathBackwards (shouldn't happen, this is a bug)");
                }
            } 

            for (int i = 0; i < _path.Count; i++)
            {
                _visualization?.ShowPathNode(_path[i]);
            }
        }

        private Vector3 StraightLineFromStartToPointAlongPath(Vector3 start)
        {
            // maybe check some points in between the steps of the path.

            // Go straight towards one of the nodes in the path, prefering the ones closer to the end.
            // _path sorts the path backwards (index 0 is closest to the end position) so iterate forwards.

            int betweenPointsPerPathNode = 100 / _path.Count;

            Vector3 targetPosition = _path[0];
            for (int i = 0; i < _path.Count; i++)
            {

                if (i == _path.Count - 1)
                {
                    // There's no next index so need this code
                    if (CanTravelBetween(start, _path[i]))
                    {
                        targetPosition = _path[i];
                        break;
                    }
                }

                bool foundTargetPosition = false;

                for (int j = 0; j < betweenPointsPerPathNode + 1; j++)
                {
                    // e.g. if there's 2 points between nodes, check at 1/3 and 2/3 between.
                    float t = j / (1 + betweenPointsPerPathNode);
                    Vector3 point = Vector3.Lerp(_path[i], _path[i + 1], t);
                    if (CanTravelBetween(start, point))
                    {
                        targetPosition = point;
                        foundTargetPosition = true;
                        break;
                    }
                }

                if (foundTargetPosition)
                {
                    break;
                }
            }

            return targetPosition - start;
        }

        private bool CanTravelBetween(Vector3 a, Vector3 b)
        {
            return !Physics.CheckCapsule(a, b, _agentRadius, _obstacleLayers, QueryTriggerInteraction.Ignore);
        }

        private static LayerMask LayersWhichCollideWith(int layer)
        {
            LayerMask result = new();
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    result |= 1 << i;
                }
            }
            return result;
        }
    }
}