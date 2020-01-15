using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : Bolt.EntityBehaviour<IPlayerState>
{
    bool localLeft;
    bool localRight;
    bool localJump;
    bool localFire;
    bool localInteract;
    float localHorizontalInput;

    Vector3 localPosition = Vector3.zero;

    CharacterController cc;

    [Header("Debug Stuff")]
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private float gravity = -9.81f;

    [Header("Movement Settings")]
    [SerializeField]
    float moveSpeed = 20f;
    [SerializeField]
    private float jumpHeight = 80f;

    [Header("Gravity and Drag Settings")]
    [SerializeField]
    private float gravityMultiplier = 10f;
    [SerializeField]
    private Vector3 drag = new Vector3(1, 0, 1);

    [Header("Ground Check Settings")]
    [SerializeField]
    private Transform groundChecker;
    [SerializeField]
    private float groundCheckDistance = 0.02f;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private bool isGrounded;


    void Awake()
    {
        cc = transform.GetComponent<CharacterController>();
        if (groundChecker == null)
        {
            groundChecker = transform.Find("GroundChecker");
        }
    }
    void Update()
    {
        PoolInput();
    }

    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
        //cc = transform.GetComponent<CharacterController>();

        /*if (groundChecker == null)
        {
            groundChecker = transform.Find("GroundChecker");
        }*/
    }

    void PoolInput()
    {
        localLeft = Input.GetKey(KeyCode.A);
        localRight = Input.GetKey(KeyCode.D);
        localFire = Input.GetMouseButton(0);
        localJump = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1);
        localInteract = Input.GetKey(KeyCode.F);
        localHorizontalInput = Input.GetAxis("Horizontal");
    }

    public override void SimulateOwner()
    {
        // Lets kill all!
        /*if (Input.GetKeyDown(KeyCode.K)) {
			BoltNetwork.Detach(entity);
			BoltNetwork.Destroy(gameObject);
		}*/
    }

    public override void SimulateController()
    {
        PoolInput();

        IPlayerMovementCommandInput input = PlayerMovementCommand.Create();

        input.Left = localLeft;
        input.Right = localRight;
        input.Fire = localFire;
        input.Jump = localJump;
        input.Interact = localInteract;
        input.Horizontal = localHorizontalInput;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Bolt.Command command, bool resetState)
    {
        if (state.Dead)
        {
            return;
        }

        PlayerMovementCommand cmd = (PlayerMovementCommand)command;

        if (resetState)
        {
            SetMovementState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, 0);
            localJump = cmd.Input.Jump;
            localInteract = cmd.Input.Interact;
        }
        else
        {
            isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, whatIsGround, QueryTriggerInteraction.Ignore);
            if (isGrounded && velocity.y < 0f)
                velocity.y = 0f;

            moveDirection = new Vector3(cmd.Input.Horizontal, 0, 0);
            cc.Move(moveDirection * BoltNetwork.FrameDeltaTime * moveSpeed);

            transform.position.Set(transform.position.x, transform.position.y, 0f);

            if (cmd.Input.Jump && isGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity = ApplyGravityTo(velocity, gravity, gravityMultiplier, BoltNetwork.FrameDeltaTime);
            velocity = ApplyDragTo(velocity, drag, BoltNetwork.FrameDeltaTime);
            cc.Move(velocity * BoltNetwork.FrameDeltaTime);

            transform.position.Set(transform.position.x, transform.position.y, 0f);

            cmd.Result.Position = transform.position;
            cmd.Result.Velocity = velocity;
            cmd.Result.IsGrounded = isGrounded;
            cmd.Result.IsInteracting = cmd.Input.Interact;

            if (entity.IsOwner)
            {
                cmd.Result.Token = new TestToken();
            }
        }
    }

    private void SetMovementState(Vector3 position, Vector3 _velocity, bool _isGrounded, int jumpFrames)
    {
        position.z = 0f;
        transform.position = position;
        velocity = _velocity;
        isGrounded = _isGrounded;
    }

    private Vector3 ApplyGravityTo(Vector3 vel, float gravity, float gravityMultiplier, float deltaTime)
    {
        vel.y += gravity * gravityMultiplier * deltaTime;
        return vel;
    }

    private Vector3 ApplyDragTo(Vector3 vel, Vector3 drag, float deltaTime)
    {
        vel.x /= 1 + drag.x * deltaTime;
        vel.y /= 1 + drag.y * deltaTime;
        vel.z /= 1 + drag.z * deltaTime;
        return vel;
    }
}
