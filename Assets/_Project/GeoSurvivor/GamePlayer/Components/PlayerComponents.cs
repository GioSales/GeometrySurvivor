using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace GamePlayer
{
    [Game, Unique]
    public class PlayerComponent : IComponent
    {
    }
    
    [Game]
    public class PlayerMovementComponent : IComponent
    {
        public float MoveSpeed;
        public float HorizontalAxis;
        public float VerticalAxis;
    }

    [Game]
    public class PlayerBasicAtkComponent : IComponent
    {
        public bool IsActive;
        public float Cooldown;
        public float CdTimer;
    }
    
    [Game]
    public class PlayerAimDirectionComponent : IComponent
    {
        public Vector2 Value;
    }
    
    [Game]
    public class PlayerExpComponent : IComponent
    {
        public int TotalExp;
    }
    
    [Game]
    public class PlayerExpNeededComponent : IComponent
    {
        public int Value;
    }
    
    [Game]
    public class PlayerLevelComponent : IComponent
    {
        public int Value;
    }
    
    [Game]
    [Cleanup(CleanupMode.RemoveComponent)]
    public class PlayerGainExpComponent : IComponent
    {
        public int ExpToGain;
    }
    
}