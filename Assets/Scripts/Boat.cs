using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : Actor {
	public Sprite side;
	public Sprite back;

	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;

	private Vector2 facing;
	private bool followingEdge;
	private static float speed = 1f / Constants.PIXEL_SIZE;

	protected virtual void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	Vector2 Clockwise(Vector2 direction) {
		if (direction.x > 0) { 
			return Vector2.down;
		} else if (direction.y < 0) {
			return Vector2.left;
		} else if (direction.x < 0) { 
			return Vector2.up;
		} else {
			return Vector2.right;
		}
	}

	Vector2 CounterClockwise(Vector2 direction) {
		if (direction.x > 0) { 
			return Vector2.up;
		} else if (direction.y < 0) {
			return Vector2.right;
		} else if (direction.x < 0) { 
			return Vector2.down;
		} else {
			return Vector2.left;
		}
	}

	bool CanMoveIn(Vector2 direction) {
		Vector2 center = (Vector2)boxCollider.bounds.center + direction;
		Vector2 size = boxCollider.bounds.size;
		foreach (Collider2D collider in Physics2D.OverlapBoxAll(center, size, 0f, LayerMask.GetMask("Island", "Boat"))) {
			if (collider.gameObject == this.gameObject) { continue; }

			// Debug.Log(name + " colliding with " + collider.gameObject.name + " in direction " + direction);
			return false;
		}
		return true;
	}

	public bool Move(Vector2 direction) {
		if (!CanMoveIn(direction)) {
			return false;
		}

		SetFacing(direction);
		transform.position += (Vector3)direction;
		return true;
	}

	public void MoveTowards(GameObject dest) {
		// Debug.Log(name + " moving...");

		// first finish following the current edge
		if (followingEdge) {
			// Debug.Log(name + " can follow edge going " + facing + "?");
			Vector2 turnDirection = CounterClockwise(facing) * speed;
			if (Move(turnDirection)) {
				// Debug.Log(name + " turning to " + turnDirection + "!");
				followingEdge = false;
				return;
			}

			if (Move(facing)) {
				// Debug.Log(name + " following edge!");
				return;
			}
		}

		// if no current edge, try moving toward our target
		Vector2 direction = GetDirection(dest) * speed;
		if (Move(direction)) {
			// Debug.Log(name + " following target!");
			followingEdge = false;
			return;
		}

		// if we can't move toward our target or follow a edge, rotate until we can move
		// Debug.Log(name + " moving around obstacle...");
		direction = facing;
		followingEdge = true;
		for (int i = 0; i < 3; ++i) {
			direction = Clockwise(direction) * speed;
			// Debug.Log(name + " trying " + direction + " instead...");
			if (Move(direction)) {
				// Debug.Log(name + " found edge!");
				return;
			}
		}
		Debug.Log(name + " could not move!");
	}

	public Vector2 GetDirection(GameObject dest) {
		Vector2 direction = dest.transform.position - transform.position;
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
			return Mathf.Sign(direction.x) * Vector2.right;
		} else {
			return Mathf.Sign(direction.y) * Vector2.up;
		}
	}

	public void SetFacing(Vector2 direction) {
		if (direction.x != 0.0f) {
			spriteRenderer.sprite = side;
		} else {
			spriteRenderer.sprite = back;
		}
		facing = direction;
	}
}
