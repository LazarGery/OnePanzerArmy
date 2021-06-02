using UnityEngine;

public interface ICover
{
    CoverPoint GetCover(Vector3 Position, Vector3 Target, float ViewDistance, float BuildingsRadius);
    void FreeCover(CoverPoint Point);
}
