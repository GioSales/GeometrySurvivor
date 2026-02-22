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
            _player.AddSprite("Square");
            _player.AddPosition(Vector3.zero);
            _player.AddSpriteSize(new Vector3(0.25f, 0.25f, 1));
            // TODO: add player movement with input
        }
    }
}