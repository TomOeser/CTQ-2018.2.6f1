using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIServerPanel : MonoBehaviour, ILobbyUI
{
    [SerializeField] private InputField matchNameInputField;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Button createServerButton;
    [SerializeField] private Button backButton;

    public event Action OnCreateServerButtonClick;
    public event Action OnBackButtonClick;

    private int maxPlayers = 6;
    private static string DefaultMatchName { get { return "Capture the Quak Server"; } }
    private string matchName = DefaultMatchName;

    public string MatchName { get { return !String.IsNullOrWhiteSpace(matchName) ? matchName : LobbyUIServerPanel.DefaultMatchName; } }
    public int MaxPlayers { get { return maxPlayers; } }

    public void OnEnable()
    {
        createServerButton.onClick.RemoveAllListeners();
        createServerButton.onClick.AddListener(() =>
        {
            if (OnCreateServerButtonClick != null) OnCreateServerButtonClick();
        });
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            if (OnBackButtonClick != null) OnBackButtonClick();
        });
    }

    public void OnMatchNameInputFieldChanged()
    {
        Debug.Log("OnMatchNameInputFieldChanged");
        matchName = matchNameInputField.text;
    }
    public void OnMaxPlayersSliderChanged()
    {
        Debug.Log("OnMaxPlayersSliderChanged");
        maxPlayers = (int)maxPlayersSlider.value;
        GameObject valueLabel = maxPlayersSlider.transform.Find("ValueLabel").gameObject;
        valueLabel.GetComponent<Text>().text = maxPlayers.ToString();
    }

    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
