using System.Collections;
using UnityEngine;
using System;
using UdpKit;
using Bolt.Matchmaking;
using Bolt;

public partial class LobbyManager : Bolt.GlobalEventListener
{
    public static LobbyManager Instance;

    [Header("Ready Check and Playercount")]
    [SerializeField] private int minPlayers = 2;
    [Tooltip("Time in seconds between all players ready and match start")]
    [SerializeField] [Range(0.0f, 10.0f)] private float prematchCountdown = 5.0f;

    private int maxPlayers = 6;
    private static string DefaultMatchName { get { return "Capture the Quak"; } }
    private string matchName = DefaultMatchName;

    private bool isCountdownActive = false;

    private void Awake()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
    }

    public new void OnEnable()
    {
        base.OnEnable();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartUI();
    }

    private void FixedUpdate()
    {
        if (BoltNetwork.IsServer && isCountdownActive == false)
        {
            VerifyReady();
        }
    }

    private void VerifyReady()
    {
        var allReady = true;
        var readyCount = 0;

        foreach (var entity in BoltNetwork.Entities)
        {
            if (entity.StateIs<ILobbyPlayerState>() == false) continue;

            var lobbyPlayer = entity.GetState<ILobbyPlayerState>();

            allReady &= lobbyPlayer.Ready;

            if (allReady == false) break;
            readyCount++;
        }

        if (allReady && readyCount >= minPlayers)
        {
            isCountdownActive = true;
            StartCoroutine(ServerCountdownCoroutine());
        }
    }

    private IEnumerator ServerCountdownCoroutine()
    {
        var remainingTime = prematchCountdown;
        var floorTime = Mathf.FloorToInt(remainingTime);

        LobbyCountdown countdown;

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            var newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
                countdown.Time = floorTime;
                countdown.Send();
                BoltConsole.Write(string.Format("Countdown: {0}", floorTime));
            }
        }

        countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
        countdown.Time = 0;
        countdown.Send();

        BoltNetwork.LoadScene("GameScene");
    }



    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
        BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
        BoltNetwork.RegisterTokenClass<ServerConnectToken>();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            var token = new RoomProtocolToken()
            {
                matchName = !String.IsNullOrWhiteSpace(matchName) ? matchName : LobbyManager.DefaultMatchName,
                mapName = "Capture The Quak Map 1",
                maxPlayers = (byte)maxPlayers,
                ArbitraryData = "My DATA",
            };

            BoltLog.Info("Starting Server");

            // Start Photon Room
            BoltMatchmaking.CreateSession(
                sessionID: !String.IsNullOrWhiteSpace(matchName) ? matchName : LobbyManager.DefaultMatchName,
                token: token
            );
        }
    }


    // Extension after rebuild
    ////////////////////////////
    ////////////////////////////
    ////////////////////////////
    private void StartServerEventHandler(string matchName, int maxPlayers = 6)
    {
        this.matchName = matchName;
        this.maxPlayers = maxPlayers;
        BoltLauncher.StartServer();
    }

    private void StartClientEventHandler()
    {
        BoltLauncher.StartClient();
    }
    private void JoinServerEventHandler(UdpSession session)
    {
        if (BoltNetwork.IsClient)
        {
            BoltNetwork.Connect(session);
        }
    }
    private void ShutdownEventHandler()
    {
        BoltLauncher.Shutdown();
    }


    // MORE Rebuild

    public override void SessionCreated(UdpSession session)
    {
        ChangeToPanel(lobbyUIRoomPanel);

        // Build Server Entity
        var entity = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayer);
        entity.TakeControl();
    }

    public override void EntityAttached(BoltEntity entity)
    {
        if (entity.StateIs<ILobbyPlayerState>())
        {
            EntityAttachedEventHandler(entity);
            var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            if (lobbyPlayer)
            {
                if (entity.IsControlled)
                {
                    lobbyPlayer.SetupLocalPlayer();
                }
                else
                {
                    lobbyPlayer.SetupOtherPlayer();
                }
            }
        }
    }

    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsClient)
        {
            BoltConsole.Write(string.Format("HERE Client Connected: {0}", connection), Color.yellow);
            Debug.Log(string.Format("HERE Client Connected: {0}", connection));

            ChangeToPanel(lobbyUIRoomPanel);
        }
        else if (BoltNetwork.IsServer)
        {
            BoltConsole.Write(string.Format("HERE Server Connected: {0}", connection), Color.yellow);
            Debug.Log(string.Format("HERE Server Connected: {0}", connection));

            var entity = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayer);
            entity.AssignControl(connection);
        }

        BoltConsole.Write("Connected", Color.red);

        connection.UserData = new Player();
        connection.GetPlayer().connection = connection;
        connection.GetPlayer().name = "CLIENT:" + connection.RemoteEndPoint.Port;
    }

    public override void Disconnected(BoltConnection connection)
    {
        foreach (var entity in BoltNetwork.Entities)
        {
            if (entity.StateIs<ILobbyPlayerState>() == false || entity.IsController(connection) == false) continue;

            var player = entity.GetComponent<LobbyPlayer>();

            if (player)
            {
                player.RemovePlayer();
            }
        }
    }


}
