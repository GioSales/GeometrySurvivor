using Entitas;
using UnityEngine;

namespace GamePlayer
{
    public class PlayerMovementSystem : IExecuteSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        public PlayerMovementSystem(Contexts contexts)
        {
            _context = contexts.game;
        }
        
        public void Execute()
        {
            _player = _context.playerEntity;
            
            // TODO: improve initialization or init order so this is not needed
            if(!_player.hasView)
                return;
            
            float moveHorizontal = _player.playerMovement.HorizontalAxis;
            float moveVertical   = _player.playerMovement.VerticalAxis;
            float moveSpeed      = _player.playerMovement.MoveSpeed; 
            
            Vector2 movement = new Vector2(moveHorizontal * moveSpeed, moveVertical * moveSpeed) * Time.deltaTime;
            Transform playerTransform = _player.view.GameObject.transform;
            playerTransform.Translate(movement);
        }
    }
}