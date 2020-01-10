using UnityEngine;

public class CTDPlayerController : Bolt.EntityBehaviour<ICTDPlayerState>
{
    //public TextMesh playerNameText;

    public override void Attached()
    {
        BoltConsole.Write("Attached CTDPlayerController");

        state.SetTransforms(state.Transform, transform);

        Debug.LogFormat("CTDPlayerController.Attached entity.IsOwner: {0} for {1}", entity.IsOwner, state.Name);

        if (entity.IsOwner)
        {

            //state.Color = Color.white;
        }

        /*state.AddCallback("Color", () =>
        {
            GetComponent<MeshRenderer>().material.color = state.Color;
        });*/

        state.AddCallback("Name", () =>
        {
            Debug.LogFormat("CTDPlayerController.Bolt-Name-Callback called with state.name: {0}", state.Name);
            //playerNameText.text = state.Name;
        });
    }

    public override void Initialized()
    {
        BoltConsole.Write("Initialized CTDPlayer");
    }

    public override void ControlGained()
    {
        BoltConsole.Write("ControlGained CTDPlayerController", Color.blue);
    }

    public override void SimulateOwner()
    {
        var speed = 5f;
        var movement = Vector3.zero;

        //if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
        //if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
        if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
        if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

        //if (Input.GetKey(KeyCode.F)) { movement.x += 1; }

        if (movement != Vector3.zero)
        {
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }
    }

    private void Setup(string playerName, int playerTeam)
    {
        BoltConsole.Write(string.Format("Setup CTD Player with Name: {0}, Team: {1}", playerName, playerTeam));

        if (entity.IsOwner)
        {
            state.Name = playerName;
            state.Team = playerTeam;
        }
    }

    public static void Spawn()
    {
        var pos = new Vector3(Random.Range(-2, 2), 1f, 0f);
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.CTDPlayer, pos, Quaternion.identity);
        playerEntity.TakeControl();
        //HERE

        CTDPlayerController playerController = playerEntity.GetComponent<CTDPlayerController>();

        LobbyPlayer lobbyPlayer = LobbyPlayer.localPlayer;

        if (lobbyPlayer)
        {
            playerController.Setup(lobbyPlayer.playerName, lobbyPlayer.team);
        }
        else
        {
            playerController.Setup("Player #" + Random.Range(1, 100), 0);
        }
    }
}