using GameSystems;

namespace Features
{
    public class CleanUpSystems : Feature
    {
        public CleanUpSystems(Contexts contexts, GameObjectPool pool) : base("TearDown Systems")
        {
            Add(new DeathSystem(contexts, pool));

            // Keep last
            Add(new GameCleanupSystems(contexts));
        }
    }
}