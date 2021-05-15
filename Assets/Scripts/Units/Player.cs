using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float Acceleration;

    [SerializeField]
    private float MaxForwardSpeed;

    [SerializeField]
    private float MaxBackwardSpeed;

    [SerializeField]
    private float SpeedDecay;

    [SerializeField]
    private float TurnSpeed;

    [SerializeField]
    private float PushBackSpeedRatioOnCollisionWithWalls;

    [SerializeField]
    private float HitPoints;

    float _Speed;
    bool _isSlowing;
    float _HitPoints;

    // Use this for initialization
    void Start()
    {
        _Speed = 0;
        _HitPoints = HitPoints;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isSlowing = true;

        if (Input.GetAxisRaw("Horizontal") == -1)
        {
            transform.Rotate(Vector3.forward * TurnSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetAxisRaw("Horizontal") == 1)
        {
            transform.Rotate(Vector3.back * TurnSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetAxisRaw("Vertical") == 1)
        {
            if (_Speed < MaxForwardSpeed)
            {
                _Speed += Acceleration * Time.fixedDeltaTime;
                if (_Speed > MaxForwardSpeed) _Speed = MaxForwardSpeed;
            }
            _isSlowing = false;
        }
        if (Input.GetAxisRaw("Vertical") == -1)
        {
            if (_Speed > -1 * MaxBackwardSpeed)
            {
                _Speed -= Acceleration * Time.fixedDeltaTime;
                if (_Speed < -1 * MaxBackwardSpeed) _Speed = -1 * MaxBackwardSpeed;
            }
            _isSlowing = false;
        }

        transform.Translate(Vector2.up * _Speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (_HitPoints <= 0)
        {
            Destroy(gameObject);
        }
        else if (_isSlowing)
        {
            if (_Speed > 0)
            {
                _Speed -= SpeedDecay * Time.deltaTime;
                if (_Speed < 0) _Speed = 0;
            }
            else if (_Speed < 0)
            {
                _Speed += SpeedDecay * Time.deltaTime;
                if (_Speed > 0) _Speed = 0;
            }
        }
    }

    public void ApplyDamage(float Damage)
    {
        _HitPoints -= Damage;
    }

    //Adds a pushback effect when colliding with Buildings or the Border of the map
    public void OnCollisionEnter2D(Collision2D Collision)
    {
        if (Collision.gameObject.CompareTag("Buildings") || Collision.gameObject.CompareTag("Border"))
        {
            _Speed = PushBackSpeedRatioOnCollisionWithWalls * -_Speed;
        }
    }

    //To avoid the glitch when the player can slide after colliding with Buildings or the Border of the map
    public void OnCollisionStay2D(Collision2D Collision)
    {
        if (Collision.gameObject.CompareTag("Buildings") || Collision.gameObject.CompareTag("Border"))
        {
            _Speed = 0;
        }
    }
}
