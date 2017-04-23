using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {
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

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Piratable")) {
			collider.gameObject.SendMessage("OnHit");
			Destroy(this.gameObject);
		}
	}
}
