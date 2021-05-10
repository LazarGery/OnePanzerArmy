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
    private float FieldOffsetX;

    [SerializeField]
    private float FieldOffsetY;

    float _HitPoints;
    EnemyTurret _Turret;

    List<Vector2Int> _Path;
    Vector3 _Destination;
    public MovementState State { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _Turret = gameObject.GetComponentInChildren<EnemyTurret>();
        _HitPoints = HitPoints;
        _Path = new List<Vector2Int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (State == MovementState.Moving && Rotate(_Destination) && Move(_Destination))
        {
            if (_Path.Count > 0)
            {
                _Destination = new Vector3(_Path[0].x + FieldOffsetX, _Path[0].y + FieldOffsetY);
                _Path.RemoveAt(0);
            }
            else
            {
                State = MovementState.Idle;
            }
        }
    }

    public void ApplyDamage(float Damage, Vector3 Source)
    {
        _HitPoints -= Damage;
        _Turret.GotHit(Source, this);
    }

    public void MovePosition(Vector3 Position, EnemyTurret Sender)
    {
        if (State == MovementState.Idle && Sender == _Turret)
        {
            _Path = GameController.Instance.PathFinder.FindPath(transform.position, Position);
            if (_Path != null && _Path.Count > 0)
            {
                _Destination = new Vector3(_Path[0].x + FieldOffsetX, _Path[0].y + FieldOffsetY);
                _Path.RemoveAt(0);
                State = MovementState.Moving;
            }
        }
    }

    public void StopMovements(EnemyTurret Sender)
    {
        if (State == MovementState.Moving && Sender == _Turret)
        {
            if (_Path != null && _Path.Count > 0)
            {
                _Path.Clear();
            }
            State = MovementState.Idle;
        }
    }

    bool Rotate(Vector3 Direction)
    {
        Vector3 difference = Direction - transform.position;
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
        transform.position = Vector3.MoveTowards(transform.position, Position, Speed * Time.deltaTime);
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
