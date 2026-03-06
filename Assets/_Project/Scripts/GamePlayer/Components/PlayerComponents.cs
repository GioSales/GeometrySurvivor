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
    
}