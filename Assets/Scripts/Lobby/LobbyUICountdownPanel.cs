﻿using UnityEngine;
using UnityEngine.UI;

public class LobbyUICountdownPanel : MonoBehaviour, ILobbyUI
{
    [SerializeField] private Text countdownText;

    public void SetText(string text)
    {
        countdownText.text = text;
    }

    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
