using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemWarehouse : MonoBehaviour {
	public Sprite fullSprite;
	public Sprite emptySprite;

	public PlayerResources hitDamage;
	public PlayerResources containedResources;
	public PlayerResources maxResources;
	public int pirateValue;

	private SpriteRenderer spriteRenderer;
	private PirateTarget pirateTarget;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		pirateTarget = GetComponent<PirateTarget>();
	}
	
	// Update is called once per frame
	void Update () {
		if (containedResources.diamonds <= 0) {
			spriteRenderer.sprite = emptySprite;
		} else {
			spriteRenderer.sprite = fullSprite;
		}
	}

	public void SetBuildSite(GameObject site) {
	}

	void FixedUpdate() {
		pirateTarget.value = containedResources.diamonds > 0 ? pirateValue : 0;
	}

	public void OnHit() {
		containedResources -= hitDamage;
	}
}
