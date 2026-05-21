using System.Collections.Generic;
using Entitas;
using GeoSurvivor.Extensions;

namespace GamePlayer
{
    public class PlayerLevelUpSystem : ReactiveSystem<GameEntity>, IInitializeSystem
    {
        private GameContext _context;
        private GameEntity _player;
        
        public PlayerLevelUpSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }
        
        public void Initialize()
        {
            _player = _context.playerEntity;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.PlayerGainExp);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isPlayer;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity entity in entities)
            {
                int expToGain = entity.playerGainExp.ExpToGain;
                _player.playerExp.TotalExp += expToGain;
                GameContextExtensions.CreateMessage(_context, "Player gained " + expToGain + ", Total: " + _player.playerExp.TotalExp);

                if (_player.playerExp.TotalExp >= _player.playerExpNeeded.Value)
                {
                    int newPlayerLevel = _player.playerLevel.Value + 1;
                    _player.ReplacePlayerLevel(newPlayerLevel);
                    // TODO: improve exp level up formula
                    int newExpNeeded = _player.playerExpNeeded.Value + 10;
                    _player.ReplacePlayerExpNeeded(newExpNeeded);
                    GameContextExtensions.CreateMessage(_context, "Player Leveled up to level " + newPlayerLevel);
                }
            }
        }

        
    }
}