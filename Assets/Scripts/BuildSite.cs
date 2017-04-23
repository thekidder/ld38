using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSite : MonoBehaviour {
	public float maxOpacity = 0.25f;
	public float minOpacity = 0.0f;
	public float opacityStep = 0.005f;


	private SpriteRenderer spriteRenderer;
	private bool increasingOpacity = true;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = new Color(1f, 1f, 1f, minOpacity);
	}
	
	void FixedUpdate () {
		float newOpacity = spriteRenderer.color.a;

		if (increasingOpacity) {
			newOpacity += opacityStep;

			if (newOpacity >= maxOpacity) {
				increasingOpacity = false;
				newOpacity = maxOpacity;
			}
		} else {
			newOpacity -= opacityStep;

			if (newOpacity <= minOpacity) {
				increasingOpacity = true;
				newOpacity = minOpacity;
			}
		}

		spriteRenderer.color = new Color(1f, 1f, 1f, newOpacity);
	}
}
