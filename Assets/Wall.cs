﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
	public Sprite wallHorizontal;
	public Sprite wallVertical;
	public Sprite wallFrontRight;
	public Sprite wallFrontLeft;
	public Sprite wallFront;
	public Sprite wallBackRight;
	public Sprite wallBackLeft;
	public Sprite wallBack;
	public Sprite wallLeft;
	public Sprite wallFreestanding;

	void Start () {
		
	}
	
	void Update () {
		
	}

	bool findBuilding(Vector2 pos) {
		return Physics2D.OverlapPoint(pos, LayerMask.GetMask("Building")) != null;
	}

	public void Redisplay() {
		bool hasTopNeighbor = findBuilding((Vector2)transform.position + Vector2.up);
		bool hasBottomNeighbor = findBuilding((Vector2)transform.position + Vector2.down);
		bool hasLeftNeighbor = findBuilding((Vector2)transform.position + Vector2.left);
		bool hasRightNeighbor = findBuilding((Vector2)transform.position + Vector2.right);

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (hasLeftNeighbor && hasRightNeighbor) {
			spriteRenderer.sprite = wallHorizontal;
		} else if (hasTopNeighbor && hasBottomNeighbor) {
			spriteRenderer.sprite = wallVertical;
		} else if (hasBottomNeighbor && hasLeftNeighbor) {
			spriteRenderer.sprite = wallBackRight;
		} else if (hasBottomNeighbor && hasRightNeighbor) {
			spriteRenderer.sprite = wallBackLeft;
		} else if (hasBottomNeighbor) {
			spriteRenderer.sprite = wallBack;
		} else if (hasTopNeighbor && hasLeftNeighbor) {
			spriteRenderer.sprite = wallFrontRight;
		} else if (hasTopNeighbor && hasRightNeighbor) {
			spriteRenderer.sprite = wallFrontLeft;
		} else if (hasTopNeighbor) {
			spriteRenderer.sprite = wallFront;
		} else if (hasRightNeighbor) {
			spriteRenderer.sprite = wallLeft;
		} else {
			spriteRenderer.sprite = wallFreestanding;
		}
	}

}
