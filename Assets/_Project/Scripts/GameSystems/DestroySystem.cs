using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace GameSystems
{
    public class DestroySystem : IExecuteSystem
    {
        readonly GameContext _context;
        readonly GameObjectPool _pool;
        readonly IGroup<GameEntity> _entitiesToDestroy;

        public DestroySystem(Contexts contexts, GameObjectPool pool)
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
                }
            }
        }
    }
}
