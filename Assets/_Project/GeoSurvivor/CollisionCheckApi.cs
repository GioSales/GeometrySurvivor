using GameComponents;

namespace GeoSurvivor
{
    public static class CollisionCheckApi
    {
        public static bool IsColliding(PositionComponent positionA, CircleColliderComponent collA,
            PositionComponent positionB, CircleColliderComponent collB)
        {
            float distanceSq = (positionA.Value - positionB.Value).sqrMagnitude;
            float radiiSum = collA.Radius + collB.Radius;
            return distanceSq < radiiSum * radiiSum;
        }
    }
}