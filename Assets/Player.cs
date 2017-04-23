using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public PlayerResources currentResources;
	public int diamondGoal;
	
	void Start () {
	}
	
	void Update () {
		
	}

	public int GetDiamonds() {
		int diamonds = 0;
		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			diamonds += warehouse.containedResources.diamonds;
		}

		return diamonds;
	}

	public void AddResources(PlayerResources r) {
		currentResources += new PlayerResources(r.gold, 0);

		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			Debug.Log("warehouse?");
			if (warehouse.containedResources < warehouse.maxResources) {
				int diamonds = Mathf.Min(r.diamonds, warehouse.maxResources.diamonds - warehouse.containedResources.diamonds);
				Debug.Log("deposting " + diamonds + " diamonds");
				PlayerResources toGive = new PlayerResources(0, diamonds);
				warehouse.containedResources += toGive;
				r -= toGive;
				if (r.diamonds == 0) {
					break;
				}
			}
		}
	}
}
