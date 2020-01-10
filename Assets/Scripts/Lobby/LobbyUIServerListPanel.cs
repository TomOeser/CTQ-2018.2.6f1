using UnityEngine;
using System;
using UdpKit;
using UnityEngine.UI;

public class LobbyUIServerListPanel : Bolt.GlobalEventListener, ILobbyUI
{
    [SerializeField] private RectTransform serverListRect;
    [SerializeField] private GameObject lobbyServerPrefab;
    [SerializeField] private GameObject noServerFound;
    [SerializeField] private Button backButton;

    public event Action<UdpSession> OnJoinServerClick;
    public event Action OnBackButtonClick;

    private new void OnEnable()
    {
        base.OnEnable();
        ResetUI();

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            if (OnBackButtonClick != null) OnBackButtonClick();
        });
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
                if (OnJoinServerClick != null) OnJoinServerClick.Invoke(session);
            });
        }
    }

    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
