using UnityEngine;

[BoltGlobalBehaviour("Testlevel")]
public class PlayerCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        // ui
        //GameUI.Instantiate();

        // camera
        //PlayerCamera.Instantiate();
    }

    public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token)
    {
        BoltLog.Info("PlayerCallbacks:SceneLoadLocalBegin-Token: {0}", token);
    }

    public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
    {
        BoltLog.Info("PlayerCallbacks:SceneLoadLocalDone-Token: {0}", token);
    }

    public override void SceneLoadRemoteDone(BoltConnection connection, Bolt.IProtocolToken token)
    {
        BoltLog.Info("PlayerCallbacks:SceneLoadRemoteDone-Token: {0}", token);
    }

    public override void ControlOfEntityGained(BoltEntity entity)
    {
        BoltLog.Info("PlayerCallbacks:ControlOfEntityGained: {0}", entity);
        // add audio listener to our character
        entity.gameObject.AddComponent<AudioListener>();

        // set camera callbacks
        //PlayerCamera.instance.getAiming = () => entity.GetState<IPlayerState>().Aiming;
        //PlayerCamera.instance.getHealth = () => entity.GetState<IPlayerState>().health;
        //PlayerCamera.instance.getPitch = () => entity.GetState<IPlayerState>().pitch;

        // set camera target
        //PlayerCamera.instance.SetTarget(entity);
    }
}