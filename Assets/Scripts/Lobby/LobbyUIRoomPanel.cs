using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIRoomPanel : MonoBehaviour, ILobbyUI
{
    [SerializeField] private RectTransform playerListRect;
    [SerializeField] private Button backButton;

    public event Action OnBackButtonClick;

    private List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();

    public IEnumerable<LobbyPlayer> AllLobbyPlayers
    {
        get { return lobbyPlayers; }
    }

    public void OnEnable()
    {
        lobbyPlayers = new List<LobbyPlayer>();
        ResetUI();

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            if (OnBackButtonClick != null) OnBackButtonClick();
        });
    }

    public void ResetUI()
    {
        foreach (Transform child in playerListRect)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddPlayer(LobbyPlayer lobbyPlayer)
    {
        BoltConsole.Write(string.Format("HERE LobbyUIRoom.AddPlayer: {0}", lobbyPlayer), Color.yellow);
        Debug.Log(string.Format("HERE LobbyUIRoom.AddPlayer: {0}", lobbyPlayer));

        if (lobbyPlayer == null)
        {
            BoltConsole.Write(string.Format("LobbyUIRoom.AddPlayer lobbyPlayer was null!"), Color.red);
            Debug.LogWarning(string.Format("LobbyUIRoom.AddPlayer lobbyPlayer was null!", lobbyPlayer));
        }
        else
        {
            if (lobbyPlayers.Contains(lobbyPlayer))
                return;

            lobbyPlayers.Add(lobbyPlayer);
            lobbyPlayer.transform.SetParent(playerListRect);
            PlayerListModified();
        }
    }

    public void PlayerListModified()
    {
        //int i = 0;
        foreach (LobbyPlayer lobbyPlayer in lobbyPlayers)
        {
            //player.OnPlayerListChanged(i);
            //++i;
        }
    }

    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
