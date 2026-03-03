using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class LifeTimeSystem : IExecuteSystem
    {
        readonly GameContext _gameContext;
        
        public LifeTimeSystem(Contexts contexts)
        {
            _gameContext = contexts.game;
        }
        
        public void Execute()
        {
            GameEntity[] lifeTimeEntities = _gameContext.GetGroup(GameMatcher.LifeTime).GetEntities();
            foreach (GameEntity e in lifeTimeEntities)
            {
                e.lifeTime.TimeLeft -= Time.deltaTime;
                if (e.lifeTime.TimeLeft <= 0)
                    e.isToBeDestroyed = true; // mark for destruction
            }
        }
    }
}