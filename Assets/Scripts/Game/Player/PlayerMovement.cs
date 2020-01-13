using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Bolt.EntityBehaviour<IPlayerState>
{
	bool localLeft;
	bool localRight;
	bool localJump;
	bool localFire;

	Vector3 localPosition = Vector3.zero;

	CharacterController _cc;

	float moveSpeed = 20f;
	float jumpForce = 10f;
	float gravity = 9.81f;

	Vector3 moveDirection = Vector3.zero;

	public override void Attached()
	{
		state.SetTransforms(state.transform, transform);
		_cc = transform.GetComponent<CharacterController>();
	}

	void PoolInput() {
		localLeft = Input.GetKey(KeyCode.A);
		localRight = Input.GetKey(KeyCode.D); 
		localFire = Input.GetMouseButton(0);
		localJump = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1);
	}

	public override void SimulateOwner()
	{
		// Lets kill all!
		/*if (Input.GetKeyDown(KeyCode.K)) {
			BoltNetwork.Detach(entity);
			BoltNetwork.Destroy(gameObject);
		}*/
		//_cc = transform.GetComponent<CharacterController>();
	}

	public override void SimulateController()
	{
		PoolInput();

		IPlayerMovementCommandInput input = PlayerMovementCommand.Create();

		input.Left = localLeft;
		input.Right = localRight;
		input.Fire = localFire;
		input.Jump = localJump;

		entity.QueueInput(input);
	}

	public override void ExecuteCommand(Bolt.Command command, bool resetState)
	{
		PlayerMovementCommand cmd = (PlayerMovementCommand)command;

		if (resetState)
		{
			SetMovementState(cmd.Result.Position, cmd.Result.Velocity, true, 0);
		}
		else
		{
			var hInput = CalcHorizontalInput(cmd.Input.Left, cmd.Input.Right);
			moveDirection = new Vector3(hInput * moveSpeed, moveDirection.y, 0);

			

			if (cmd.Input.Jump && _cc.isGrounded) {
				Debug.LogWarning("GROUNDED AND JUMP!");
				moveDirection.y += jumpForce;
			} else if (!_cc.isGrounded) {
				moveDirection.y = moveDirection.y - (gravity * BoltNetwork.FrameDeltaTime);
			}

			_cc.Move(moveDirection * BoltNetwork.FrameDeltaTime);

			//var movingDir = Move(cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump);




			/*if (cmd.Input.Jump && _cc.isGrounded) {
				Debug.LogWarning("GROUNDED AND JUMP!");
				movingDir.y += jumpForce;
			} else if (!_cc.isGrounded) {
				movingDir.y = movingDir.y - (gravity * BoltNetwork.FrameDeltaTime);
			}
			*/

			//var delta = movingDir.normalized * movingSpeed * BoltNetwork.FrameDeltaTime; 
			//var delta = movingDir.normalized * movingSpeed * BoltNetwork.FrameDeltaTime;

			//_cc.Move(delta);

			/*if (cmd.Input.Jump && _cc.isGrounded) {
				_cc.Move(new Vector3(0, 1, 0));
			} else {
				_cc.Move(new Vector3(0, -9.81f * BoltNetwork.FrameDeltaTime, 0));
			}*/


			/*localPosition = movingDir.normalized * movingSpeed * BoltNetwork.FrameDeltaTime; 
			

			transform.position = transform.position + localPosition;*/

			cmd.Result.Position = transform.position;
			cmd.Result.Velocity = Vector3.zero;

			if (entity.IsOwner) {
				cmd.Result.Token = new TestToken();
			}
		}
	}

	private Vector3 Move(bool left, bool right, bool jump) {
		Vector3 movingDir = Vector3.zero;

		if (left ^ right) {
			movingDir.x = right ? +1 : -1;
		}

		return movingDir;
	}

	private float CalcHorizontalInput(bool left, bool right)
	{
		if (left ^ right) {
			return right ? +1 : -1;
		}
		return 0;
	}

	private void SetMovementState(Vector3 position, Vector3 velocity, bool isGrounded, int jumpFrames)
	{
		transform.position = position;
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update() {
		
	}
}
