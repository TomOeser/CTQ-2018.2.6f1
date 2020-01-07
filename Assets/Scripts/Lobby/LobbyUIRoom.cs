using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIRoom : MonoBehaviour
{
    [SerializeField] private RectTransform playerListRect;

    private List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();

    public IEnumerable<LobbyPlayer> AllLobbyPlayers
    {
        get { return lobbyPlayers; }
    }

    public void OnEnable()
    {
        lobbyPlayers = new List<LobbyPlayer>();
        ResetUI();
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

        if (lobbyPlayers.Contains(lobbyPlayer))
            return;

        lobbyPlayers.Add(lobbyPlayer);

        //lobbyPlayer.transform.SetParent(null);
        lobbyPlayer.transform.SetParent(playerListRect);

        //lobbyPlayer.transform.SetParent(playerListContentTransform, false);

        //addButtonRow.transform.SetAsLastSibling();
        PlayerListModified();
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
}
