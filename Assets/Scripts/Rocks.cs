using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocks : MonoBehaviour {
	public float liveTime;

	void Start () {
		StartCoroutine(Die());
	}

	IEnumerator Die() {
		yield return new WaitForSeconds(liveTime);
		Destroy(gameObject);
	}
	
	void Update () {
		
	}
}
