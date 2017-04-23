using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	public GameObject victoryOverlay;
	public Text victoryText;

	public PlayerResources currentResources;
	public int diamondGoal;
	
	void Start () {
	}
	
	void Update () {
		
	}

	void FixedUpdate() {
		if (GetDiamonds() >= diamondGoal) {
			Time.timeScale = 0f;
			victoryOverlay.SetActive(true);
			StartCoroutine(NextLevel());
		}
	}

	IEnumerator NextLevel() {
		yield return new WaitForSecondsRealtime(2f);
		if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			Time.timeScale = 1f;
		} else {
			victoryText.text = "You win!";
		}
	}

	public int GetDiamonds() {
		int diamonds = 0;
		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			diamonds += warehouse.containedResources.diamonds;
		}

		return diamonds;
	}

	public bool CanDeliverGems() {
		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			if (warehouse.containedResources < warehouse.maxResources) {
				return true;
			}
		}
		return false;
	}

	public int GemCapacity() {
		int c = 0;
		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			c += warehouse.maxResources.diamonds;
		}
		return c;
	}

	public void AddResources(PlayerResources r) {
		currentResources += new PlayerResources(r.gold, 0);

		foreach(GemWarehouse warehouse in GameObject.FindObjectsOfType(typeof(GemWarehouse))) {
			// Debug.Log("warehouse?");
			if (warehouse.containedResources < warehouse.maxResources) {
				int diamonds = Mathf.Min(r.diamonds, warehouse.maxResources.diamonds - warehouse.containedResources.diamonds);
				// Debug.Log("deposting " + diamonds + " diamonds");
				PlayerResources toGive = new PlayerResources(0, diamonds);
				warehouse.containedResources += toGive;
				r -= toGive;
				if (r.diamonds == 0) {
					break;
				}
			}
		}
	}

	public void Reload() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
