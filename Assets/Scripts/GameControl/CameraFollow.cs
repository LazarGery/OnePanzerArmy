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

    [SerializeField]
    private float MoveSpeed;

    [SerializeField]
    private float TapTime;

    Transform _Target;
    Camera _Camera;
    float _Min_X, _Max_X, _Min_Y, _Max_Y;
    Vector3 _MinTile, _MaxTile;
    
    Vector2Int _ScreenSize;
    float _OffsetX, _OffsetY;
    float _OffsetX_Limit, _OffsetY_Limit;
    float _TapTimer;
    bool _isCameraSlideButtonPressed;

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

        _ScreenSize = new Vector2Int(Screen.width, Screen.height);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Input.GetButton("ZoomIn") && _Camera.orthographicSize > MinimumZoom)
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
        if (Input.GetButton("ZoomOut") && _Camera.orthographicSize < MaximumZoom)
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
        if (Input.GetButton("CameraSlide"))
        {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.y > _ScreenSize.y * 0.95f && _OffsetY < _OffsetY_Limit)
            {
                _OffsetY += Time.deltaTime * MoveSpeed;
            }
            else if (mousePosition.y < _ScreenSize.y * 0.05f && _OffsetY > _OffsetY_Limit * -1)
            {
                _OffsetY -= Time.deltaTime * MoveSpeed;
            }

            if (mousePosition.x > _ScreenSize.x * 0.95f && _OffsetX < _OffsetX_Limit)
            {
                _OffsetX += Time.deltaTime * MoveSpeed;
            }
            else if (mousePosition.x < _ScreenSize.x * 0.05f && _OffsetX > _OffsetX_Limit * -1)
            {
                _OffsetX -= Time.deltaTime * MoveSpeed;
            }

            if (!_isCameraSlideButtonPressed)
            {
                _isCameraSlideButtonPressed = true;
            }
            _TapTimer += Time.deltaTime;
        }
    }

    private void Update()
    {
        if (_isCameraSlideButtonPressed && Input.GetButtonUp("CameraSlide"))
        {
            _isCameraSlideButtonPressed = false;
            if (_TapTimer < TapTime)
            {
                _OffsetX = 0;
                _OffsetY = 0;
            }
            _TapTimer = 0.0f;
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

        _OffsetX_Limit = _Camera.orthographicSize * _Camera.aspect - 1;
        _OffsetY_Limit = _Camera.orthographicSize - 1;

        if (_OffsetX > _OffsetX_Limit)
        {
            _OffsetX = _OffsetX_Limit;
        }
        else if (_OffsetX < _OffsetX_Limit * -1)
        {
            _OffsetX = _OffsetX_Limit * -1;
        }

        if (_OffsetY > _OffsetY_Limit)
        {
            _OffsetY = _OffsetY_Limit;
        }
        else if (_OffsetY < _OffsetY_Limit * -1)
        {
            _OffsetY = _OffsetY_Limit * -1;
        }
    }

    void LateUpdate()
    {
        if (_Target != null)
        {
            transform.position = new Vector3(Mathf.Clamp(_Target.position.x + _OffsetX, _Min_X, _Max_X), Mathf.Clamp(_Target.position.y + _OffsetY, _Min_Y, _Max_Y), -10);
        }
    }
}
