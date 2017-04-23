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
public class Tower : Actor {
	public enum State {
		SEARCHING,
		ATTACKING,
		COOLDOWN,
		WALL
	}

	public GameObject buildSite;

	public Sprite towerFreestanding;
	public Sprite towerBack;
	public Sprite towerBackRight;
	public Sprite towerBackLeft;
	public Sprite towerFront;

	public Sprite wallVertical;
	public Sprite wallHorizontal;

	public float attackRange;
	public float cooldownTime;
	public int maxHp;
	public GameObject cannonballPrefab;

	public State currentState;
	private PirateComparer pirateComparer;
	private GameObject currentTarget;
	private int currentHp;

	void Start () {
		currentState = State.SEARCHING;
		pirateComparer = new PirateComparer(this.gameObject);
		currentHp = maxHp;
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
					StartBehavior(Fire());
				}
				break;
			case State.COOLDOWN:
				break;
		}
	}

	bool findBuilding(Vector2 pos) {
		return Physics2D.OverlapPoint(pos, LayerMask.GetMask("Building")) != null;
	}

	public void Redisplay() {
		// Debug.Log(name + " REDISPLAY");

		bool hasTopNeighbor = findBuilding((Vector2)transform.position + Vector2.up);
		bool hasBottomNeighbor = findBuilding((Vector2)transform.position + Vector2.down);
		bool hasLeftNeighbor = findBuilding((Vector2)transform.position + Vector2.left);
		bool hasRightNeighbor = findBuilding((Vector2)transform.position + Vector2.right);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		gameObject.tag = "Piratable";
		currentState = State.SEARCHING;
		if (hasLeftNeighbor && hasRightNeighbor) {
			spriteRenderer.sprite = wallHorizontal;
			CancelBehavior();
			currentState = State.WALL;
			gameObject.tag = "Untagged";
		} else if (hasTopNeighbor && hasBottomNeighbor) {
			spriteRenderer.sprite = wallVertical;
			CancelBehavior();
			currentState = State.WALL;
			gameObject.tag = "Untagged";
		} else if (hasBottomNeighbor && hasLeftNeighbor) {
			spriteRenderer.sprite = towerBackRight;
		} else if (hasBottomNeighbor && hasRightNeighbor) {
			spriteRenderer.sprite = towerBackLeft;
		} else if (hasBottomNeighbor) {
			spriteRenderer.sprite = towerBack;
		} else if (hasTopNeighbor) {
			spriteRenderer.sprite = towerFront;
		} else {
			spriteRenderer.sprite = towerFreestanding;
		}
	}

	public void OnHit() {
		currentHp--;

		if (currentHp == 0) {
			if (buildSite != null) {
				buildSite.SetActive(true);
			}

			Destroy(this.gameObject);
			Utility.Redisplay();
		}
	}

	public void SetBuildSite(GameObject site) {
		buildSite = site;
	}

	IEnumerator Fire() {
		Debug.Log("Tower firing!");

		GameObject cannonball = (GameObject)Instantiate(cannonballPrefab, transform.position, Quaternion.identity);
		Vector2 velocity = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
		velocity *= (1f / Constants.PIXEL_SIZE);
		cannonball.GetComponent<Cannonball>().friendly = true;
		cannonball.GetComponent<Cannonball>().velocity = velocity;
		cannonball.GetComponent<Cannonball>().origin = this.gameObject;

		currentTarget.gameObject.SendMessage("OnTargeted", gameObject);

		currentState = State.COOLDOWN;
		yield return new WaitForSeconds(cooldownTime);
		currentState = State.ATTACKING;
		currentBehavior = null;
	}

	bool IsInAttackRange(GameObject target) {
		Vector2 direction = (Vector2)target.transform.position - (Vector2)transform.position;

		if (direction.magnitude > attackRange) {
			return false;
		}

		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, direction.normalized, direction.magnitude, LayerMask.GetMask("Boat", "Building"))) {
			if (hit.collider.gameObject == target || hit.collider.gameObject == this.gameObject) { continue; }

			// Debug.Log("Tower can't attack due to " + hit.collider.gameObject.name);

			return false;
		}

		return true;
	}
}
