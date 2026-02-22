using Entitas;
using Entitas.CodeGeneration.Attributes;
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
    
    [Game]
    public class MoveComponent : IComponent
    {
        public Vector2 Target;
    }
    
    [Game]
    public class SpriteSizeComponent : IComponent
    {
        public Vector3 Size;
    }

    // TODO: remove?
    [Game]
    public class MoverComponent : IComponent
    {
    }
    
    [Game]
    public class MoveCompleteComponent : IComponent
    {
    }
    
    [Game, Unique]
    public class PlayerComponent : IComponent
    {
    }
}
