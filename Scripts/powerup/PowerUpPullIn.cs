using UnityEngine;
using System.Collections;

public class PowerUpPullIn : PowerUpBase {

	public override string GetName() {
		return "PowerUpPullIn";
	}

	public override void Activate(GameObject player, Vector3 pos) {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty attractorStrength = main.GetPowerUpProperty("PowerUpPullIn", "strength");
		PowerUpProperty attractorRadius = main.GetPowerUpProperty("PowerUpPullIn", "radius");
		PowerUpProperty attractorTTL = main.GetPowerUpProperty("PowerUpPullIn", "time to live");
		MagneticAttractor attractor = new MagneticAttractor();
		attractor.power = -1f*attractorStrength.val;
		attractor.radius = attractorRadius.val;
		attractor.timeToLive = attractorTTL.val;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(attractor, pos);
	}
}