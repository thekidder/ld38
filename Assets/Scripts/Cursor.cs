using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
	public Player player;

	private SpriteRenderer spriteRenderer;
	private BuildingDepth buildingDepth;
	private GameObject structure = null;
	private PlayerResources cost;
	private bool inWater;
	private Vector2 waterSiteSize = new Vector2(.9f, .9f);

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildingDepth = GetComponent<BuildingDepth>();
		Redisplay();
	}

	void Redisplay() {
		Utility.Redisplay();
	}
	
	void Update () {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		string layer = inWater ? "WaterSite" : "BuildSite";
		Collider2D site = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask(layer));
		if (site != null) {
			transform.position = new Vector3(site.gameObject.transform.position.x, site.gameObject.transform.position.y, 0f);
			buildingDepth.Redisplay();
		} else {
			transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, -200f);
		}

		Collider2D boatCollider = Physics2D.OverlapBox(transform.position, waterSiteSize, 0f, LayerMask.GetMask("Boat", "Island"));

		if (structure != null && inWater && boatCollider != null) {
			Debug.Log(boatCollider.gameObject.name);
			Color c = spriteRenderer.color;
			c.a = 0.5f;
			spriteRenderer.color = c;
		} else {
			spriteRenderer.color = Color.white;
		}

		if (Input.GetMouseButtonDown(1)) {
			structure = null;
			spriteRenderer.sprite = null;
		} else if (Input.GetMouseButtonUp(0) && site != null && structure != null) {
			if (inWater && boatCollider != null) {
				Debug.Log("can't place on boat");
				return;
			}

			Vector3 position = new Vector3(site.gameObject.transform.position.x, site.gameObject.transform.position.y, site.gameObject.transform.position.z - 1f);
			GameObject builtStructure = (GameObject)Instantiate(structure, position, Quaternion.identity);
			builtStructure.SendMessage("SetBuildSite", site.gameObject);

			if (!inWater) {
				site.gameObject.SetActive(false);
			}
			player.currentResources -= cost;

			structure = null;
			spriteRenderer.sprite = null;

			Redisplay();
		}
	}

	public void ChooseStructure(GameObject structure, bool inWater, PlayerResources cost) {
		this.structure = structure;
		this.cost = cost;
		this.inWater = inWater;
		spriteRenderer.sprite = structure.GetComponent<SpriteRenderer>().sprite;
	}
}
