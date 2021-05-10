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

    float _Speed;
    bool _isSlowing;

    // Use this for initialization
    void Start()
    {
        _Speed = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isSlowing = true;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * TurnSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * TurnSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (_Speed < MaxForwardSpeed)
            {
                _Speed += Acceleration * Time.fixedDeltaTime;
                if (_Speed > MaxForwardSpeed) _Speed = MaxForwardSpeed;
            }
            _isSlowing = false;
        }
        if (Input.GetKey(KeyCode.S))
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
        if (_isSlowing)
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