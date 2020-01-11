using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Testlevel")]
public class ServerCallbacks : Bolt.GlobalEventListener
{
    public static bool ListenServer = true;

    void Awake()
    {
        BoltConsole.Write("ServerCallbacks:Awake() listenServer: " + ListenServer);
        if (ListenServer)
        {
            Player.CreateServerPlayer();
            Player.serverPlayer.name = "SERVER";
        }
    }

    void FixedUpdate()
    {
        foreach (Player p in Player.allPlayers)
        {
            // if we have an entity, it's dead but our spawn frame has passed
            if (p.entity && p.state.Dead && p.state.respawnFrame <= BoltNetwork.ServerFrame)
            {
                Debug.Log("Spawning a Player");
                p.Spawn();
            }
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltConsole.Write("ServerCallbacks:SceneLoadRemoteDone");
        connection.GetPlayer().InstantiateEntity();
    }

    public override void SceneLoadLocalDone(string scene)
    {
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone " + scene);
        if (Player.serverIsPlaying)
        {
            Player.serverPlayer.InstantiateEntity();
        }
    }

    public override void SceneLoadLocalBegin(string scene)
    {
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalBegin " + scene);
        foreach (Player p in Player.allPlayers)
        {
            p.entity = null;
        }
    }
}