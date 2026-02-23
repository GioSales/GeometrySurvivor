using Entitas;
using GamePlayer;
using Rewired;

namespace Input.Systems
{
    public class PlayerInputSystem : IExecuteSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        private readonly Player _rewiredPlayer;
        
        public PlayerInputSystem(Contexts contexts)
        {
            _context = contexts.game;
            // TODO: stop using system player, use system player only for debugging actions?
            _rewiredPlayer = ReInput.players.GetSystemPlayer();
        }
        
        public void Execute()
        {
            _player = _context.playerEntity;
            
            float horMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveHorizontal);
            _player.playerMovement.HorizontalAxis = horMovement;
            
            float verMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveVertical);
            _player.playerMovement.VerticalAxis = verMovement;

            PlayerActionComponent actionComponent = _player.playerAction;
            actionComponent.BasicAtkActive = _rewiredPlayer.GetButton(RewiredConsts.Action.BasicAttack);
            // TODO: skill action state
        }
    }
}