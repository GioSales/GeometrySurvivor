using Entitas;
using Input.Systems;
using GameSystems;

namespace Features
{
    public class InputSystems : Feature
    {
        public InputSystems(Contexts contexts) : base("Input Systems")
        {
            Add(new EmitInputSystem(contexts));
            Add(new CreateMoverSystem(contexts));
            Add(new PlayerInputSystem(contexts));
        }         
    }
}