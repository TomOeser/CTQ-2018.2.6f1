using System;
using UdpKit;
using UnityEngine;

public partial class LobbyManager
{
    [Header("UI Reference")]
    [SerializeField] private LobbyUIStartPanel lobbyUIStartPanel = null;
    [SerializeField] private LobbyUIServerPanel lobbyUIServerPanel = null;
    [SerializeField] private LobbyUIServerListPanel lobbyUIServerListPanel = null;
    [SerializeField] private LobbyUIRoomPanel lobbyUIRoomPanel = null;
    [SerializeField] private LobbyUICountdownPanel lobbyUICountdownPanel = null;

    private ILobbyUI _currentPanel = null;

    private void StartUI()
    {
        ResetUI();

        lobbyUIStartPanel.OnHostButtonClick += HostGameEvent;
        lobbyUIStartPanel.OnSearchButtonClick += SearchGameEvent;

        lobbyUIServerPanel.OnCreateServerButtonClick += StartServerEvent;
        lobbyUIServerPanel.OnBackButtonClick += ServerPanelBackButtonEvent;

        lobbyUIServerListPanel.OnJoinServerClick += JoinServerEvent;
        lobbyUIServerListPanel.OnBackButtonClick += ServerListPanelBackButtonEvent;

        lobbyUIRoomPanel.OnBackButtonClick += RoomPanelBackButtonEvent;

        ChangeToPanel(lobbyUIStartPanel);
    }

    private void ResetUI()
    {
        lobbyUIServerListPanel.ResetUI();
    }

    private void ToggleBackground(bool visible)
    {
        transform.Find("Background").gameObject.SetActive(visible);
    }

    private void EntityAttachedEventHandler(BoltEntity entity)
    {
        if (entity.StateIs<ILobbyPlayerState>())
        {
            var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            lobbyUIRoomPanel.AddPlayer(lobbyPlayer);
        }
    }

    // UI EVENTS

    // StartPanel
    public void HostGameEvent()
    {
        ChangeToPanel(lobbyUIServerPanel);
    }
    public void SearchGameEvent()
    {
        lobbyUIServerListPanel.ResetUI();
        ChangeToPanel(lobbyUIServerListPanel);
        StartClientEventHandler();
    }

    // ServerPanel
    private void StartServerEvent()
    {
        StartServerEventHandler(lobbyUIServerPanel.MatchName, lobbyUIServerPanel.MaxPlayers);
    }
    private void ServerPanelBackButtonEvent()
    {
        ChangeToPanel(lobbyUIStartPanel);
    }

    // ServerListPanel
    private void JoinServerEvent(UdpSession session)
    {
        JoinServerEventHandler(session);
    }
    private void ServerListPanelBackButtonEvent()
    {
        ChangeToPanel(lobbyUIStartPanel);
        ShutdownEventHandler();
    }

    // RoomPanel
    private void RoomPanelBackButtonEvent()
    {
        if (BoltNetwork.IsServer)
        {
            ChangeToPanel(lobbyUIServerPanel);
            ShutdownEventHandler();
            // Inform Players server has closed
            Debug.Log("IsSErver");
        }
        else
        {
            ChangeToPanel(lobbyUIServerListPanel);
            ShutdownEventHandler();
            StartClientEventHandler();
        }
    }

    // BOLT Events
    public override void SceneLoadLocalBegin(string scene)
    {
        BoltLog.Info(string.Format("Loading Scene: {0}", scene));
    }

    private void ChangeToPanel(ILobbyUI newPanel)
    {
        if (_currentPanel != null)
        {
            _currentPanel.ToggleVisibility(false);
        }

        if (newPanel != null)
        {
            newPanel.ToggleVisibility(true);
        }

        _currentPanel = newPanel;


        if (_currentPanel == null)
        {
            // So the game is running - blend out the background-image

        }

        // Currently we have nothing like that
        /*if (uiMainMenu == _currentPanel as LobbyUIMainMenu)
        {
            uiTopPanel.HideBackButton();
        }
        else
        {
            uiTopPanel.SetupBackButton("Shutdown", ShutdownEventHandler);
        }*/
    }

    public override void SceneLoadLocalDone(string scene)
    {
        BoltLog.Info(string.Format("Loading Scene: {0} Done.", scene));

        try
        {
            if (scene == "Testlevel")
            {
                // Hiding the LobbyManager
                ChangeToPanel(null);
                ToggleBackground(false);
            }
            else
            {
                ChangeToPanel(lobbyUIStartPanel);
                ToggleBackground(true);
            }

            /*if (lobbyScene.SimpleSceneName == scene)
            {
                ChangeBodyTo(uiMainMenu);

                uiTopPanel.HideBackButton();
                uiTopPanel.SetInGame(false);
            }
            else
            {
                ChangeBodyTo(null);

                uiTopPanel.SetInGame(true);
                uiTopPanel.ToggleVisibility(false);
                uiTopPanel.SetupBackButton("Menu", ShutdownEventHandler);
            }*/

        }
        catch (Exception e)
        {
            BoltLog.Error(e);
        }
    }

    public override void OnEvent(LobbyCountdown evt)
    {
        ChangeToPanel(null);
        lobbyUICountdownPanel.SetText(string.Format("Last Duck Standing ;p\nstarting in {0}", evt.Time));
        lobbyUICountdownPanel.ToggleVisibility(evt.Time != 0);
    }
}
