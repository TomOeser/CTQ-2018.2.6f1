using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIStartPanel : MonoBehaviour, ILobbyUI
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button searchButton;

    public event Action OnHostButtonClick;
    public event Action OnSearchButtonClick;

    public void OnEnable()
    {
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(() =>
        {
            if (OnHostButtonClick != null) OnHostButtonClick();
        });

        searchButton.onClick.RemoveAllListeners();
        searchButton.onClick.AddListener(() =>
        {
            if (OnSearchButtonClick != null) OnSearchButtonClick();
        });
    }

    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
