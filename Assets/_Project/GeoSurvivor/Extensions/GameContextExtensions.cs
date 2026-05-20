using UnityEngine;

namespace GeoSurvivor.Extensions
{
    public static class GameContextExtensions
    {
        public static void CreateMessage(GameContext _context, string message)
        {
            GameEntity messageEntity = _context.CreateEntity();
            messageEntity.AddDebugMessage(message);
        }

        public static void CreateEnemy(GameContext _context, Vector2 position)
        {
            GameEntity enemy = _context.CreateEntity();
            enemy.isEnemy = true;
            enemy.AddEnemyMoveTarget(newTarget: Vector2.zero, newMoveSpeed: 0.5f);
            enemy.AddPosition(position);
            enemy.AddDirection(Random.Range(0,360));
            enemy.AddSprite("Triangle");
            enemy.AddSpriteSize(new Vector3(0.15f, 0.15f, 1));
            enemy.AddCircleCollider(0.12f);
            enemy.AddHealth(2);
            enemy.AddExpDrop(1);
        }
    }
}