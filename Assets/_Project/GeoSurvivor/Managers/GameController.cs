using Features;
using Entitas;
using GameSystems;
using UnityEngine;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        private Systems _systems;
        private Contexts _contexts;

        void Start()
        {
            _contexts = Contexts.sharedInstance;
            _systems = CreateSystems(_contexts);
            _systems.Initialize();
        }

        private static Systems CreateSystems(Contexts contexts)
        {
            var pool = new GameObjectPool();

            return new Feature("Systems")
                .Add(new PlayerSystems(contexts))
                .Add(new InputSystems(contexts))
                .Add(new MovementSystems(contexts))
                .Add(new ViewSystems(contexts, pool))
                .Add(new DestroyFxSystems(contexts))
                .Add(new ProjectileCollisionSystem(contexts))
                .Add(new DebugMessageSystem(contexts))
                .Add(new DamageSystem(contexts))
                .Add(new CleanUpSystems(contexts, pool));
        }

        void Update()
        {
            _systems.Execute();
            _systems.Cleanup();
        }
    }
}