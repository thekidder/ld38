using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour {
	public Cursor cursor;
	public Player player;
	public PlayerResources cost;
	public GameObject prefab;
	public bool inWater;

	private Button button;

	void Start() {
		button = GetComponent<Button>();
	}

	void Update() {
		button.interactable = player.currentResources >= cost;
	}

	public void Select() {
		cursor.ChooseStructure(prefab, inWater, cost);
	}
}
