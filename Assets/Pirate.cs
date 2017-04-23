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

	public GameObject spawn;

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
		Vector2 velocity = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
		velocity *= (1f / Constants.PIXEL_SIZE);
		cannonball.GetComponent<Cannonball>().friendly = false;
		cannonball.GetComponent<Cannonball>().velocity = velocity;
		cannonball.GetComponent<Cannonball>().origin = this.gameObject;

		currentState = State.COOLDOWN;
		yield return new WaitForSeconds(cooldownTime);
		currentState = State.APPROACHING;

		currentBehavior = null;
	}

	bool IsInSearchRange(GameObject target) {
		if (target == null) { return false; }

	  if(target.GetComponent<PirateTarget>().value <= 0) {
	  	 return false;
	  }

		Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);

		if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) {
			return false;
		}

		Vector2 direction = (Vector2)transform.position - (Vector2)target.transform.position;

		return direction.magnitude < searchRange;
	}

	void OnHit() {
		Debug.Log("onHit pirate");
		CancelBehavior();
		Vector3 spawnPos = spawn.transform.position;
		spawnPos.z = transform.position.z;
		transform.position = spawnPos;
		currentState = State.SEARCHING;
	}

	bool HasClearShot(Vector2 v, GameObject target) {
		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, v.normalized, v.magnitude, LayerMask.GetMask("Boat", "Building"))) {
			if (hit.collider.gameObject == target || hit.collider.gameObject == this.gameObject) { continue; }

			return false;
		}

		return true;

	}

	bool IsInAttackRange(GameObject target) {
		Vector2 direction = (Vector2)target.transform.position - (Vector2)transform.position;

		if (direction.magnitude > attackRange) {
			return false;
		}

		if (!HasClearShot(direction, target)) {
			return false;
		}

		// check in a cone to have a better shot of not getting stuck
		float epsilonDegrees = 5f;
		if (!HasClearShot(direction.Rotate(-epsilonDegrees), target)) {
			return false;
		}

		if (!HasClearShot(direction.Rotate(epsilonDegrees), target)) {
			return false;
		}

		return true;
	}
}
