using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed;

    [SerializeField]
    private Transform LeftBulletSpawnPoint;

    [SerializeField]
    private Transform RightBulletSpawnPoint;

    [SerializeField]
    private float ReloadingTime;

    private bool _isReloadedLeft, _isReloadedRight;
    private float _ReloadingLeft, _ReloadingRight;

    // Use this for initialization
    void Start()
    {
        _isReloadedLeft = true;
        _isReloadedRight = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // This will calculate the distance between the mouse in the game and the position of the tank turret

        difference.Normalize();
        // This returns simplified values which makes it easier to work with

        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90;
        // This calculates the angle between the mouse and the turret by using the values derives from the difference calculation.

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, angle), RotationSpeed * Time.deltaTime);
        // This will rotate the turret towards the calculated angle over time. Tweaking the multiplication value will state how quickly or slowly it will rotate.

        if (Input.GetButton("Fire1") && _isReloadedLeft)
        {
            _isReloadedLeft = false;
            GameController.Instance.BulletPool.ActivateProjectile(LeftBulletSpawnPoint);
            _ReloadingLeft = ReloadingTime;
        }
        if (Input.GetButton("Fire2") && _isReloadedRight)
        {
            _isReloadedRight = false;
            GameController.Instance.BulletPool.ActivateProjectile(RightBulletSpawnPoint);
            _ReloadingRight = ReloadingTime;
        }
    }

    private void LateUpdate()
    {
        if (!_isReloadedLeft)
        {
            _ReloadingLeft -= Time.deltaTime;
            if (_ReloadingLeft <= 0)
            {
                _isReloadedLeft = true;
            }
        }
        if (!_isReloadedRight)
        {
            _ReloadingRight -= Time.deltaTime;
            if (_ReloadingRight <= 0)
            {
                _isReloadedRight = true;
            }
        }
    }
}
