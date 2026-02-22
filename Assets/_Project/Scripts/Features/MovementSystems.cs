using GameSystems;

namespace Features
{
    public class MovementSystems : Feature
    {
        public MovementSystems(Contexts contexts) : base("Movement Systems")
        {
            Add(new MoveSystem(contexts));
            Add(new EnemyFollowPlayerSystem(contexts));
        }
    }
}