using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Tilemap TileMapBorder;

    [SerializeField]
    private float ZoomSpeed;

    [SerializeField]
    private float MaximumZoom;

    [SerializeField]
    private float MinimumZoom;

    Transform _Target;
    Camera _Camera;
    float _Min_X, _Max_X, _Min_Y, _Max_Y;
    Vector3 _MinTile, _MaxTile;

    // Use this for initialization
    void Start()
    {
        _Target = GameObject.FindGameObjectWithTag("Player").transform;
        _Camera = Camera.main;

        _MinTile = TileMapBorder.CellToWorld(TileMapBorder.cellBounds.min);
        if (_MinTile.x >= 0) _Min_X = _MinTile.x - 1; else _Min_X = _MinTile.x + 1;
        if (_MinTile.y >= 0) _Min_Y = _MinTile.y - 1; else _Min_Y = _MinTile.y + 1;
        _MinTile = new Vector3(_Min_X, _Min_Y, 0);

        _MaxTile = TileMapBorder.CellToWorld(TileMapBorder.cellBounds.max);
        if (_MaxTile.x >= 0) _Max_X = _MaxTile.x - 1; else _Max_X = _MaxTile.x + 1;
        if (_MaxTile.y >= 0) _Max_Y = _MaxTile.y - 1; else _Max_Y = _MaxTile.y + 1;
        _MaxTile = new Vector3(_Max_X, _Max_Y, 0);
        
        SetLimits();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Q) && _Camera.orthographicSize > MinimumZoom)
        {
            float size = _Camera.orthographicSize;
            size -= Time.fixedDeltaTime * ZoomSpeed;
            if (size >= MinimumZoom)
            {
                _Camera.orthographicSize = size;
            }
            else
            {
                _Camera.orthographicSize = MinimumZoom;
            }
            SetLimits();
        }
        if (Input.GetKey(KeyCode.E) && _Camera.orthographicSize < MaximumZoom)
        {
            float size = _Camera.orthographicSize;
            size += Time.fixedDeltaTime * ZoomSpeed;
            if (size <= MaximumZoom)
            {
                _Camera.orthographicSize = size;
            }
            else
            {
                _Camera.orthographicSize = MaximumZoom;
            }
            SetLimits();
        }
    }
    
    void SetLimits()
    {
        float height = 2f * _Camera.orthographicSize;
        float width = height * _Camera.aspect;

        _Min_X = _MinTile.x + width / 2;
        _Max_X = _MaxTile.x - width / 2;

        _Min_Y = _MinTile.y + height / 2;
        _Max_Y = _MaxTile.y - height / 2;
    }

    void LateUpdate()
    {
        if (_Target != null)
        {
            transform.position = new Vector3(Mathf.Clamp(_Target.position.x, _Min_X, _Max_X), Mathf.Clamp(_Target.position.y, _Min_Y, _Max_Y), -10);
        }
    }
}
