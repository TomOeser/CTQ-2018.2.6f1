using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : Bolt.EntityBehaviour<IPlayerState> {

	public GameObject healthBar;
	public GameObject canvas;
	public GameObject instance;
	public float hpHeightCorrectionY = 30;
	public Vector3 playerPosition;
	public Vector3 healthBarPosition;

	public float tempCurrentHP;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find("UICanvas");

		if(!healthBar){
			healthBar = Resources.Load<GameObject>("HealthBar");
			Debug.Log("Health bar loaded: " + healthBar.name);
		}

		instance = Instantiate<GameObject>(healthBar);
		instance.transform.SetParent(canvas.transform, false);
		playerPosition = new Vector3(transform.position.x, transform.position.y + hpHeightCorrectionY, transform.position.z);
		instance.GetComponent<Slider>().minValue = 0;
		instance.GetComponent<Slider>().maxValue = state.health; // hier maxHP einfügen
		instance.GetComponent<Slider>().value = state.health; // hier currentHP einfügen
		instance.transform.position = Camera.main.WorldToScreenPoint(playerPosition);
	}

	public override void Attached(){
		state.AddCallback("health", HealthBar);
	}
	
	public void HealthBar(){
		int t_hp = state.health; // hier currentHP einfügen
		playerPosition = new Vector3(transform.position.x, transform.position.y + hpHeightCorrectionY, transform.position.z);
		instance.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(playerPosition);
		instance.GetComponent<Slider>().value = t_hp; 
		if(t_hp == 0){
			Destroy(instance);
		}
	}
	
	void Update()
	{
		playerPosition = new Vector3(transform.position.x, transform.position.y + hpHeightCorrectionY, transform.position.z);
		instance.transform.position = Camera.main.WorldToScreenPoint(playerPosition);	
	}
	
	public override void SimulateController()
	{
		if(Input.GetKeyDown(KeyCode.U)){
			state.health +=5;
		}	

		if(Input.GetKeyDown(KeyCode.I)){
			state.health -=5;
		}
	}
}
