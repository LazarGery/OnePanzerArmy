using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Represents the time duration while the animation is played on the scene
    [SerializeField]
    private float AnimationLength;

    float _AnimationTimer;

    // This gameobject's role is to play the Explosion animation for the given time duration whenever it's needed
    // thus the initial state is disabled and the Explosion animation starts whenever it's activated
    void Awake()
    {
        gameObject.SetActive(false);
    }

    // Activates the gameobject when the Explosion animation needs to be played on the scene for the given time duration
    void OnEnable()
    {
        _AnimationTimer = AnimationLength;
    }

    // The animationTimer which keeps track of how much time left for the Explosion animation
    // can be reduced after every important Update finished on the scene
    // If the animationTimer reached 0 then the Explosion gameobject can be disabled
    // thus the animation ceased to being played until the next activation
    void LateUpdate()
    {
        _AnimationTimer -= Time.deltaTime;
        if (_AnimationTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
