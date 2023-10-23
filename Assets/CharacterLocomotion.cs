using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class CharacterLocomotion : NetworkBehaviour
{
    public InputActionAsset InputControl;
    public InputActionMap MovementControl;

    public InputAction Move;
    public InputAction Look;

    Rigidbody rb;
    NavMeshAgent agent;
    private float agentSpeed = 3;

    private Animator mAnimator;
    [SerializeField] string idle = "Idle";
    [SerializeField] string walk = "Walk";

    public Transform cameraTarget;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        MovementControl = InputControl.FindActionMap("MovementControl");
        MovementControl.Enable();

        Move = MovementControl.FindAction("Move");
        Look = MovementControl.FindAction("Look");

        mAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        if(isOwned)
        {
            CinemachineSwitcher.Instance.OnPlayerAdded(NetworkClient.connection.identity.gameObject.GetComponent<CharacterLocomotion>().cameraTarget.gameObject);
        }

        CmdPlayAnimation(idle, 0.2f, 0);
    }
    [Command(requiresAuthority =false)]
    public virtual void CmdPlayAnimation(string animName, float transition_duration, float normalized_begin_time)
    {
        RpcPlayAnimation(animName, transition_duration, normalized_begin_time);
    }
    [ClientRpc]
    public virtual void RpcPlayAnimation(string animName, float transition_duration, float normalized_begin_time)
    {
        mAnimator.Play(animName);
    }


    private void FixedUpdate()
    {
        if(isOwned)
        {
            OwnerMovementLogic();
        }
    }

    private void ServerMovementLogic()
    {
        
    }
    private void OwnerMovementLogic()
    {
        Vector3 movement = CalculateMovement();

        if(movement.magnitude > 0)
        {
            CmdPlayAnimation(walk, 0.2f, 0);
            CmdMove(movement);
            CmdRotate(Quaternion.LookRotation(movement.normalized).eulerAngles.y);
        }
        else
        {
            CmdPlayAnimation(idle, 0.2f, 0);
        }


    }
    private Vector3 CalculateMovement()
    {
        Vector2 movement = Move.ReadValue<Vector2>();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * movement.y +
            right * movement.x;
    }
    [Command(requiresAuthority = false)]
    private void CmdMove(Vector3 movementInput)
    {        
        MoveCharacter(movementInput);
    }
    private void MoveCharacter(Vector3 movementInput)
    {
        agent.velocity = movementInput * agentSpeed * velocityMultiplier;
        //agent.Move(movementInput * agentSpeed * Time.fixedDeltaTime);
    }
    [Command(requiresAuthority = false)]
    public void CmdRotate(float yRotation)
    {
        Quaternion currentRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        Quaternion _targetRotation = Quaternion.Euler(0, yRotation, 0);
        transform.rotation = Quaternion.RotateTowards(currentRotation, _targetRotation, Time.fixedDeltaTime * rotationLerp);
    }
    [SerializeField] float rotationLerp = 330;
    [SerializeField] float velocityMultiplier = 100;
}
