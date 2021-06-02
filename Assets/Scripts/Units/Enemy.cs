using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float HitPoints;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float TurnSpeed;

    [SerializeField]
    private float PositioningSpeed;

    float _HitPoints;
    EnemyTurret _Turret;

    List<Vector2Int> _Path;
    float _FieldOffsetX, _FieldOffsetY;
    Vector3 _Destination;
    public MovementState State { get; private set; }

    bool _isMovingToCover, _isPositioning;
    Vector3 _Direction;

    // Start is called before the first frame update
    void Start()
    {
        _Turret = gameObject.GetComponentInChildren<EnemyTurret>();
        _HitPoints = HitPoints;
        _Path = new List<Vector2Int>();
        _FieldOffsetX = GameController.Instance.Map.Field_Offset_X;
        _FieldOffsetY = GameController.Instance.Map.Field_Offset_Y;
    }

    // Update is called once per frame
    void Update()
    {
        if (State == MovementState.Moving && Rotate(_Destination) && Move(_Destination))
        {
            if (_Path.Count > 0)
            {
                _Destination = new Vector3(_Path[0].x + _FieldOffsetX, _Path[0].y + _FieldOffsetY);
                _Path.RemoveAt(0);
            }
            else if (_isMovingToCover)
            {
                State = MovementState.Positioning;
                _isMovingToCover = false;
                _Turret.ReachedCover(this);
            }
            else
            {
                State = MovementState.Idle;
            }
        }
        else if (State == MovementState.Positioning && Rotate(_Direction) && _isPositioning && Move(_Destination))
        {
            _Turret.ReachedPosition(this);
            _isPositioning = false;
        }
    }

    public void ApplyDamage(float Damage, Vector3 Source)
    {
        _HitPoints -= Damage;
        _Turret.GotHit(Source, this);
    }

    public void MoveLocation(Vector3 Position, EnemyTurret Sender)
    {
        if (_Turret != null && Sender == _Turret &&
            State == MovementState.Idle)
        {
            _Path = GameController.Instance.PathFinder.FindPath(transform.position, Position);
            if (_Path != null && _Path.Count > 0)
            {
                _Destination = new Vector3(_Path[0].x + _FieldOffsetX, _Path[0].y + _FieldOffsetY);
                _Path.RemoveAt(0);
                State = MovementState.Moving;
            }
        }
    }

    public void MoveToCover(Vector3 Position, Vector3 Front, EnemyTurret Sender)
    {
        if (_Turret != null && Sender == _Turret)
        {
            MoveLocation(Position, Sender);
            _isMovingToCover = true;
            Vector2Int location;
            if (_Path != null && _Path.Count != 0)
            {
                if (_Path.Count > 2)
                {
                    location = _Path[_Path.Count - 2];
                }
                else if (_Path.Count == 1)
                {
                    location = new Vector2Int(Mathf.FloorToInt(_Destination.x), Mathf.FloorToInt(_Destination.y));
                }
                else
                {
                    location = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
                }

                if (location.x == Mathf.FloorToInt(Front.x) && location.y == Mathf.FloorToInt(Front.y))
                {
                    _Direction = Position - Front;
                }
                else
                {
                    _Direction = Position - Front;
                }
            }
        }
    }

    public void MovePosition(Vector3 Position, EnemyTurret Sender)
    {
        if (_Turret != null && Sender == _Turret &&
            State == MovementState.Positioning)
        {
            _Destination = Position;
            _isPositioning = true;
        }
    }

    public void StopMovements(EnemyTurret Sender)
    {
        if (_Turret != null && Sender == _Turret &&
            (State == MovementState.Moving || State == MovementState.Positioning))
        {
            if (_Path != null && _Path.Count > 0)
            {
                _Path.Clear();
            }
            State = MovementState.Idle;
            _isMovingToCover = false;
        }
    }

    public void StopPositioning(EnemyTurret Sender)
    {
        if (_Turret != null && Sender == _Turret &&
            State == MovementState.Positioning)
        {
            _isPositioning = false;
        }
    }

    bool Rotate(Vector3 Direction)
    {
        Vector3 difference;
        if (State != MovementState.Positioning)
        {
            difference = Direction - transform.position;
        }
        else
        {
            difference = Direction;
        }
        difference.Normalize();
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90;
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), TurnSpeed * Time.deltaTime);
        if (Mathf.Abs(transform.rotation.eulerAngles.z - nextRotation.eulerAngles.z) > 0.1f)
        {
            transform.rotation = nextRotation;
            return false;
        }
        return true;
    }

    bool Move(Vector3 Position)
    {
        if (State != MovementState.Positioning)
        {
            transform.position = Vector3.MoveTowards(transform.position, Position, Speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Position, PositioningSpeed * Time.deltaTime);
        }
        return (transform.position == Position);
    }

    private void LateUpdate()
    {
        if (_HitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
