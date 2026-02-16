using UnityEngine;
using UnityEngine.InputSystem;

// TODO: make inputhandler and compute input, then consume input to move
public class InputHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (keyboard != null)
        {
            KeyboardInput(keyboard);
        }

        
    }

    private void KeyboardInput(Keyboard keyboard)
    {
        Vector2 movement = Vector2.zero;
        if (keyboard.wKey.isPressed) movement.y += 1;
        if (keyboard.sKey.isPressed) movement.y -= 1;
        if (keyboard.aKey.isPressed) movement.x -= 1;
        if (keyboard.dKey.isPressed) movement.x += 1;
        
        transform.Translate(movement);
    }
}
