using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDepth : MonoBehaviour {
	void Start () {
	}
	
	void Update () {
		
	}

	public void Redisplay() {
		Vector3 pos = transform.position;
		pos.z = pos.y / 10f - 4f;

		transform.position = pos;
	}
}
