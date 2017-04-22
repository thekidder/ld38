using UnityEngine;

[System.Serializable]
public class PlayerResources : System.Object {
	public PlayerResources(int g, int d) {
		gold = g;
		diamonds = d;
	}

	public int gold;
	public int diamonds;

	public static PlayerResources operator+(PlayerResources a, PlayerResources b) {
		return new PlayerResources(a.gold + b.gold, a.diamonds + b.diamonds);
	}
}
