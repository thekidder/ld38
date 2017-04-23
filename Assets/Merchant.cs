using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : Boat {
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

	private State currentState;

	public Player player;
	public PlayerResources resourcesToGive;

	protected override void Start () {
		base.Start();
		StartBehavior(Load());
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
					StartBehavior(Deposit());
				}
				break;
			case State.RETURNING:
				if (collider.gameObject == loadingZone) {
					StartBehavior(Load());
				}
				break;
			case State.LOADING:
			case State.DEPOSITING:
				break;
		}
	}

	void OnHit() {
		CancelBehavior();
		this.transform.position = this.loadingZone.transform.position;
		StartBehavior(Load());
	}

	IEnumerator Deposit() {
		currentState = State.DEPOSITING;
		yield return new WaitForSeconds(deliverTime);
		player.currentResources += resourcesToGive;
		currentState = State.RETURNING;
		currentBehavior = null;
	}

	IEnumerator Load() {
		currentState = State.LOADING;
		yield return new WaitForSeconds(loadTime);
		currentState = State.DELIVERING;
		currentBehavior = null;
	}
}
