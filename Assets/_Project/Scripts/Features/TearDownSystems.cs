using GameSystems;

namespace Features
{
    public class TearDownSystems : Feature
    {
        public TearDownSystems(Contexts contexts) : base("TearDown Systems")
        {
            // TODO: projectile/enemy on death effects using system added here


            // Keep last
            Add(new DestroySystem(contexts));
        }
    }
}