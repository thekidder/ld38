using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCamera : MonoBehaviour {
	private Camera myCamera;

	void Start () {
		myCamera = GetComponent<Camera>();
	}
	
	void Update () {
		float currentRatio = Screen.height / (2f * Constants.PIXEL_SIZE * myCamera.orthographicSize);
		int newRatio = (int)currentRatio;
		float newSize = Screen.height / (2f * Constants.PIXEL_SIZE * newRatio);
		myCamera.orthographicSize = newSize;
	}
}
