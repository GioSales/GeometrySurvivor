using Entitas;
using Rewired;
using UnityEngine;

namespace Input.Systems
{

    // Just for debugging for now, TODO: rename?
    public class EmitInputSystem : IInitializeSystem, IExecuteSystem
    {
        readonly InputContext _context;
        private InputEntity _leftMouseEntity;
        private InputEntity _rightMouseEntity;

        public EmitInputSystem(Contexts contexts)
        {
            _context = contexts.input;
        }

        public void Initialize()
        {
            // initialize the unique entities that will hold the mouse button data
            _context.isLeftMouse = true;
            _leftMouseEntity = _context.leftMouseEntity;

            _context.isRightMouse = true;
            _rightMouseEntity = _context.rightMouseEntity;
        }

        public void Execute()
        {
            Mouse mouse = ReInput.controllers.Mouse;
            
            // mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouse.screenPosition);

            // left mouse button
            if (mouse.GetButtonDown(0))
                _leftMouseEntity.ReplaceMouseDown(mousePosition);
            
            if (mouse.GetButton(0))
                _leftMouseEntity.ReplaceMousePosition(mousePosition);
        
            if (mouse.GetButtonUp(0))
                _leftMouseEntity.ReplaceMouseUp(mousePosition);
        

            // right mouse button
            if (mouse.GetButtonDown(1))
                _rightMouseEntity.ReplaceMouseDown(mousePosition);
        
            if (mouse.GetButton(1))
                _rightMouseEntity.ReplaceMousePosition(mousePosition);
        
            if (mouse.GetButtonUp(1))
                _rightMouseEntity.ReplaceMouseUp(mousePosition);
        
        }
    }
}