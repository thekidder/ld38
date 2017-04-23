using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
	public bool friendly;
	public GameObject origin;
	public Vector2 velocity;

	void Start () {
	}
	
	void FixedUpdate () {
		transform.position += (Vector3)velocity;
	}

	void Update() {
		Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

		if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) {
			Destroy(this.gameObject);
			return;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		// Debug.Log("trigger " + collider.gameObject.name + " " + friendly);
		if ((collider.gameObject.CompareTag("Piratable") && !friendly) || (collider.gameObject.CompareTag("Pirate") && friendly)) {
			collider.gameObject.SendMessage("OnHit");
			Destroy(this.gameObject);
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Building")) {
			if (collider.gameObject != origin) {
				// always destroy when we hit a building
				Destroy(this.gameObject);
			}
		}
	}
}
