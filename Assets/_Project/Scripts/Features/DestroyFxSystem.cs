using GameSystems;

namespace Features
{
    public class DestroyFxSystems : Feature
    {
        public DestroyFxSystems(Contexts contexts) : base("DestroyFx Systems")
        {
            // TODO: projectile/enemy on death effects using system added here
            Add(new ProjectileDestroySystem(contexts));
        }
        
    }
}