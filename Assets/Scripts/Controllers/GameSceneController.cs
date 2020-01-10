using UnityEngine;

//[BoltGlobalBehaviour("GameScene")]
[BoltGlobalBehaviour(BoltNetworkModes.Server, "GameScene")]
public class GameSceneController : Bolt.GlobalEventListener
{
    /*public override void SceneLoadLocalDone(string scene)
    {
        BoltConsole.Write("Spawn Player on map " + scene, Color.yellow);
        CTDPlayerController.Spawn();
    }*/

    /*void FixedUpdate()
    {
        foreach (Player p in Player.allPlayers)
        {
            // if we have an entity, it's dead but our spawn frame has passed
            if (p.entity && p.state.dead && p.state.respawnFrame <= BoltNetwork.ServerFrame)
            {
                p.Spawn();
            }
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        connection.GetPlayer().InstantiateEntity();
    }

    public override void SceneLoadLocalDone(string scene)
    {
        if (Player.serverIsPlaying)
        {
            Player.serverPlayer.InstantiateEntity();
        }
    }

    public override void SceneLoadLocalBegin(string scene)
    {
        foreach (Player p in Player.allPlayers)
        {
            p.entity = null;
        }
    }*/
}