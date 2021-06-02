using UnityEngine;

public class CoverPoint
{
    Vector3 _Position;
    public Vector3 Position
    {
        get { return _Position + (_OffsetVector * _OffsetCount); }
        private set { _Position = value; }
    }

    Vector3 _Front;
    public Vector3 Front
    {
        get { return _Front + (_OffsetVector * _OffsetCount); }
        private set { _Front = value; }
    }

    Vector3 _OffsetVector;
    int _OffsetCount;
    int _MaxOffsetCount;

    public CoverPoint(Vector3 Position, Vector3 Front)
    {
        _Position = Position;
        _Front = Front;
        _OffsetVector = Position - Front;
    }

    public void SetMaxOffsetCount(int MaxOffsetCount)
    {
        if (MaxOffsetCount >= 0)
        {
            _MaxOffsetCount = MaxOffsetCount;
        }
    }

    public bool MoveForward()
    {
        if (_OffsetCount > 0)
        {
            _OffsetCount -= 1;
            return true;
        }
        return false;
    }

    public bool MoveBackward()
    {
        if (_OffsetCount < _MaxOffsetCount)
        {
            _OffsetCount += 1;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _OffsetCount = 0;
    }
}
