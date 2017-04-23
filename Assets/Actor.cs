using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {
	protected IEnumerator currentBehavior = null;

	protected void StartBehavior(IEnumerator behavior) {
		if (currentBehavior != null) {
			Debug.LogWarning("Multiple active behaviors on " + name + "!");
		}
		StartCoroutine(behavior);
		currentBehavior = behavior;
	}

	protected void CancelBehavior() {
		if (currentBehavior != null) {
			StopCoroutine(currentBehavior);
		}
		currentBehavior = null;
	}
}
