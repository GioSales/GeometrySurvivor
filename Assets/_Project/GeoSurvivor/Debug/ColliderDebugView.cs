using Entitas;
using GameComponents;
using UnityEngine;

namespace GeoSurvivor.Debug
{
    public class ColliderDebugView : MonoBehaviour
    {
        private GameContext _context;

        void Start()
        {
            _context = Contexts.sharedInstance.game;
        }

        void OnDrawGizmos()
        {
            if (_context == null) return;

            DrawGroup(
                _context.GetGroup(GameMatcher.AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.Position,
                    GameMatcher.CircleCollider)),
                Color.red);

            DrawGroup(
                _context.GetGroup(GameMatcher.AllOf(
                    GameMatcher.Projectile,
                    GameMatcher.Position,
                    GameMatcher.CircleCollider)),
                Color.blue);
        }

        private void DrawGroup(IGroup<GameEntity> group, Color color)
        {
            Gizmos.color = color;
            foreach (GameEntity entity in group)
            {
                Gizmos.DrawWireSphere(entity.position.Value, entity.circleCollider.Radius);
            }
        }
    }
}
