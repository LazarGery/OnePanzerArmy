using UnityEngine;

class ProjectilePool : IProjectiles
{
    // For the reuse of Projectile gameobjects instead of repeated Instantiate and Destroy
    // The maximum capacity of the pool is known by parameter
    // so an Array can be used for reaching the requested elements faster than in the case of using List
    GameObject[] _Pool;
    int _LastItemIndex;
    
    // The parent gameobject of all Projectile gameobjects in the Editor Hierarchy
    Transform _Parent;
    
    // Projectile Prefab reference to create new Projectile gameobjects
    GameObject _Bullet;

    public ProjectilePool(int Limit, Transform Parent, GameObject Bullet)
    {
        _Pool = new GameObject[Limit];
        _LastItemIndex = -1;
        _Parent = Parent;
        _Bullet = Bullet;
    }

    // Take out the latest Projectile gameobject from the pool if it's possible
    // Otherwise create a new one for further use and put it under the parent gameobject in the Editor Hierarchy
    // Sets the position and rotation of the Projectile to be the same as the Turret gameobject which launched it
    // Activates the Projectile gameobject so it can work independently on the scene
    // until it finishes it's task (explode on collision or going out of the map) and returns to the pool
    public void ActivateProjectile(Transform Turret)
    {
        if (_LastItemIndex > -1)
        {
            GameObject actual = _Pool[_LastItemIndex];
            _Pool[_LastItemIndex] = null;
            _LastItemIndex -= 1;
            actual.transform.position = Turret.position;
            actual.transform.rotation = Turret.rotation;
            actual.SetActive(true);
        }
        else
        {
            GameObject.Instantiate(_Bullet, Turret.position, Turret.rotation, _Parent);
        }
    }

    // Checks if the input gameobject counts as a Projectile (if not then just ignore it)
    // If the pool's maximum capacity hasn't reached then deactivates the Projectile and put it into the pool
    // Otherwise destroyes the Projectile to avoid having to many GameObjects on the scene unnecessarily
    public void ReturnProjectile(GameObject Bullet)
    {
        if (Bullet != null && Bullet.GetComponentInChildren<Projectile>() != null)
        {
            if (_LastItemIndex < _Pool.Length - 1)
            {
                Bullet.SetActive(false);
                _LastItemIndex += 1;
                _Pool[_LastItemIndex] = Bullet;
            }
            else
            {
                GameObject.Destroy(Bullet);
            }
        }
    }
}
