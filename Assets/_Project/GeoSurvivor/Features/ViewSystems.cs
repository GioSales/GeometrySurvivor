using Entitas;
using GameSystems;

namespace Features
{

    public class ViewSystems : Feature
    {
        public ViewSystems(Contexts contexts, GameObjectPool pool) : base("View Systems")
        {
            Add(new AddViewSystem(contexts, pool));
            Add(new RenderSpriteSystem(contexts));
            Add(new RenderPositionSystem(contexts));
            Add(new RenderDirectionSystem(contexts));
            Add(new LifeTimeSystem(contexts)); // TODO: move to a different feature?
        }
    }
}