using System;
using Unity.Collections;
using UnityEngine;

// Fires projectiles, can be triggered by events (input from player or enemy AI or something else)
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private float _cooldown = 0.5f;
    
    [SerializeField]
    private float _currCooldown;
    
    public void Fire(Vector2 direction)
    {
        if(_currCooldown > 0)
            return;
        
        GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
        projectile.SendMessage("SetDirection", direction);
        _currCooldown = _cooldown;
    }

    private void Update()
    {
        if(_currCooldown > 0)
            _currCooldown -= Time.deltaTime;
    }
}
