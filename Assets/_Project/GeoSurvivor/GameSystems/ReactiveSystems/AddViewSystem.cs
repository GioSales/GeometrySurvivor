using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace GameSystems
{

    public class AddViewSystem : ReactiveSystem<GameEntity>
    {
        readonly Transform _viewContainer = new GameObject("Game Views").transform;
        readonly GameContext _context;
        readonly GameObjectPool _pool;

        public AddViewSystem(Contexts contexts, GameObjectPool pool) : base(contexts.game)
        {
            _context = contexts.game;
            _pool = pool;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Sprite);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasSprite && !entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity e in entities)
            {
                GameObject go = _pool.Borrow();
                if (go == null)
                    go = new GameObject("Game View");

                go.transform.SetParent(_viewContainer, false);
                e.AddView(go);
                go.Link(e);
            }
        }
    }
}