using UnityEngine;

class Node : IHeapItem<Node>
{
    public Vector2Int Position { get; private set; }
    public int HeapIndex { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public Node Parent { get; set; }

    public Node(int X, int Y)
    {
        Position = new Vector2Int(X, Y);
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
}
