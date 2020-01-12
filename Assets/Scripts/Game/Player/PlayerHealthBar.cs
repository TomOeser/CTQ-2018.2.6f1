using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

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
		instance.GetComponent<Slider>().maxValue = 20; // hier maxHP einfügen
		instance.GetComponent<Slider>().value = 15; // hier currentHP einfügen
		instance.transform.position = Camera.main.WorldToScreenPoint(playerPosition);
	}

	public void HealthBar(){
		float t_hp = 15; // hier currentHP einfügen
		playerPosition = new Vector3(transform.position.x, transform.position.y + hpHeightCorrectionY, transform.position.z);
		instance.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(playerPosition);
		instance.GetComponent<Slider>().value = t_hp; 
		if(t_hp == 0){
			Destroy(healthBar);
		}
	}
	
	// Update is called once per frame
	void Update () {
		HealthBar();
	}
}
