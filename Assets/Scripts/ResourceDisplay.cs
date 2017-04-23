using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {
	public Player player;

	public Text gold;
	public Text diamonds;
	public Text goal;
	public GameObject gemsFull;

	void Start () {
		
	}
	
	void Update () {
		goal.text = "Accumulate " + player.diamondGoal + " gems";

		int gems = player.GetDiamonds();
		int maxGems = player.GemCapacity();

		gemsFull.SetActive(gems == maxGems);

		gold.text = "Gold: " + player.currentResources.gold;
		diamonds.text = "Gems: " + gems + " / " + maxGems;
	}
}
