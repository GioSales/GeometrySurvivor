using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class PlayerInitSystem : IInitializeSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        public PlayerInitSystem(Contexts contexts)
        {
            _context = contexts.game;
        }

        public void Initialize()
        {
            _context.isPlayer = true;
            _player = _context.playerEntity;
            _player.AddSprite(newName: "Square");
            _player.AddPosition(newValue: Vector3.zero);
            _player.AddSpriteSize(newSize: new Vector3(x: 0.25f, y: 0.25f, z: 1));
            _player.AddPlayerMovement(newMoveSpeed: 1, newHorizontalAxis: 0, newVerticalAxis: 0);
            _player.AddPlayerBasicAtk(newIsActive: false, newCooldown: 0.5f, newCdTimer: 0);
            _player.AddPlayerAimDirection(Vector2.zero);
            _player.AddHealth(10);
        }
    }
}