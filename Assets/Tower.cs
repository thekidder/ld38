using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateComparer : IComparer {
	private GameObject self;

	public PirateComparer(GameObject self) {
		this.self = self;
	}

  public int Compare(System.Object x, System.Object y)  
  {
		GameObject lhs = (GameObject)x;
		GameObject rhs = (GameObject)y;

		return (int)Mathf.Sign((self.transform.position - lhs.transform.position).magnitude - (self.transform.position - rhs.transform.position).magnitude);
  }
}
public class Tower : MonoBehaviour {
	enum State {
		SEARCHING,
		ATTACKING,
		COOLDOWN
	}

	public float attackRange;
	public float cooldownTime;
	public GameObject cannonballPrefab;

	private State currentState;
	private PirateComparer pirateComparer;
	private GameObject currentTarget;

	void Start () {
		currentState = State.SEARCHING;
		pirateComparer = new PirateComparer(this.gameObject);
	}
	
	void FixedUpdate () {
		switch (currentState) {
			case State.SEARCHING:
				GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Pirate");
				Array.Sort(potentialTargets, pirateComparer);

				foreach(GameObject target in potentialTargets) {
					if (IsInAttackRange(target)) {
						currentTarget = target;
						currentState = State.ATTACKING;
					}
				}
				break;
			case State.ATTACKING:
				if (currentTarget == null) {
					currentState = State.SEARCHING;
				} else if (!IsInAttackRange(currentTarget)) {
					currentState = State.SEARCHING;
				} else {
					StartCoroutine(Fire());
				}
				break;
			case State.COOLDOWN:
				break;
		}
	}

	IEnumerator Fire() {
		GameObject cannonball = (GameObject)Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
		Vector2 velocity = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
		velocity *= (1f / Constants.PIXEL_SIZE);
		cannonball.GetComponent<Cannonball>().friendly = true;
		cannonball.GetComponent<Cannonball>().velocity = velocity;

		currentState = State.COOLDOWN;
		yield return new WaitForSeconds(cooldownTime);
		currentState = State.ATTACKING;
	}

	bool IsInAttackRange(GameObject target) {
		Vector2 direction = (Vector2)transform.position - (Vector2)target.transform.position;

		if (direction.magnitude > attackRange) {
			return false;
		}

		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, direction.normalized, direction.magnitude, LayerMask.GetMask("Boat", "Building"))) {
			if (hit.collider.gameObject == target || hit.collider.gameObject == this.gameObject) { continue; }

			return false;
		}

		return true;
	}
}
