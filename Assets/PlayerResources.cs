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

	public static PlayerResources operator-(PlayerResources a, PlayerResources b) {
		int g = a.gold - b.gold;
		int d = a.diamonds - b.diamonds;

		if (g < 0) { g = 0; }
		if (d < 0) { d = 0; }
		return new PlayerResources(g, d);
	}

	public static bool operator>=(PlayerResources a, PlayerResources b) {
		return a.gold >= b.gold && a.diamonds >= b.diamonds;
	}

	public static bool operator<=(PlayerResources a, PlayerResources b) {
		return a.gold <= b.gold && a.diamonds <= b.diamonds;
	}

	public static bool operator<(PlayerResources a, PlayerResources b) {
		return a.gold < b.gold && a.diamonds < b.diamonds;
	}

	public static bool operator>(PlayerResources a, PlayerResources b) {
		return a.gold > b.gold && a.diamonds > b.diamonds;
	}
}
