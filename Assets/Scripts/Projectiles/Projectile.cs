using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // The Projectile doesn't use the Unity Engine built-in Physics System (for performance gain)
    // Instead of simulating the projectile with Physics (Physics effects when collides, velocity etc.)
    // it practically teleports the projectile in small distances through the map in every frame until it explodes
    // If the projectile enters into another objects collider then responds according to the reached object
    [SerializeField]
    private float BulletSpeed;

    // The UnitsLayer contains the positions of all the enemy tanks and the player
    [SerializeField]
    private LayerMask UnitsLayer;
    
    // The range around the centre of the projectile where the explosion will cause damage
    [SerializeField]
    private float ExplosionRadius;

    // The time until the projectile explodes by itself without colliding with anything
    [SerializeField]
    private float ExplosionTimer;

    // The damage caused by the explosion of the projectile
    [SerializeField]
    private float ExplosionDamage;

    // The relative size of the Explosion animation compared to the projectile
    // 1 - means the same size as the projectile
    // It only affects the animation, for the range of damage use ExplosionRadius
    [SerializeField]
    private float ExplosionScale;

    // Explosion Prefab reference for instantiating the Explosion gameobject
    // which playes the Explosion animation when activated
    // until the Explosion's given time duration ran out and it turns off by itself
    [SerializeField]
    private GameObject Explosion;

    [SerializeField]
    private float EstimationTimer;

    [SerializeField]
    private float EstimationDistance;

    float _ExplosionTimer;
    float _EstimationTimer;
    GameObject _Explosion;
    Vector3 _Destination;

    List<EnemyTurret> _Targets;
    
    // When the enemies don't know the players location then they won't shoot projectiles randomly
    // Thus if any enemy unit gets hit by a projectile that means they can track down the player by it
    Vector3 _StartingPosition;

    // The current state of the projectile
    // False - the projectile still needs to be moved on Update and the colliders needs to be handled
    // True - only needs to wait until the Explosion gameobject finishes its animation and disables itself
    //        after that the current gameobject can also return to the pool for reuse
    bool _isExploded;

    void Awake()
    {
        _Explosion = Instantiate(Explosion, transform);
        _Explosion.transform.localScale = new Vector3(ExplosionScale, ExplosionScale, 1);
        _StartingPosition = transform.position;
        _Targets = new List<EnemyTurret>();
        _EstimationTimer = EstimationTimer;
    }

    // Activates when the bullet is reused by another unit and sets the necessary values to default
    void OnEnable()
    {
        GetComponent<Renderer>().enabled = true;
        _StartingPosition = transform.position;
        _Destination = transform.position + (transform.rotation * Vector3.up * BulletSpeed * ExplosionTimer);
        _EstimationTimer = EstimationTimer;
        _ExplosionTimer = 0;
        _isExploded = false;
        _Targets.Clear();
    }

    // Update is called once per frame
    // Moves the projectile on the map in every frame until the gameobject explodes through collision or times up
    // After explosion it waits for the animation to be finished to send back the gameobject to the pool for reuse
    void Update()
    {
        if (!_isExploded)
        {
            if (_ExplosionTimer > ExplosionTimer)
            {
                Explode();
            }
            else
            {
                transform.Translate(Vector2.up * BulletSpeed * Time.deltaTime);
                if (_EstimationTimer > EstimationTimer)
                {
                    RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, _Destination, EstimationDistance);
                    int index = 0;
                    while (index < obstacles.Length)
                    {
                        if (obstacles[index].collider.gameObject.CompareTag("Buildings"))
                        {
                            index = obstacles.Length;
                        }
                        else if (obstacles[index].collider.gameObject.CompareTag("Enemy"))
                        {
                            EnemyTurret actual = obstacles[index].collider.gameObject.GetComponentInChildren<EnemyTurret>();
                            if (!_Targets.Contains(actual))
                            {
                                _Targets.Add(actual);
                            }
                            actual.IncomingBullet(this);
                        }
                        index += 1;
                    }
                    _EstimationTimer = 0;
                }
            }
        }
        else if (!_Explosion.activeSelf)
        {
            GameController.Instance.BulletPool.ReturnProjectile(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (!_isExploded)
        {
            _ExplosionTimer += Time.deltaTime;
            _EstimationTimer += Time.deltaTime;
        }
    }

    // Manages the gameobjects from the UnitsLayer that has been reached by the range of the explosion
    // Deactivates the projectile on the scene(renderer, movement, timer) and activates the Explosion animation
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius, UnitsLayer);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<Enemy>().ApplyDamage(ExplosionDamage, _StartingPosition);
            }
            else if (col.CompareTag("Player"))
            {
                col.gameObject.GetComponent<Player>().ApplyDamage(ExplosionDamage);
            }
        }

        GetComponent<Renderer>().enabled = false;
        _Explosion.SetActive(true);
        _isExploded = true;

        int index = _Targets.Count - 1;
        while (index >= 0)
        {
            _Targets[index].IncomingBulletExploded(this);
            _Targets.RemoveAt(index);
            index -= 1;
        }
    }

    // When the Projectile collides with (only handles colliders before explosion):
    // Border - the gameobject ran out of the map, so it can be sent back to the pool for reuse
    // Buildings, Enemy, Player, Projectile - the gameobject collided with valid targets so it will explode
    // other colliders are ignored
    // (EnemyTurret and PlayerTurret mostly resides on top of the tanks so it is not necessary to check them independently)
    void OnCollisionEnter2D(Collision2D Collision)
    {
        if (!_isExploded)
        {
            if (Collision.gameObject.CompareTag("Border"))
            {
                GameController.Instance.BulletPool.ReturnProjectile(gameObject);
            }
            else if (Collision.gameObject.CompareTag("Buildings") ||
                    Collision.gameObject.CompareTag("Enemy") ||
                    Collision.gameObject.CompareTag("Player") ||
                    Collision.gameObject.CompareTag("Projectile"))
            {
                Explode();
            }
        }
    }
}
