using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public partial class LobbyManager
{
    [Header("UI Reference")]
    [SerializeField] private LobbyUIServerPanel lobbyUIServerPanel = null;
    [SerializeField] private LobbyUIServerList lobbyUIServerList = null;
    [SerializeField] private LobbyUIRoom lobbyUIRoom = null;
    [SerializeField] private LobbyUICountdownPanel lobbyUICountdownPanel = null;

    private void StartUI()
    {
        ResetUI();

        lobbyUIServerPanel.OnCreateServerButtonClick += StartServerEvent;

        lobbyUIServerList.OnClickJoinServer += JoinServerEvent;
    }

    private void ResetUI()
    {
        lobbyUIServerList.ResetUI();
    }

    private void StartServerEvent()
    {
        Debug.Log("IOJOJIOJIO");
        //uiInfoPanel.Display("Creating Room...");
        StartServerEventHandler(lobbyUIServerPanel.MatchName, lobbyUIServerPanel.MaxPlayers);
    }

    private void StartClientEvent()
    {
        //uiInfoPanel.Display("Connecting to Cloud...");
        StartClientEventHandler();
    }

    private void JoinServerEvent(UdpSession session)
    {
        //uiInfoPanel.Display("Connecting to Session...");
        JoinServerEventHandler(session);
    }

    private void EntityAttachedEventHandler(BoltEntity entity)
    {
        var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
        lobbyUIRoom.AddPlayer(lobbyPlayer);
    }

    public override void OnEvent(LobbyCountdown evt)
    {
        lobbyUICountdownPanel.SetText(string.Format("Last Duck Standing ;p\nstarting in {0}", evt.Time));
        lobbyUICountdownPanel.ToggleVisibility(evt.Time != 0);
    }
}
