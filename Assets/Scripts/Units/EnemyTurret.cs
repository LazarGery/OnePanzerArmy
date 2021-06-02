using System.Collections.Generic;
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
    private float AimDistanceBuildings;

    [SerializeField]
    private float AimDistanceAllies;

    [SerializeField]
    private float SafeDistance;

    [SerializeField]
    private float SenseDistance;

    [SerializeField]
    private float CoverRequestTimer;

    [SerializeField]
    private float CoverCheckTimer;

    [SerializeField]
    private float PositioningTimer;

    [SerializeField]
    private float PlayerModelHeight;

    [SerializeField]
    private float PlayerModelWidth;

    [SerializeField]
    private float EnemyModelHeight;

    [SerializeField]
    private float EnemyModelWidth;

    Enemy _Unit;
    Transform _Player;
    Vector3[] _PlayerTransformPositions;
    Vector3 _Target;
    bool _isReloaded;
    float _ReloadingTimer;
    LayerMask _BuildingsLayer;
    LayerMask _UnitsLayer;

    CoverPoint _Cover;
    bool _isCoverRequested;
    float _CoverRequestTimer;
    float _CoverCheckTimer;
    bool _isMovingToCover, _isMovingToFront;
    float _PositioningTimer;

    List<Projectile> _ApproachingBullets;
    bool _isHiding;

    public FSMState State { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").transform;
        _Unit = gameObject.GetComponentInParent<Enemy>();
        _BuildingsLayer = GameController.Instance.Map.Buildings_Layer;
        _UnitsLayer = GameController.Instance.Map.Units_Layer;
        _ApproachingBullets = new List<Projectile>();
        _isReloaded = true;
        
        SetPlayerTransformPositions(PlayerModelHeight, PlayerModelWidth);
    }

    void SetPlayerTransformPositions(float height, float width)
    {
        _PlayerTransformPositions = new Vector3[4];
        _PlayerTransformPositions[0] = new Vector3(height / 2, width / 2);
        _PlayerTransformPositions[1] = new Vector3(-1 * height / 2, width / 2);
        _PlayerTransformPositions[2] = new Vector3(height / 2, -1 * width / 2);
        _PlayerTransformPositions[3] = new Vector3(-1 * height / 2, -1 * width / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (_Player != null)
        {
            if (SeeableUnitInFOV(transform.position, transform.rotation, _Player))
            {
                if (RotateTurret(_Target) && Aim(transform.position, transform.rotation, _Target))
                {
                    Shoot();
                }

                if (State == FSMState.Idle || State == FSMState.Alarmed)
                {
                    State = FSMState.Fight;
                    if (_Unit.State == MovementState.Moving)
                    {
                        _Unit.StopMovements(this);
                    }
                }
                else if (State == FSMState.Fight)
                {
                    if (!_isCoverRequested || _CoverRequestTimer >= CoverRequestTimer)
                    {
                        _Cover = GameController.Instance.Cover.GetCover(transform.position, _Target, ViewRange, AimDistanceBuildings);
                        if (_Cover != null)
                        {
                            _isCoverRequested = false;
                            if (_Unit.State == MovementState.Moving)
                            {
                                _Unit.StopMovements(this);
                            }
                            _Unit.MoveToCover(_Cover.Position, _Cover.Front, this);
                            State = FSMState.GoCover;
                        }
                        else
                        {
                            _isCoverRequested = true;
                            _CoverRequestTimer = 0;
                        }
                    }
                }
                else if (State == FSMState.InCover)
                {
                    Behavior_InCover(true);
                }
            }
            else if (State == FSMState.Fight || State == FSMState.Alarmed)
            {
                if (RotateTurret(_Target) && _Unit.State == MovementState.Idle)
                {
                    _Unit.MoveLocation(_Target, this);
                    State = FSMState.Alarmed;
                }
            }
            else if (State == FSMState.InCover)
            {
                Behavior_InCover(false);
            }

            if (State == FSMState.GoCover || State == FSMState.InCover)
            {
                if (_CoverCheckTimer > CoverCheckTimer && !_isHiding)
                {
                    CheckCover();
                    _CoverCheckTimer = 0;
                }
            }
        }
    }

    void Behavior_InCover(bool _isPlayerInSight)
    {
        if (_isPlayerInSight)
        {
            if (_isMovingToFront)
            {
                _Unit.StopPositioning(this);
                _isMovingToFront = false;
            }
            else if (_PositioningTimer > PositioningTimer && !_isMovingToCover)
            {
                _Unit.MovePosition(_Cover.Position, this);
                _isMovingToCover = true;
                _PositioningTimer = 0;
            }
        }
        else
        {
            if (!_isHiding)
            {
                if (_isMovingToCover)
                {
                    _Unit.StopPositioning(this);
                    _isMovingToCover = false;
                }
                if (!_isMovingToFront)
                {
                    _Unit.MovePosition(_Cover.Front, this);
                    _isMovingToFront = true;
                }
            }
            RotateTurret(_Target);
        }
    }

    void CheckCover()
    {
        bool success = true;
        bool isStopPositioning = false;
        if (!SeeableUnit(_Cover.Front, _Player))
        {
            do
            {
                success = _Cover.MoveForward();
            }
            while (success && !SeeableUnit(_Cover.Front, _Player));
            isStopPositioning = true;
        }
        else if (SeeableUnit(_Cover.Position, _Player))
        {
            do
            {
                success = _Cover.MoveBackward();
            }
            while (success && SeeableUnit(_Cover.Position, _Player));
            isStopPositioning = true;
        }

        if (!success)
        {
            GameController.Instance.Cover.FreeCover(_Cover);
            _Cover = null;
            State = FSMState.Alarmed;
            _Unit.StopMovements(this);
        }
        if (isStopPositioning)
        {
            _Unit.StopPositioning(this);
            _isMovingToCover = false;
            _isMovingToFront = false;
            _PositioningTimer = PositioningTimer;
        }        
    }

    public void GotHit(Vector3 Position, Enemy Sender)
    {
        if (_Unit != null && Sender == _Unit)
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

    public void ReachedCover(Enemy Sender)
    {
        if (_Unit != null && Sender == _Unit)
        {
            State = FSMState.InCover;
        }
    }

    public void ReachedPosition(Enemy Sender)
    {
        if (_Unit != null && Sender == _Unit)
        {
            _isMovingToCover = false;
            _isMovingToFront = false;
        }
    }

    public void IncomingBullet(Projectile Bullet)
    {
        if (Bullet != null && SeeableInFOV(transform.position, transform.rotation, Bullet.transform.position))
        {
            if (_ApproachingBullets != null && !_ApproachingBullets.Contains(Bullet))
            {
                _ApproachingBullets.Add(Bullet);
            }
            if (State == FSMState.InCover)
            {
                RaycastHit2D[] actuals = Physics2D.RaycastAll(Bullet.transform.position, Bullet.transform.rotation * Vector3.up, Vector3.Distance(Bullet.transform.position, _Unit.transform.position), _UnitsLayer);
                int index = 0;
                while (index < actuals.Length && actuals[index].transform != _Unit.transform)
                {
                    index += 1;
                }
                if (index < actuals.Length)
                {
                    if (Vector3.Distance(_Unit.transform.position, _Cover.Position) > 0.5f || _Cover.MoveBackward())
                    {
                        _Unit.StopPositioning(this);
                        _isMovingToFront = false;
                        _Unit.MovePosition(_Cover.Position, this);
                        _isMovingToCover = true;
                        _PositioningTimer = 0;
                        _isHiding = true;
                    }
                }
            }
        }
    }

    public void IncomingBulletExploded(Projectile Bullet)
    {
        if (Bullet != null && _ApproachingBullets != null && _ApproachingBullets.Contains(Bullet))
        {
            _ApproachingBullets.Remove(Bullet);
            if (_ApproachingBullets.Count == 0)
            {
                _isHiding = false;
            }
        }
    }

    bool Seeable(Vector3 Position, Vector3 Target)
    {
        float distance = Vector3.Distance(Position, Target);
        if (distance <= ViewRange)
        {
            Vector3 direction = Target - Position;
            if (!Physics2D.CircleCast(Position, AimDistanceBuildings, direction, distance, _BuildingsLayer))
            {
                return (true);
            }
        }
        return (false);
    }

    bool SeeableInFOV(Vector3 Position, Quaternion Rotation, Vector3 Target)
    {
        float distance = Vector3.Distance(Position, Target);
        if (distance <= ViewRange)
        {
            Vector3 direction = (Target - Position).normalized;
            if ((Vector3.Angle(Rotation * Vector3.up, direction) < ViewAngle / 2) || distance <= SenseDistance)
            {
                if (!Physics2D.CircleCast(Position, AimDistanceBuildings, direction, distance, _BuildingsLayer))
                {
                    return (true);
                }
            }
        }
        return (false);
    }

    bool SeeableUnit(Vector3 Position, Transform Target)
    {
        if (Seeable(Position, Target.position))
        {
            return (true);
        }
        int index = 0;
        while (index < _PlayerTransformPositions.Length)
        {
            Vector3 offset = Target.rotation * _PlayerTransformPositions[index];
            if (Seeable(Position, Target.position + offset))
            {
                return (true);
            }
            index += 1;
        }
        return (false);
    }

    bool SeeableUnitInFOV(Vector3 Position, Quaternion Rotation, Transform Target)
    {
        if (Target != null)
        {
            if (SeeableInFOV(Position, Rotation, Target.position))
            {
                if (Target == _Player)
                {
                    _Target = Target.position;
                }
                return (true);
            }
            int index = 0;
            while (index < _PlayerTransformPositions.Length)
            {
                Vector3 offset = Target.rotation * _PlayerTransformPositions[index];
                if (SeeableInFOV(Position, Rotation, Target.position + offset))
                {
                    if (Target == _Player)
                    {
                        _Target = Target.position + offset;
                    }
                    return (true);
                }
                index += 1;
            }
        }
        return (false);
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
            if (!Physics2D.CircleCast(Position, AimDistanceBuildings, Rotation * Vector3.up, distance, _BuildingsLayer))
            {
                RaycastHit2D[] units = Physics2D.CircleCastAll(Position, AimDistanceAllies, Rotation * Vector3.up, distance, _UnitsLayer);
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
        if (_isCoverRequested)
        {
            _CoverRequestTimer += Time.deltaTime;
        }
        if (State == FSMState.GoCover)
        {
            _CoverCheckTimer += Time.deltaTime;
        }
        else if (State == FSMState.InCover)
        {
            _CoverCheckTimer += Time.deltaTime;
            _PositioningTimer += Time.deltaTime;
        }
    }
}
