using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float HitPoints;

    float _HitPoints;
    EnemyTurret _Turret;
    
    // Start is called before the first frame update
    void Start()
    {
        _Turret = gameObject.GetComponentInChildren<EnemyTurret>();
        _HitPoints = HitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float Damage, Vector3 Source)
    {
        _HitPoints -= Damage;
        _Turret.GotHit(Source, this);
    }

    private void LateUpdate()
    {
        if (_HitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
