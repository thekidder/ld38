using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetComparer : IComparer  
{
   public int Compare(System.Object x, System.Object y)  
   {
       GameObject lhs = (GameObject)x;
			 GameObject rhs = (GameObject)y;

			 return -(lhs.GetComponent<PirateTarget>().value - rhs.GetComponent<PirateTarget>().value);
   }
}

public class Pirate : Boat {
	enum State {
		SEARCHING,
		APPROACHING,
		ATTACKING,
		COOLDOWN
	}

	public float searchRange;
	public float attackRange;
	public float cooldownTime;

	public GameObject cannonballPrefab;

	private TargetComparer targetComparer;

	private State currentState;
	private GameObject currentTarget;
	

	protected override void Start () {
		base.Start();
		currentState = State.SEARCHING;
		targetComparer = new TargetComparer();
	}
	
	void FixedUpdate () {
		switch (currentState) {
			case State.SEARCHING:
				GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Piratable");
				Array.Sort(potentialTargets, targetComparer);

				foreach(GameObject target in potentialTargets) {
					if (IsInSearchRange(target)) {
						currentTarget = target;
						currentState = State.APPROACHING;
					}
				}
				break;
			case State.APPROACHING:
				if (!IsInSearchRange(currentTarget)) {
					currentTarget = null;
					currentState = State.SEARCHING;
				} else if (IsInAttackRange(currentTarget)) {
					currentState = State.ATTACKING;
				} else {
					MoveTowards(currentTarget);
				}
				break;
			case State.ATTACKING:
				if (!IsInSearchRange(currentTarget)) {
					currentTarget = null;
					currentState = State.SEARCHING;
				} else if (!IsInAttackRange(currentTarget)) {
					currentState = State.APPROACHING;
				} else {
					StartBehavior(Fire());
				}
				break;
			case State.COOLDOWN:
				break;
		}
	}

	IEnumerator Fire() {
		GameObject cannonball = (GameObject)Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
		Vector2 velocity = (currentTarget.transform.position - transform.position).normalized;
		velocity *= 1f / Constants.PIXEL_SIZE;
		cannonball.GetComponent<Cannonball>().velocity = velocity;

		currentState = State.COOLDOWN;

		yield return new WaitForSeconds(cooldownTime);
		currentState = State.APPROACHING;
		currentBehavior = null;
	}

	bool IsInSearchRange(GameObject target) {
		Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);

		if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) {
			return false;
		}

		Vector2 direction = (Vector2)transform.position - (Vector2)target.transform.position;

		return direction.magnitude < searchRange;
	}

	bool IsInAttackRange(GameObject target) {
		Vector2 direction = (Vector2)transform.position - (Vector2)target.transform.position;

		if (direction.magnitude > attackRange) {
			return false;
		}

		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, direction.normalized, direction.magnitude, LayerMask.GetMask("Boat", "Island"))) {
			if (hit.collider.gameObject == target || hit.collider.gameObject == this.gameObject) { continue; }

			return false;
		}

		return true;
	}
}
