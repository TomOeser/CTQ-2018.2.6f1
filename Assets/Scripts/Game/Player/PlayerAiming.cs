using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : Bolt.EntityEventListener<IPlayerState>
{
    private Vector2 positionOnScreen = Vector2.zero;
    private Vector2 mouseOnScreen = Vector2.zero;
    private float localeAngle = 0f;
    private float lastLocaleAngle = 0f;

    [SerializeField]
    private Transform shoulder;

    public override void Attached()
    {
        state.AddCallback("AimingAngle", OnAimingAngleChanged);
    }

    void OnAimingAngleChanged()
    {
        shoulder.rotation = Quaternion.Euler(new Vector3(0f, 0f, state.AimingAngle));
    }

    public override void SimulateOwner()
    {

    }

    public override void SimulateController()
    {
        positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        localeAngle = AngleBetweenTwoPoints(mouseOnScreen, positionOnScreen) + 90;

        if (lastLocaleAngle != localeAngle)
        {
            //shoulder.rotation = Quaternion.Euler(new Vector3(0f, 0f, localeAngle));
            IPlayerAimingCommandInput input = PlayerAimingCommand.Create();
            input.AimingAngle = localeAngle;
            entity.QueueInput(input);

            lastLocaleAngle = localeAngle;
        }

    }

    public override void ExecuteCommand(Bolt.Command command, bool resetState)
    {
        try
        {
            PlayerAimingCommand cmd = (PlayerAimingCommand)command;

            if (resetState)
            {
                //shoulder.rotation = Quaternion.Euler(new Vector3(0f, 0f, cmd.Input.AimingAngle));
            }
            else
            {
                if (cmd.IsFirstExecution)
                {
                    if (entity.IsOwner)
                    {
                        state.AimingAngle = cmd.Input.AimingAngle;
                    }
                    //shoulder.rotation = Quaternion.Euler(new Vector3(0f, 0f, cmd.Input.AimingAngle));
                    cmd.Result.AimingAngle = cmd.Input.AimingAngle;
                }

                if (entity.IsOwner)
                {
                    cmd.Result.Token = new TestToken();
                }
            }
        }
        catch (System.Exception ex) { }
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}