using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed;

    // Use this for initialization
    void Start()
    {
        
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
    }
}