using Entitas;
using GamePlayer;
using InputComponents;
using Rewired;
using UnityEngine;

namespace Input.Systems
{
    public class PlayerInputSystem : IExecuteSystem, IInitializeSystem
    {
        readonly GameContext _gameContext;
        readonly InputContext _inputContext;
        GameEntity _player;
        Camera _camera;
        
        private readonly Player _rewiredPlayer;
        
        public PlayerInputSystem(Contexts contexts)
        {
            _gameContext = contexts.game;
            _inputContext = contexts.input;
            // TODO: stop using system player, use system player only for debugging actions?
            _rewiredPlayer = ReInput.players.GetSystemPlayer();
        }
        
        public void Initialize()
        {
            _camera = Camera.main;
        }
        
        public void Execute()
        {
            _player = _gameContext.playerEntity;
            
            float horMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveHorizontal);
            _player.playerMovement.HorizontalAxis = horMovement;
            
            float verMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveVertical);
            _player.playerMovement.VerticalAxis = verMovement;

            PlayerBasicAtkComponent playerBasicAtkComponent = _player.playerBasicAtk;
            playerBasicAtkComponent.IsActive = _rewiredPlayer.GetButton(RewiredConsts.Action.BasicAttack);
        }
    }
}