using UnityEngine;

// Base class for all projectiles, will be refactored with ECS later
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;

    [SerializeField] private Vector2 _direction;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.up * (_speed * Time.deltaTime));
    }
    
    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
        transform.up = _direction;
    }
}
