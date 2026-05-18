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
    
    [Game]
    public class ProjectileComponent : IComponent
    {
        public float Speed;
    }
    
    [Game]
    public class ProjectileDirectionComponent : IComponent
    {
        public Vector2 Value;
    }
    
    [Game]
    public class LifeTimeComponent : IComponent
    {
        public float TimeLeft;
    }
    
    // Destroys the entire entity at end of frame
    [Game]
    [Cleanup(CleanupMode.DestroyEntity)]
    public sealed class ToBeDestroyedComponent : IComponent { }
    
    [Game]
    public class HealthComponent : IComponent
    {
        public int Value;
    }
    
    [Game]
    [Cleanup(CleanupMode.RemoveComponent)]
    public class TakeDamageComponent : IComponent
    {
        public int Value;
    }
    
    [Game]
    public class DealDamageComponent : IComponent
    {
        public int Value;
    }

}
