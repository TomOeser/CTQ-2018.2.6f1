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
                Debug.Log("ServerCallbacks:FixedUpdate Spawning a Player");
                p.Spawn();
            }
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltConsole.Write("ServerCallbacks:SceneLoadRemoteDone");
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone on Client done instantiating ClientPlayer");
        connection.GetPlayer().InstantiateEntity();
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone on Client done instantiating ClientPlayer done");
    }

    public override void SceneLoadLocalDone(string scene)
    {
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone " + scene);
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone on Server done instantiating ServerPlayer");
        if (Player.serverIsPlaying)
        {
            Player.serverPlayer.InstantiateEntity();
        }
        BoltConsole.Write("ServerCallbacks:SceneLoadLocalDone on Server done instantiating ServerPlayer done");
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