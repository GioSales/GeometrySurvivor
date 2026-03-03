using Entitas;
using GameSystems;
using Unity.VisualScripting;

namespace Features
{

    public class ViewSystems : Feature
    {
        public ViewSystems(Contexts contexts) : base("View Systems")
        {
            Add(new AddViewSystem(contexts));
            Add(new RenderSpriteSystem(contexts));
            Add(new RenderPositionSystem(contexts));
            Add(new RenderDirectionSystem(contexts));
            Add(new LifeTimeSystem(contexts)); // TODO: move to a different feature?
        }
    }
}