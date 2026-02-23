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
    public class EnemyMoveTargetComponent : IComponent
    {
        // Position to move towards
        public Vector2 Target;
        public float MoveSpeed;
    }
    
    [Game]
    public class SpriteSizeComponent : IComponent
    {
        public Vector3 Size;
    }
    
    [Game]
    public class EnemyComponent : IComponent
    {
    }
    
    [Game]
    public class CircleColliderComponent : IComponent
    {
        public float Radius;
    }
}
