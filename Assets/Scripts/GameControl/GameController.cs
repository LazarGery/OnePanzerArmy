using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    Transform BulletPoolParent;

    [SerializeField]
    int BulletPoolMaxLimit;

    public IProjectiles BulletPool { get; private set; }

    public static GameController Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            BulletPool = new ProjectilePool(BulletPoolMaxLimit, BulletPoolParent, Bullet);
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
