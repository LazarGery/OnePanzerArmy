using System.Collections.Generic;
using UnityEngine;

class Node : IHeapItem<Node>
{
    public Vector2Int Position { get; private set; }
    public int HeapIndex { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public Node Parent { get; set; }
    public List<Node> Neighbours { get; private set; }

    public Node(int X, int Y)
    {
        Position = new Vector2Int(X, Y);
        Neighbours = new List<Node>();
    }

    public int FCost
    {
        get { return GCost + HCost; }
    }

    public int CompareTo(Node Other)
    {
        int compare = FCost.CompareTo(Other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(Other.HCost);
        }
        return -compare;
    }

    public void SetNeighbours(List<Node> NeighboursList)
    {
        if (NeighboursList != null)
        {
            Neighbours = NeighboursList;
        }
    }
}
