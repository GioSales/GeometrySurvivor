using GamePlayer;
using GameSystems;

namespace Features
{
    public class PlayerSystems : Feature
    {
        public PlayerSystems(Contexts contexts) : base("Player Systems")
        {
            Add(new PlayerInitSystem(contexts));
            Add(new PlayerMovementSystem(contexts));
            Add(new PlayerActionSystem(contexts));
        }  
    }
}