using System.Collections.Generic;
using UnityEngine;

public interface IPathFind
{
    List<Vector2Int> FindPath(Vector3 StartPosition, Vector3 TargetPosition);
}
