using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

using Cinemachine;

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

    CinemachineVirtualCamera cinemachineVirtualCamera;

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

        //gameObject.GetComponent<MeshRenderer>().material.color = state.team == 0 ? Color.blue : Color.red;

        if (entity.IsOwner)
        {
            // Using the Balancing-CSV to read out the values and place them into an Object for the player
            // this then will get replicated due the IPlayerState

            BoltConsole.Write("PlayerController:Attached Server: Updating BalanceSettings in State for added Player", Color.green);
            Type type = state.BalanceSettings.GetType();
            foreach (CSVValue entry in CSVValueLookup.Instance.ValueList)
            {
                PropertyInfo info = type.GetProperty(entry.name);
                if (info != null)
                {
                    info.SetValue(state.BalanceSettings, entry.value);
                }
            }

            // Now the state.BalanceSettings-Object should contain all Values of the Properties
            // which are defined in the BoltAssets.BalanceSettings-Object itself, only known properties are set

            BoltConsole.Write("PlayerController:Attached Server: Replicate BalanceSettings now", Color.green);
            state.BalanceSettingsChanged();
        }

        // Adding the Eventlistener for BalanceSettingsUpdated which is called from the server if needed
        state.OnBalanceSettingsChanged += OnBalanceSettingsChanged;
    }

    public override void ControlGained()
    {
        cinemachineVirtualCamera = GameObject.Find("CM Cam").GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = entity.transform;
    }

    void OnBalanceSettingsChanged()
    {
        BoltConsole.Write("PlayerController:OnBalanceSettingsChanged Client: Event triggered", Color.magenta);
        BoltConsole.Write("PlayerController:OnBalanceSettingsChanged Client: Updating PlayerMotor with new BalanceSettings", Color.magenta);
        _motor.UpdateWithBalancingSettings(state.BalanceSettings);
    }



    // Since we have all entities created on the server, just the server is the owner
    // Here it ticks up the health over interval
    public override void SimulateOwner()
    {
        if ((BoltNetwork.Frame % 5) == 0 && (state.Dead == false))
        {
            state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
        }
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

    // This is called on the Client eg. the "Controller" because at instantiation we gave the control to the player
    // The server is also controlling just its player-entity, so also the server calls this function for its entity
    public override void SimulateController()
    {
        PollKeys(false);

        IPlayerCommandInput input = PlayerCommand.Create();

        //input.forward = forward;
        //input.backward = backward;
        input.forward = false;
        input.backward = false;

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

    // This function is called on the server after a player does some action in the SimulateController function and send out the command
    // The Server interpret the input, change the values on the entity (because he is the owner) and then replicate it to all clients
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

            // Doing the best i can
            result.position.z = 0;

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

            if (entity.HasControl)
            {
                //Camera.main.transform.position = new Vector3(entity.transform.position.x, 15, -30);
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
