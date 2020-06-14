using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ABXY.AssetLink;

public class MovementController : MonoBehaviour
{
    CharacterController controller;
    

    private Vector3 movementVector = Vector3.zero;

    [SerializeField]
    private Animator animator;

    private System.Single isRunning = 0;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 velocity = movementVector * R.Character_Controller_Properties.Player_Speed / Time.deltaTime;
        velocity += velocity * isRunning;
        controller.SimpleMove(velocity);
        if (movementVector.magnitude > 0f)
        {
            this.transform.LookAt(this.transform.position + velocity, Vector3.up);
        }
    }

    public void GetRunState(InputAction.CallbackContext input)
    {
        isRunning = (System.Single)input.ReadValueAsObject();
    }

    public void GetMovement(InputAction.CallbackContext input)
    {

        Vector2 stickVector = (Vector2)input.ReadValueAsObject();
        animator.SetFloat("Speed", stickVector.magnitude + stickVector.magnitude*isRunning);
        movementVector = new Vector3(stickVector.x, 0f, stickVector.y);
    }
}
