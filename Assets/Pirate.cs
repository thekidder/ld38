using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetComparer : IComparer  
{
	private Pirate self;

	public TargetComparer(Pirate self) { this.self = self; }

  public int Compare(System.Object x, System.Object y)  
  {
		GameObject lhs = (GameObject)x;
		GameObject rhs = (GameObject)y;

		Merchant lhsMerchant = lhs.GetComponent<Merchant>();
		Merchant rhsMerchant = rhs.GetComponent<Merchant>();

		if (lhsMerchant != null && lhsMerchant.resourcesToGive.diamonds > 0 &&
			(rhsMerchant == null || rhsMerchant.resourcesToGive.diamonds == 0)) {
			return -1;
		}

		if (rhsMerchant != null && rhsMerchant.resourcesToGive.diamonds > 0 &&
				(lhsMerchant == null || lhsMerchant.resourcesToGive.diamonds == 0)) {
			return 1;
		}

		if (lhsMerchant != null && self.IsInAttackRange(lhs) && 
				(rhsMerchant == null || !self.IsInAttackRange(rhs))) {
			return -1;
		}

		if (lhsMerchant != null && self.IsInAttackRange(lhs) && 
				(rhsMerchant == null || !self.IsInAttackRange(rhs))) {
			return 1;
		}

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
	public float timeBetweenTargeting;

	public int maxHp;

	public GameObject spawn;

	public GameObject cannonballPrefab;

	private TargetComparer targetComparer;

	private State currentState;
	private GameObject currentTarget;
	private int currentHp;

	private GameObject lastTargeter = null;
	private float lastTargetTime = 0f;
	
	protected override void Start() {
		base.Start();
		currentState = State.SEARCHING;
		targetComparer = new TargetComparer(this);
		currentHp = maxHp;
	}
	
	void FixedUpdate () {
		switch (currentState) {
			case State.SEARCHING:
				GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Piratable");
				Array.Sort(potentialTargets, targetComparer);

				foreach(GameObject target in potentialTargets) {
					PirateTarget pirateTarget = target.GetComponent<PirateTarget>();

					if (IsInSearchRange(target) && pirateTarget.value > 0) {
						Debug.Log("Search complete with " + target.name);
						currentTarget = target;
						currentState = State.APPROACHING;
					}
				}
				break;
			case State.APPROACHING:
				if (!IsInSearchRange(currentTarget)) {
					Debug.Log("Lost target due to range (approach)");
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
					Debug.Log("Lost target due to range (attack)");
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
		currentState = State.SEARCHING;
		Debug.Log("Fired, finding new target");
		currentTarget = null;

		currentBehavior = null;
	}

	bool IsInSearchRange(GameObject target) {
		if (target == null) { return false; }

		Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);

		if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) {
			return false;
		}

		Vector2 direction = (Vector2)transform.position - (Vector2)target.transform.position;

		return direction.magnitude < searchRange;
	}

	public void OnTargeted(GameObject source) {
		if (Time.time - lastTargetTime > timeBetweenTargeting) {
			Debug.Log("TARGETED. Going to attack " + source.name);
			CancelBehavior();
			currentState = State.APPROACHING;
			currentTarget = source;

			lastTargetTime = Time.time;
			lastTargeter = source;
		}
	}

	public void OnHit() {
		Debug.Log("onHit pirate");
		currentHp--;

		if (currentHp == 0) {
			CancelBehavior();
			Vector3 spawnPos = spawn.transform.position;
			spawnPos.z = transform.position.z;
			transform.position = spawnPos;
			currentState = State.SEARCHING;
			currentHp = maxHp;
		}
	}

	bool HasClearShot(Vector2 v, GameObject target) {
		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, v.normalized, v.magnitude, LayerMask.GetMask("Boat", "Building"))) {
			if (hit.collider.gameObject == target || hit.collider.gameObject == this.gameObject) { continue; }

			return false;
		}

		return true;

	}

	public bool IsInAttackRange(GameObject target) {
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
