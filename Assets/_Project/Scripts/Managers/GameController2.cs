using Features;
using Entitas;
using UnityEngine;

namespace Managers
{
    public class GameController2 : MonoBehaviour
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
            return new Feature("Systems")
                .Add(new PlayerSystems(contexts))
                .Add(new InputSystems(contexts))
                .Add(new MovementSystems(contexts))
                .Add(new ViewSystems(contexts));
        }

        void Update()
        {
            _systems.Execute();
            _systems.Cleanup();
        }
    }
}