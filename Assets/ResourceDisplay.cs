using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {
	public Player player;

	public Text gold;
	public Text diamonds;

	void Start () {
		
	}
	
	void Update () {
		gold.text = "Gold: " + player.currentResources.gold;
		diamonds.text = "Diamonds: " + player.currentResources.diamonds;
	}
}
