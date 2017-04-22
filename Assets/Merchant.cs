using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour {
	enum State {
		DEPOSITING,
		RETURNING,
		LOADING,
		DELIVERING
	}

	public GameObject depositZone;
	public GameObject loadingZone;

	public float deliverTime;
	public float loadTime;

	public Sprite side;
	public Sprite back;

	private State currentState;

	public Player player;
	public PlayerResources resourcesToGive;

	private SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		StartCoroutine(Load());
	}
	
	void FixedUpdate () {
		switch (currentState) {
			case State.DELIVERING:
				MoveTowards(depositZone);
				break;
			case State.RETURNING:
				MoveTowards(loadingZone);
				break;
			case State.LOADING:
			case State.DEPOSITING:
				break;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		switch (currentState) {
			case State.DELIVERING:
				if (collider.gameObject == depositZone) {
					StartCoroutine(Deposit());
				}
				break;
			case State.RETURNING:
				if (collider.gameObject == loadingZone) {
					StartCoroutine(Load());
				}
				break;
			case State.LOADING:
			case State.DEPOSITING:
				break;
		}
	}

	IEnumerator Deposit() {
		currentState = State.DEPOSITING;
		yield return new WaitForSeconds(deliverTime);
		player.currentResources += resourcesToGive;
		currentState = State.RETURNING;
	}

	IEnumerator Load() {
		currentState = State.LOADING;
		yield return new WaitForSeconds(loadTime);
		currentState = State.DELIVERING;
	}

	void MoveTowards(GameObject dest) {
		Vector2 direction = GetDirection(dest);
		SetFacing(direction);
		transform.position += (Vector3)(1f / Constants.PIXEL_SIZE * direction);
	}

	Vector2 GetDirection(GameObject dest) {
		Vector2 direction = dest.transform.position - transform.position;
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
			return Mathf.Sign(direction.x) * Vector2.right;
		} else {
			return Mathf.Sign(direction.y) * Vector2.up;
		}
	}

	void SetFacing(Vector2 direction) {
		if (direction.x != 0.0f) {
			spriteRenderer.sprite = side;
		} else {
			spriteRenderer.sprite = back;
		}
	}
}
