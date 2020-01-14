using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Player : IDisposable
{
    public const byte TEAM_DRUIDS = 0;
    public const byte TEAM_WITCHES = 1;

    public string name;
    public int team;
    public BoltEntity entity;
    public BoltConnection connection;

    public IPlayerState state
    {
        get { return entity.GetState<IPlayerState>(); }
    }

    public bool isServer
    {
        get { return connection == null; }
    }

    public Player()
    {
        players.Add(this);
    }


    internal void Spawn()
    {
        if (entity)
        {
            state.Dead = false;
            state.health = 100;

            // teleport
            entity.transform.position = RandomSpawn();
        }
    }

    public void Kill()
    {
        if (entity)
        {
            state.Dead = true;
            state.respawnFrame = BoltNetwork.ServerFrame + (15 * BoltNetwork.FramesPerSecond);
        }
    }

    public void Dispose()
    {
        players.Remove(this);

        // destroy
        if (entity)
        {
            BoltNetwork.Destroy(entity.gameObject);
        }

        // while we have a team difference of more then 1 player
        /*while (Mathf.Abs(redPlayers.Count() - bluePlayers.Count()) > 1)
        {
            if (redPlayers.Count() < bluePlayers.Count())
            {
                var player = bluePlayers.First();
                player.Kill();
                player.state.team = TEAM_RED;
            }
            else
            {
                var player = redPlayers.First();
                player.Kill();
                player.state.team = TEAM_BLUE;
            }
        }*/
    }

    public void InstantiateEntity()
    {
        BoltConsole.Write("Player:InstantiateEntity isServer: " + isServer);
        entity = BoltNetwork.Instantiate(BoltPrefabs.Player, new TestToken(), RandomSpawn(), Quaternion.identity);

        var rbEntity = BoltNetwork.Instantiate(BoltPrefabs.RBPlayer, new TestToken(), RandomSpawn(), Quaternion.identity);

        state.name = name;
        state.team = team; //redPlayers.Count() >= bluePlayers.Count() ? TEAM_BLUE : TEAM_RED;
        state.name = LobbyPlayer.localPlayer.playerName;
        state.team = LobbyPlayer.localPlayer.team;

        if (isServer)
        {
            BoltConsole.Write("Player:InstantiateEntity We are on the Server. Entity created, taking the control of the Entity");
            entity.TakeControl(new TestToken());

            rbEntity.TakeControl(new TestToken());
        }
        else
        {
            BoltConsole.Write("Player:InstantiateEntity We are on the Server Entity. created, assing the control of the Entity to client");
            entity.AssignControl(connection, new TestToken());

            rbEntity.AssignControl(connection, new TestToken());
        }
        Spawn();
    }
}

partial class Player
{
    static List<Player> players = new List<Player>();

    public static IEnumerable<Player> druidPlayers
    {
        get { return players.Where(x => x.entity && x.state.team == TEAM_DRUIDS); }
    }

    public static IEnumerable<Player> witchPlayers
    {
        get { return players.Where(x => x.entity && x.state.team == TEAM_WITCHES); }
    }

    public static IEnumerable<Player> allPlayers
    {
        get { return players; }
    }

    public static bool serverIsPlaying
    {
        get { return serverPlayer != null; }
    }

    public static Player serverPlayer
    {
        get;
        private set;
    }

    public static void CreateServerPlayer()
    {
        serverPlayer = new Player();
    }

    static Vector3 RandomSpawn()
    {
        float x = UnityEngine.Random.Range(-5f, +5f);
        //float z = UnityEngine.Random.Range(-32f, +32f);
        return new Vector3(x, 1, 0);
    }
}
