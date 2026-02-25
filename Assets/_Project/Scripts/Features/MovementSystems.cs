using GameSystems;

namespace Features
{
    public class MovementSystems : Feature
    {
        public MovementSystems(Contexts contexts) : base("Movement Systems")
        {
            Add(new EnemyMoveSystem(contexts));
            Add(new EnemyFollowPlayerSystem(contexts));
            Add(new ProjectileMovementSystem(contexts));
        }
    }
}