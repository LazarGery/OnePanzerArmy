using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed;
    
    [SerializeField]
    private Transform MiddleBulletSpawnpoint;

    [SerializeField]
    private float ReloadingTime;

    [SerializeField]
    private float ViewRange;

    [SerializeField]
    [Range(0, 360)]
    private float ViewAngle;

    [SerializeField]
    private LayerMask BuildingsLayer;

    [SerializeField]
    private LayerMask UnitsLayer;

    [SerializeField]
    private float AimDistanceBuildings;

    [SerializeField]
    private float AimDistanceAllies;

    [SerializeField]
    private float SafeDistance;

    [SerializeField]
    private float SenseDistance;

    Enemy _Unit;
    Transform _Player;
    Vector3 _Target;
    bool _isReloaded;
    float _ReloadingTimer;

    public FSMState State { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").transform;
        _Unit = gameObject.GetComponentInParent<Enemy>();
        _isReloaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ScanFOV(_Player.position))
        {
            if (RotateTurret(_Player.position) && Aim(transform.position, transform.rotation, _Player.position))
            {
                Shoot();
            }
            _Target = _Player.position;

            if (State == FSMState.Idle || State == FSMState.Alarmed)
            {
                State = FSMState.Fight;
                if (_Unit.State == MovementState.Moving)
                {
                    _Unit.StopMovements(this);
                }
            }
        }
        else if (State != FSMState.Idle && RotateTurret(_Target) && _Unit.State != MovementState.Moving)
        {
            _Unit.MovePosition(_Target, this);
            State = FSMState.Alarmed;
        }
    }

    public void GotHit(Vector3 Position, Enemy Sender)
    {
        if (Sender == _Unit)
        {
            if (State == FSMState.Idle)
            {
                _Target = Position;
                State = FSMState.Alarmed;
            }
            else if (State == FSMState.Alarmed)
            {
                _Target = Position;
                if (_Unit.State == MovementState.Moving)
                {
                    _Unit.StopMovements(this);
                }
            }
        }
    }

    // Checks if the given Position is in the field of view of the unit
    bool ScanFOV(Vector3 Position)
    {
        float distance = Vector3.Distance(transform.position, Position);
        if (distance <= ViewRange)
        {
            Vector3 direction = (Position - transform.position).normalized;
            if ((Vector3.Angle(transform.up, direction) < ViewAngle / 2) || distance <= SenseDistance)
            {
                if (!Physics2D.CircleCast(transform.position, AimDistanceBuildings, direction, distance, BuildingsLayer))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Rotates the turret towards the given Direction in the current frame
    // Returns True - if the necessary angle reached
    bool RotateTurret(Vector3 Direction)
    {
        Vector3 difference = Direction - transform.position;
        difference.Normalize();
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90;
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), RotationSpeed * Time.deltaTime);
        if (Mathf.Abs(transform.rotation.eulerAngles.z - nextRotation.eulerAngles.z) > 0.1f)
        {
            transform.rotation = nextRotation;
            return false;
        }
        return true;
    }

    // Checks if the bullets shot from the given Position and Rotation can reach the Target
    // Returns True - if the Target is in the ViewRange of the unit AND
    //               outside of the SafeDistance from the unit(to avoid damaging itself by shooting too close) AND
    //               if the bullets can reach the Player without colliding with any Buildings and other Enemy units
    bool Aim(Vector3 Position, Quaternion Rotation, Vector3 Target)
    {
        float distance = Vector3.Distance(Position, Target);
        if (distance >= SafeDistance && distance <= ViewRange)
        {
            if (!Physics2D.CircleCast(Position, AimDistanceBuildings, Rotation * Vector3.up, distance, BuildingsLayer))
            {
                RaycastHit2D[] units = Physics2D.CircleCastAll(Position, AimDistanceAllies, Rotation * Vector3.up, distance, UnitsLayer);
                int index = 0;
                while (index < units.Length)
                {
                    if (units[index].collider.CompareTag("Enemy"))
                    {
                        if (units[index].collider.gameObject != transform.parent.gameObject)
                        {
                            return false;
                        }
                    }
                    else if (units[index].collider.CompareTag("Player"))
                    {
                        return true;
                    }
                    index++;
                }
            }
        }
        return false;
    }

    void Shoot()
    {
        if (_isReloaded)
        {
            GameController.Instance.BulletPool.ActivateProjectile(MiddleBulletSpawnpoint);
            _isReloaded = false;
            _ReloadingTimer = ReloadingTime;
        }
    }

    private void LateUpdate()
    {
        if (!_isReloaded)
        {
            _ReloadingTimer -= Time.deltaTime;
            if (_ReloadingTimer <= 0)
            {
                _isReloaded = true;
            }
        }
    }
}
