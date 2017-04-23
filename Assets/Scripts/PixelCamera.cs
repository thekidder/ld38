using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelCamera : MonoBehaviour {
	public Canvas ui;

	private Camera myCamera;
	private float size;
	private RectTransform canvasRect;

	void Start () {
		myCamera = GetComponent<Camera>();
		size = myCamera.orthographicSize;
		canvasRect = ui.GetComponent<RectTransform>();
	}
	
	void Update () {
		float currentRatio = Screen.height / (2f * Constants.PIXEL_SIZE * size);
		int newRatio = (int)(currentRatio + 0.5f);
		float newSize = Screen.height / (2f * Constants.PIXEL_SIZE * newRatio);
		myCamera.orthographicSize = newSize;

		float aspectRatio = (float)Screen.width / Screen.height;
		canvasRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -newSize, newSize * 2f);
		canvasRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -newSize * aspectRatio, newSize * 2f * aspectRatio);
	}
}
