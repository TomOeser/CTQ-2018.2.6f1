using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UdpKit;
using Bolt.Matchmaking;
using Bolt;

public enum LobbyShowPanelState
{
    StartPanel,
    ServerPanel,
    SearchGamePanel,
    ClientPanel,
    TeamPanel
}

public partial class LobbyManager : Bolt.GlobalEventListener
{
    public static LobbyManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _serverPanel;
    [SerializeField] private GameObject _searchGamePanel;
    //[SerializeField] private GameObject _clientPanel;
    [SerializeField] private GameObject _teamPanel;

    [Header("Server Related")]
    [SerializeField] private InputField _matchNameInputField;
    [SerializeField] private Slider _maxPlayersSlider;

    [Header("Client Related")]
    //[SerializeField] private GameObject lobbyPlayerEntryPrefab;
    [SerializeField] private RectTransform _playerListContent;

    [Header("Ready Check and Playercount")]
    [SerializeField] private int minPlayers = 2;
    [Tooltip("Time in seconds between all players ready and match start")]
    [SerializeField] [Range(0.0f, 10.0f)] private float prematchCountdown = 5.0f;

    private LobbyShowPanelState _lobbyShowPanel = LobbyShowPanelState.StartPanel;
    private int maxPlayers = 6;
    private static string DefaultMatchName { get { return "Capture the Quak"; } }
    private string matchName = DefaultMatchName;

    private bool isCountdownActive = false;

    public LobbyShowPanelState ActivePanelState
    {
        get { return _lobbyShowPanel; }
        set
        {
            _lobbyShowPanel = value;
            ShowPanel();
        }
    }

    private void Awake()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
        ShowPanel();
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

        BoltConsole.Write("Game should start now");

        //BoltNetwork.LoadScene(gameScene.SimpleSceneName);
    }

    public void OnHostGameButtonClicked()
    {
        Debug.Log("OnHostGameButtonClicked");
        ActivePanelState = LobbyShowPanelState.ServerPanel;
    }
    public void OnSearchGameButtonClicked()
    {
        Debug.Log("OnSearchGameButtonClicked");
        //ClearServerListUI();
        ActivePanelState = LobbyShowPanelState.SearchGamePanel;
        BoltLauncher.StartClient();
    }
    public void OnBackButtonClicked()
    {
        Debug.Log("OnBackButtonClicked");

        if (BoltNetwork.IsClient)
        {
            if (ActivePanelState == LobbyShowPanelState.SearchGamePanel)
            {
                BoltLauncher.Shutdown();
                ActivePanelState = LobbyShowPanelState.StartPanel;
            }
            else if (ActivePanelState == LobbyShowPanelState.TeamPanel)
            {
                ActivePanelState = LobbyShowPanelState.SearchGamePanel;
            }
        }
        else if (BoltNetwork.IsServer)
        {
            Debug.Log("OnBackButtonClicked IsServer: true");
            if (ActivePanelState == LobbyShowPanelState.ServerPanel)
            {
                if (BoltNetwork.IsConnected)
                {
                    BoltLauncher.Shutdown();
                    BoltNetwork.Shutdown();
                }
                ActivePanelState = LobbyShowPanelState.StartPanel;
            }

            if (BoltNetwork.IsConnected)
            {
                BoltLauncher.Shutdown();
                BoltNetwork.Shutdown();
            }
            ActivePanelState = LobbyShowPanelState.StartPanel;
        }
        else
        {
            Debug.Log(BoltNetwork.IsServer + " " + BoltNetwork.IsClient + " " + BoltNetwork.IsConnected);
            ActivePanelState = LobbyShowPanelState.StartPanel;
        }
    }

    private void ShowPanel()
    {
        _startPanel.SetActive(false);
        _serverPanel.SetActive(false);
        _searchGamePanel.SetActive(false);
        //_clientPanel.SetActive(false);
        _teamPanel.SetActive(false);

        switch (_lobbyShowPanel)
        {
            case LobbyShowPanelState.StartPanel:
                _startPanel.SetActive(true);
                break;
            case LobbyShowPanelState.ServerPanel:
                _serverPanel.SetActive(true);
                break;
            case LobbyShowPanelState.SearchGamePanel:
                _searchGamePanel.SetActive(true);
                break;
            case LobbyShowPanelState.TeamPanel:
                _teamPanel.SetActive(true);
                break;
        }
        Debug.Log("ShowPanel");
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
        else if (BoltNetwork.IsClient)
        {
            /*if (randomJoin)
            {
                BoltMatchmaking.JoinRandomSession();
            }
            else
            {
                ClientStaredUIHandler();
            }

            randomJoin = false;*/
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
        Debug.Log(string.Format("HERE LobbyManager.SessionCreated: {0}", session));
        BoltConsole.Write(string.Format("HERE LobbyManager.SessionCreated: {0}", session), Color.magenta);

        ActivePanelState = LobbyShowPanelState.TeamPanel;

        // Build Server Entity
        var entity = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayer);
        entity.TakeControl();
    }

    public override void EntityAttached(BoltEntity entity)
    {
        Debug.Log(string.Format("HERE LobbyManager.EntityAttached: {0}", entity));
        BoltConsole.Write(string.Format("HERE LobbyManager.EntityAttached: {0}", entity), Color.magenta);

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

    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsClient)
        {
            BoltConsole.Write(string.Format("HERE Client Connected: {0}", connection), Color.yellow);
            Debug.Log(string.Format("HERE Client Connected: {0}", connection));
            ActivePanelState = LobbyShowPanelState.TeamPanel;
        }
        else if (BoltNetwork.IsServer)
        {
            BoltConsole.Write(string.Format("HERE Server Connected: {0}", connection), Color.yellow);
            Debug.Log(string.Format("HERE Server Connected: {0}", connection));

            var entity = BoltNetwork.Instantiate(BoltPrefabs.LobbyPlayer);
            entity.AssignControl(connection);
        }

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
