using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RBPlayerMovement : Bolt.EntityEventListener<IRBPlayerState>
{
    private Rigidbody rb;
    private Transform groundChecker;
    private CinemachineVirtualCamera cam;

    [SerializeField]
    private float speed = 15f;
    [SerializeField]
    private float jumpHeight = 30f;
    private bool isGrounded;
    [SerializeField]
    private float groundCheckDistance = 0.02f;

    public LayerMask GroundCheckLayers;

    public override void Attached()
    {
        rb = transform.GetComponent<Rigidbody>();
        groundChecker = transform.Find("GroundChecker");

        state.SetTransforms(state.Transform, transform);
    }

    public override void ControlGained()
    {
        cam = GameObject.Find("CM Cam").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = entity.transform;
    }

    public override void SimulateOwner()
    {

    }

    public override void SimulateController()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore);

        IRBPlayerCommandInput input = RBPlayerCommand.Create();

        input.Left = Input.GetKey(KeyCode.A);
        input.Right = Input.GetKey(KeyCode.D);
        input.Jump = isGrounded == true && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1));
        input.Fire = Input.GetMouseButton(0);

        input.HInput = Input.GetAxis("Horizontal");

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Bolt.Command command, bool resetState)
    {
        RBPlayerCommand cmd = (RBPlayerCommand)command;

        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore);

        if (resetState)
        {

        }
        else
        {
            var inputs = Vector3.zero;
            /*if (cmd.Input.Left)
            {
                //rb.AddForce(-1f, 0, 0, ForceMode.VelocityChange);
                inputs.x -= 1f;
            }
            if (cmd.Input.Right)
            {
                //rb.AddForce(1f, 0, 0, ForceMode.VelocityChange);
                inputs.x += 1f;
            }*/
            if (cmd.Input.HInput != 0f)
            {
                inputs.x = cmd.Input.HInput;
            }
            if (cmd.Input.Jump && isGrounded)
            {
                //rb.AddForce(0, 0.5f, 0, ForceMode.Impulse);
                rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }
            else if (!isGrounded)
            {
                rb.AddForce(Vector3.down * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.Acceleration);
            }

            rb.MovePosition(rb.position + inputs * speed * BoltNetwork.FrameDeltaTime);
        }
    }
}
