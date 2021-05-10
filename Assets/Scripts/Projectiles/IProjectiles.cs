using UnityEngine;

public interface IProjectiles
{
    void ActivateProjectile(Transform Turret);
    void ReturnProjectile(GameObject Bullet);
}
