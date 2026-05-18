using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace GameSystems
{
    public class DeathSystem : IExecuteSystem
    {
        readonly GameContext _context;
        readonly GameObjectPool _pool;
        readonly IGroup<GameEntity> _entitiesToDestroy;

        public DeathSystem(Contexts contexts, GameObjectPool pool)
        {
            _context = contexts.game;
            _pool = pool;
            _entitiesToDestroy = _context.GetGroup(GameMatcher.ToBeDestroyed);
        }

        public void Execute()
        {
            GameEntity[] entitiesToDestroy = _entitiesToDestroy.GetEntities();
            foreach (GameEntity e in entitiesToDestroy)
            {
                if (e.hasView)
                {
                    GameObject go = e.view.GameObject;
                    go.Unlink();
                    _pool.Return(go);
                    e.RemoveView();
                    
                    // spawn EXP and do other on death stuff, maybe move somewhere else?
                    if (e.hasExpDrop)
                    {
                        GameEntity expEntity = _context.CreateEntity();
                        expEntity.AddExp(e.expDrop.ExpValue);
                        expEntity.AddPosition(e.position.Value);
                        expEntity.AddSprite("Circle");
                        expEntity.AddSpriteSize(new Vector3(0.07f, 0.07f, 1));
                        expEntity.AddCircleCollider(0.05f);
                    }
                }
            }
        }
    }
}
