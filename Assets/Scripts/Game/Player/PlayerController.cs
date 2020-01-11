﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityEventListener<IPlayerState>
{
    const float MOUSE_SENSEITIVITY = 2f;

    bool forward;
    bool backward;
    bool left;
    bool right;
    bool jump;
    bool aiming;
    bool fire;

    int weapon;

    float yaw;
    float pitch;

    PlayerMotor _motor;

    void Awake()
    {
        _motor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        /*if (entity.IsOwner)
        {
            state.tokenTest = new TestToken() { Number = 1337 };
        }

        state.AddCallback("tokenTest", () =>
        {
            BoltLog.Info("Received token in .tokenTest property {0}", state.tokenTest);
        });*/

        state.SetTransforms(state.transform, transform);
        //state.SetAnimator(GetComponentInChildren<Animator>());

        // setting layerweights 
        //state.Animator.SetLayerWeight(0, 1);
        //state.Animator.SetLayerWeight(1, 1);

        //state.OnFire += OnFire;
        //state.AddCallback("weapon", WeaponChanged);

        // setup weapon
        //WeaponChanged();
    }

    public override void SimulateOwner()
    {
        if ((BoltNetwork.Frame % 5) == 0 && (state.Dead == false))
        {
            state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
        }

        /*
        var speed = 5f;
        var movement = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
        if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

        if (movement != Vector3.zero)
        {
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }*/
    }

    void PollKeys(bool mouse)
    {
        forward = Input.GetKey(KeyCode.W);
        backward = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        jump = Input.GetKey(KeyCode.Space);
        aiming = Input.GetMouseButton(1);
        fire = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = 1;
        }

        if (mouse)
        {
            yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSEITIVITY);
            yaw %= 360f;

            pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSEITIVITY);
            pitch = Mathf.Clamp(pitch, -85f, +85f);
        }
    }

    public override void SimulateController()
    {
        PollKeys(false);

        IPlayerCommandInput input = PlayerCommand.Create();

        input.forward = forward;
        input.backward = backward;
        input.left = left;
        input.right = right;
        input.jump = jump;

        input.aiming = aiming;
        input.fire = fire;

        input.yaw = yaw;
        input.pitch = pitch;

        input.weapon = weapon;
        input.Token = new TestToken();

        entity.QueueInput(input);

    }
    public override void ExecuteCommand(Bolt.Command c, bool resetState)
    {
        if (state.Dead)
        {
            return;
        }

        PlayerCommand cmd = (PlayerCommand)c;

        if (resetState)
        {
            _motor.SetState(cmd.Result.position, cmd.Result.velocity, cmd.Result.isGrounded, cmd.Result.jumpFrames);
        }
        else
        {
            // move and save the resulting state
            var result = _motor.Move(cmd.Input.forward, cmd.Input.backward, cmd.Input.left, cmd.Input.right, cmd.Input.jump, cmd.Input.yaw);

            cmd.Result.position = result.position;
            cmd.Result.velocity = result.velocity;
            cmd.Result.jumpFrames = result.jumpFrames;
            cmd.Result.isGrounded = result.isGrounded;

            if (cmd.IsFirstExecution)
            {
                // animation
                //AnimatePlayer(cmd);

                // set state pitch
                state.pitch = cmd.Input.pitch;
                state.weapon = cmd.Input.weapon;
                state.Aiming = cmd.Input.aiming;

                // deal with weapons
                if (cmd.Input.aiming && cmd.Input.fire)
                {
                    //FireWeapon(cmd);
                }
            }

            if (entity.IsOwner)
            {
                cmd.Result.Token = new TestToken();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
