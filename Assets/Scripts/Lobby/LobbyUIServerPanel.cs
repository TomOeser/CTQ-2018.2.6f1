using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIServerPanel : MonoBehaviour
{
    [SerializeField] private InputField matchNameInputField;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Button createServerButton;

    public event Action OnCreateServerButtonClick;

    private int maxPlayers = 6;
    private static string DefaultMatchName { get { return "Capture the Quak Server"; } }
    private string matchName = DefaultMatchName;

    public string MatchName { get { return !String.IsNullOrWhiteSpace(matchName) ? matchName : LobbyUIServerPanel.DefaultMatchName; } }
    public int MaxPlayers { get { return maxPlayers; } }

    public void OnEnable()
    {
        Debug.Log("ONENAB");
        createServerButton.onClick.RemoveAllListeners();
        createServerButton.onClick.AddListener(() =>
        {
            Debug.Log("CLICK");

            if (OnCreateServerButtonClick != null) OnCreateServerButtonClick();
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
}
