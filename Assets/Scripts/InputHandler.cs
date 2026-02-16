using Rewired;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: make inputhandler and compute input, then consume input to move
public class InputHandler : MonoBehaviour
{
    private Player _player;
    private float _moveSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = ReInput.players.GetSystemPlayer();
    }
    
    void FixedUpdate()
    {

        float moveHorizontal = _player.GetAxis(RewiredConsts.Action.MoveHorizontal);
        float moveVertical = _player.GetAxis(RewiredConsts.Action.MoveVertical);
        
        Vector2 movement = new Vector2(moveHorizontal * _moveSpeed, moveVertical * _moveSpeed) * Time.deltaTime;
        
        transform.Translate(movement);

        
    }
}
