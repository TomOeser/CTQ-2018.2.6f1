using UnityEngine;
using System;
using UdpKit;

public class LobbyUIServerList : Bolt.GlobalEventListener
{
    [SerializeField] private RectTransform serverListRect;
    [SerializeField] private GameObject lobbyServerPrefab;
    [SerializeField] private GameObject noServerFound;

    public event Action<UdpSession> OnClickJoinServer;

    private new void OnEnable()
    {
        base.OnEnable();
        ResetUI();
    }

    public void ResetUI()
    {
        noServerFound.SetActive(true);
        foreach (Transform child in serverListRect)
        {
            Destroy(child.gameObject);
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        ResetUI();

        if (sessionList.Count == 0)
        {
            noServerFound.SetActive(true);
            return;
        }

        noServerFound.SetActive(false);

        foreach (var pair in sessionList)
        {
            UdpSession session = pair.Value;

            GameObject serverEntry = Instantiate(lobbyServerPrefab, serverListRect, false);
            serverEntry.GetComponent<UIServerEntry>().Populate(session, Color.green,
            () =>
            {
                if (OnClickJoinServer != null) OnClickJoinServer.Invoke(session);

                /*if (BoltNetwork.IsClient && BoltNetwork.IsConnected)
                {
                    BoltLauncher.Shutdown();
                    BoltLauncher.StartClient();
                }

                var token = new ServerConnectToken();
                BoltNetwork.Connect(session, token);*/
            });
        }
    }
}
