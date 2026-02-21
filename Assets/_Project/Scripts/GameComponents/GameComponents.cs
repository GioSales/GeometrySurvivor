using Entitas;
using UnityEngine;

namespace GameComponents
{
    [Game]
    public class DirectionComponent : IComponent
    {
        public float Value;
    }

    [Game]
    public class PositionComponent : IComponent
    {
        public Vector2 Value;
    }

    [Game]
    public class SpriteComponent : IComponent
    {
        public string Name;
    }

    [Game]
    public class ViewComponent : IComponent
    {
        public GameObject GameObject;
    }
    
    public class MoveComponent : IComponent
    {
        public Vector2 Target;
    }

    [Game]
    public class MoverComponent : IComponent
    {
    }
    
    [Game]
    public class MoveCompleteComponent : IComponent
    {
    }
}
