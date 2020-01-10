using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityEventListener<IPlayerState>
{
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
        if ((BoltNetwork.Frame % 5) == 0 && (state.dead == false))
        {
            state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
        }

        var speed = 5f;
        var movement = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
        if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

        if (movement != Vector3.zero)
        {
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }
    }

    public override void SimulateController()
    {
        /*PollKeys(false);

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

        entity.QueueInput(input);*/
    }
    public override void ExecuteCommand(Bolt.Command c, bool resetState)
    {

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
