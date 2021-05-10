using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    Transform BulletPoolParent;

    [SerializeField]
    int BulletPoolMaxLimit;

    public IProjectiles BulletPool { get; private set; }

    [SerializeField]
    private Tilemap BorderTilemap;

    [SerializeField]
    private Tilemap BuildingsTilemap;

    public MapGrid Map { get; private set; }

    public IPathFind PathFinder { get; private set; }
    
    public static GameController Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            BulletPool = new ProjectilePool(BulletPoolMaxLimit, BulletPoolParent, Bullet);
            Map = new MapGrid(BorderTilemap, BuildingsTilemap);
            PathFinder = new PathFinder(Map);
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
