using System.Collections.Generic;
using UnityEngine;

class PathFinder: IPathFind
{
    Node[,] _Nodes;
    int _OffsetX, _OffsetY;
    Heap<Node> _OpenSet;
    Dictionary<Vector2Int, Node> _ClosedSet;

    public PathFinder(MapGrid Map)
    {
        if (Map != null && Map.Tiles != default(bool[,]))
        {
            _Nodes = new Node[Map.Tiles.GetLength(0), Map.Tiles.GetLength(1)];
            _OffsetX = Map.Offset_X;
            _OffsetY = Map.Offset_Y;
            int count = 0;

            for (int x = 0; x < _Nodes.GetLength(0); x++)
            {
                for (int y = 0; y < _Nodes.GetLength(1); y++)
                {
                    if (Map.Tiles[x, y])
                    {
                        _Nodes[x, y] = new Node(x - _OffsetX, y - _OffsetY);
                        count += 1;
                    }
                    else
                    {
                        _Nodes[x, y] = null;
                    }
                }
            }

            for (int x = 0; x < _Nodes.GetLength(0); x++)
            {
                for (int y = 0; y < _Nodes.GetLength(1); y++)
                {
                    if (_Nodes[x, y] != null)
                    {
                        _Nodes[x, y].SetNeighbours(GetNeighbours(_Nodes[x, y]));
                    }
                }
            }

            _OpenSet = new Heap<Node>(count);
            _ClosedSet = new Dictionary<Vector2Int, Node>();
        }
    }

    // Calculates the optimal route from the given StartPosition to the TargetPosition
    // Returns a List - which contains the Vectors representing each field of the found route
    // Returns NULL - if one of the input parameters is invalid or points to a non walkable field
    //                or the two given Positions representing the same field
    // Returns an Empty List - if the TargetPosition cannot be reached from the StartPosition
    public List<Vector2Int> FindPath(Vector3 StartPosition, Vector3 TargetPosition)
    {
        // Check if the StartPosition and the TargetPosition are valid coordinates
        int startPosition_X = Mathf.FloorToInt(StartPosition.x) + _OffsetX;
        int startPosition_Y = Mathf.FloorToInt(StartPosition.y) + _OffsetY;
        int targetPosition_X = Mathf.FloorToInt(TargetPosition.x) + _OffsetX;
        int targetPosition_Y = Mathf.FloorToInt(TargetPosition.y) + _OffsetY;
        if (startPosition_X < 0 || startPosition_X > _Nodes.GetLength(0) - 1 ||
            startPosition_Y < 0 || startPosition_Y > _Nodes.GetLength(1) - 1 ||
            targetPosition_X < 0 || targetPosition_X > _Nodes.GetLength(0) - 1 ||
            targetPosition_Y < 0 || targetPosition_Y > _Nodes.GetLength(1) - 1)
        {
            return null;
        }

        // Check if the StartPosition and the TargetPosition are the coordinates of walkable fields
        // and they are not pointing to the same field
        Node startNode = _Nodes[startPosition_X, startPosition_Y];
        Node targetNode = _Nodes[targetPosition_X, targetPosition_Y];
        if (startNode == null || targetNode == null || startNode == targetNode)
        {
            return null;
        }

        _OpenSet.Reset();
        _ClosedSet.Clear();
        Node currentNode = startNode;
        currentNode.GCost = 0;
        currentNode.HCost = GetDistance(currentNode.Position, targetNode.Position);
        List<Vector2Int> result = new List<Vector2Int>();

        // Until the field representing the TargetPosition has been found or the algorithm ran out of possibilities
        while (currentNode != null && currentNode != targetNode)
        {
            foreach (Node actualNode in currentNode.Neighbours)
            {
                int cost = currentNode.GCost + GetDistance(currentNode.Position, actualNode.Position);
                if (!_OpenSet.Contains(actualNode) && !_ClosedSet.ContainsKey(actualNode.Position))
                {
                    actualNode.GCost = cost;
                    actualNode.HCost = GetDistance(actualNode.Position, targetNode.Position);
                    actualNode.Parent = currentNode;
                    _OpenSet.AddItem(actualNode);
                }
                else if (cost < actualNode.GCost)
                {
                    actualNode.GCost = cost;
                    actualNode.Parent = currentNode;
                    if (_OpenSet.Contains(actualNode))
                    {
                        _OpenSet.UpdateItem(actualNode);
                    }
                    else
                    {
                        _ClosedSet.Remove(actualNode.Position);
                        _OpenSet.AddItem(actualNode);
                    }
                }
            }
            _ClosedSet.Add(currentNode.Position, currentNode);
            currentNode = _OpenSet.GetFirstItem();
        }
        if (currentNode != null)
        {
            result = RetracePath(startNode, targetNode);
        }
        return result;
    }

    int GetDistance(Vector2Int A, Vector2Int B)
    {
        int distanceX = Mathf.Abs(A.x - B.x);
        int distanceY = Mathf.Abs(A.y - B.y);
        if (distanceX > distanceY)
        {
            return (14 * distanceY + 10 * (distanceX - distanceY));
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    List<Node> GetNeighbours(Node CurrentNode)
    {
        List<Node> result = new List<Node>();
        int tileX = CurrentNode.Position.x + _OffsetX;
        int tileY = CurrentNode.Position.y + _OffsetY;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                // The field is not the same as the CurrentNode, valid and walkable
                if ((x != 0 || y != 0) &&
                    (tileX + x > -1 && tileX + x < _Nodes.GetLength(0)) &&
                    (tileY + y > -1 && tileY + y < _Nodes.GetLength(1)) &&
                    _Nodes[tileX + x, tileY + y] != null)
                {
                    if (x == 0 || y == 0)
                    {
                        result.Add(_Nodes[tileX + x, tileY + y]);
                    }
                    else if (_Nodes[tileX, tileY + y] != null && _Nodes[tileX + x, tileY] != null)
                    {
                        result.Add(_Nodes[tileX + x, tileY + y]);
                    }
                }
            }
        }
        return result;
    }

    List<Vector2Int> RetracePath(Node StartNode, Node TargetNode)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        Node currentNode = TargetNode;
        while (currentNode != StartNode)
        {
            result.Add(new Vector2Int(currentNode.Position.x, currentNode.Position.y));
            currentNode = currentNode.Parent;
        }
        result.Reverse();
        return result;
    }
}
