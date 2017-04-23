﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
	public Player player;

	private SpriteRenderer spriteRenderer;
	private GameObject structure = null;
	private PlayerResources cost;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D site = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("BuildSite"));
		if (site != null) {
			transform.position = new Vector3(site.gameObject.transform.position.x, site.gameObject.transform.position.y, -100f);
		} else {
			transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, -100f);
		}

		if (Input.GetMouseButtonDown(1)) {
			structure = null;
			spriteRenderer.sprite = null;
		} else if (Input.GetMouseButtonUp(0) && site != null && structure != null) {
			Vector3 position = new Vector3(site.gameObject.transform.position.x, site.gameObject.transform.position.y, site.gameObject.transform.position.x - 1f);
			GameObject builtStructure = (GameObject)Instantiate(structure, position, Quaternion.identity);
			site.gameObject.SetActive(false);
			player.currentResources -= cost;

			structure = null;
			spriteRenderer.sprite = null;
		}
	}

	public void ChooseStructure(GameObject structure, PlayerResources cost) {
		this.structure = structure;
		this.cost = cost;
		spriteRenderer.sprite = structure.GetComponent<SpriteRenderer>().sprite;
	}
}
