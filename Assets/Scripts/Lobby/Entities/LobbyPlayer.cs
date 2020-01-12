using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : Bolt.EntityEventListener<ILobbyPlayerState>
{
    static Color ReadyColor = Color.green;
    static Color NotReadyColor = Color.red;

    // Bolt
    public BoltConnection connection;

    public bool IsReady
    {
        get { return state.Ready; }
    }

    // Lobby
    public string playerName
    {
        get { return nameInput.text; }
    }

    public int team = 0;
    public bool ready = false;

    public InputField nameInput;
    public Text nameText;
    public Button teamButton;
    public Text teamText;
    public Button readyButton;
    public Text readyText;

    public static LobbyPlayer localPlayer;

    public override void Attached()
    {
        BoltConsole.Write(string.Format("LobbyPlayer:Attached: {0}", state.Name), Color.yellow);
        Debug.Log(string.Format("LobbyPlayer:Attached: {0}", state.Name));
        state.AddCallback("Name", () => /*{*/ nameInput.text = state.Name/*; nameText.text = state.Name; }*/);
        state.AddCallback("Team", callback: () => OnClientChangeTeam(state.Team));
        state.AddCallback("Ready", callback: () => OnClientReady(state.Ready));

        if (entity.IsOwner)
        {
            //state.Color = Random.ColorHSV();
            //state.Name = string.Format("{0} #{1}", GenerateFullName(), Random.Range(1, 100));
            state.Name = "Unnamed";
            state.Team = team = 0;
            state.Ready = ready = false;

            //nameInput.text = state.Name;
        }
    }

    public override void ControlGained()
    {
        BoltConsole.Write(string.Format("LobbyPlayer:ControlGained"), Color.yellow);
        Debug.Log(string.Format("LobbyPlayer:ControlGained"));

        //readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        SetupLocalPlayer();
    }

    public override void SimulateController()
    {
        // Update every 5 frames
        if (BoltNetwork.Frame % 4 != 0) return;

        var input = LobbyCommand.Create();

        input.Name = playerName;
        input.Team = team;
        input.Ready = ready;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        if (!entity.IsOwner) { return; }

        if (!resetState && command.IsFirstExecution)
        {
            LobbyCommand lobbyCommand = command as LobbyCommand;
            ILobbyCommandInput input = lobbyCommand.Input;

            state.Name = input.Name;
            state.Team = input.Team;
            state.Ready = input.Ready;
        }
    }

    public void RemovePlayer()
    {
        if (entity && entity.IsAttached)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }

    public override void Detached()
    {
        // This Overrides Detachment
        //if (OnDetach != null) OnDetach.Invoke(this);
    }


    public void SetupLocalPlayer()
    {
        BoltConsole.Write(string.Format("LobbyPlayer:SetupLocalPlayer"), Color.yellow);
        Debug.Log(string.Format("LobbyPlayer:SetupLocalPlayer"));

        localPlayer = this;

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
        readyButton.interactable = true;
        readyButton.gameObject.SetActive(true);

        nameInput.interactable = true;
        nameInput.gameObject.SetActive(true);

        nameInput.onEndEdit.RemoveAllListeners();

        //nameText.text = nameInput.text;
        nameText.gameObject.SetActive(false);

        teamButton.transform.GetChild(0).GetComponent<Text>().text = state.Team == 1 ? "Witches" : "Druids";
        teamText.text = state.Team == 1 ? "Witches" : "Druids";

        readyText.gameObject.SetActive(false);

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);

        teamButton.onClick.RemoveAllListeners();
        teamButton.onClick.AddListener(OnChangeTeamClicked);
        teamButton.interactable = true;
        teamButton.gameObject.SetActive(true);
        teamText.gameObject.SetActive(false);

        OnClientChangeTeam(state.Team);
        OnClientReady(state.Ready);
    }
    public void SetupOtherPlayer()
    {
        BoltConsole.Write(string.Format("LobbyPlayer:SetupOtherPlayer"), Color.yellow);
        Debug.Log(string.Format("LobbyPlayer:SetupOtherPlayer"));

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyButton.interactable = false;
        readyButton.gameObject.SetActive(false);

        nameInput.interactable = false;
        nameInput.gameObject.SetActive(false);
        nameInput.onEndEdit.RemoveAllListeners();

        nameText.text = state.Name;
        nameText.gameObject.SetActive(true);

        teamButton.transform.GetChild(0).GetComponent<Text>().text = state.Team == 1 ? "Witches" : "Druids";
        teamText.text = state.Team == 1 ? "Witches" : "Druids";

        readyText.gameObject.SetActive(true);
        readyText.text = state.Ready ? "Ready" : "Not Ready";

        teamButton.onClick.RemoveAllListeners();
        teamButton.interactable = false;
        teamButton.gameObject.SetActive(false);
        teamText.gameObject.SetActive(true);

        OnClientChangeTeam(state.Team);
        OnClientReady(state.Ready);
    }

    public void OnChangeTeamClicked()
    {
        team = team == 0 ? 1 : 0;
    }

    public void OnReadyClicked()
    {
        ready = !ready;
    }

    public void OnClientChangeTeam(int team)
    {
        teamButton.transform.GetChild(0).GetComponent<Text>().text = state.Team == 1 ? "Witches" : "Druids";
        teamText.text = state.Team == 1 ? "Witches" : "Druids";
    }

    public void OnClientReady(bool readyState)
    {
        Debug.Log(readyState);
        if (readyState)
        {
            /*if (entity.IsControlled)
            {
                readyButton.transform.GetChild(0).GetComponent<Text>().text = "Abort";
            }
            else
            {
                readyText.text = state.Ready ? "Ready" : "Not Ready";
            }*/

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Abort";
            readyButton.transform.GetChild(0).GetComponent<Text>().color = NotReadyColor;
            readyText.text = state.Ready ? "Ready" : "Not Ready";
            readyText.color = ReadyColor;


            //ChangeReadyButtonColor(TransparentColor);

            //Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
            //textComponent.text = "READY";
            //textComponent.color = ReadyColor;
            //readyButton.interactable = false;
            //nameInput.interactable = false;
        }
        else
        {
            /*if (entity.IsControlled)
            {
                readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            }
            else
            {
                readyText.text = state.Ready ? "Ready" : "Not Ready";
            }*/

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            readyButton.transform.GetChild(0).GetComponent<Text>().color = ReadyColor;
            readyText.text = state.Ready ? "Ready" : "Not Ready";
            readyText.color = NotReadyColor;

            //ChangeReadyButtonColor(entity.IsControlled ? JoinColor : NotReadyColor);

            //Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
            //textComponent.text = entity.IsControlled ? "JOIN" : "...";
            //textComponent.color = Color.white;
            readyButton.interactable = entity.IsControlled;
            //colorButton.interactable = entity.IsControlled;
            nameInput.interactable = entity.IsControlled;
        }
    }
}
