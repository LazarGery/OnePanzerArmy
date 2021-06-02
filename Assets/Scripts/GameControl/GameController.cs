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

    [SerializeField]
    private LayerMask BuildingsLayer;

    [SerializeField]
    private LayerMask UnitsLayer;

    [SerializeField]
    private float FieldOffsetX;

    [SerializeField]
    private float FieldOffsetY;

    public IPathFind PathFinder { get; private set; }

    [SerializeField]
    private float Cover_SafeDistance;

    [SerializeField]
    private float Cover_MaxDistance;

    public ICover Cover { get; private set; }

    public static GameController Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            BulletPool = new ProjectilePool(BulletPoolMaxLimit, BulletPoolParent, Bullet);
            Map = new MapGrid(BorderTilemap, BuildingsTilemap, BuildingsLayer, UnitsLayer, FieldOffsetX, FieldOffsetY);
            PathFinder = new PathFinder(Map);
            Cover = new Covers(Map, BuildingsLayer, FieldOffsetX, FieldOffsetY, Cover_SafeDistance, Cover_MaxDistance);
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
