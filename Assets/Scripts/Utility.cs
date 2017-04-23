using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility  {
	public static void Redisplay() {
		foreach(Component wall in GameObject.FindObjectsOfType(typeof(Wall))) {
			wall.gameObject.SendMessage("Redisplay");
		}

		foreach(Component tower in GameObject.FindObjectsOfType(typeof(Tower))) {
			tower.gameObject.SendMessage("Redisplay");
		}
	}
}
