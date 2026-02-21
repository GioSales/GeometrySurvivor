using Rewired;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;
using Mouse = Rewired.Mouse;

// TODO: make inputhandler and compute input, then consume input to move
public class PlayerInputHandler : MonoBehaviour
{
    private Player _rewiredPlayer;
    
    [SerializeField] private float _moveSpeed = 10f;

    [SerializeField] private Weapon _weapon;
    [SerializeField] private Vector3 mousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rewiredPlayer = ReInput.players.GetSystemPlayer();
    }
    
    // TODO: make sure game is not affected by FPS and stays consistent (fixed timestep?)
    void FixedUpdate()
    {
        Movement();

        // BasicAttack();
    }

    private void BasicAttack()
    {
        Mouse mouse = ReInput.controllers.Mouse;
        bool basicAttack = _rewiredPlayer.GetButton(RewiredConsts.Action.BasicAttack);
        mousePosition = Camera.main.ScreenToWorldPoint(mouse.screenPosition);
        Vector2 direction = Vector2.Normalize(new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y));
        // Vector2 direction = Vector2.left;
        if (basicAttack)
            _weapon.Fire(direction);
    }

    private void Movement()
    {
        float moveHorizontal = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveHorizontal);
        float moveVertical = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveVertical);
        
        Vector2 movement = new Vector2(moveHorizontal * _moveSpeed, moveVertical * _moveSpeed) * Time.deltaTime;
        
        transform.Translate(movement);
    }
}
