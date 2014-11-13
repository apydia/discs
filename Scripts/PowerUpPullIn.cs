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
		MagneticActor actor = new MagneticActor();
		actor.power = -1f*attractorStrength.val;
		actor.radius = attractorRadius.val;
		actor.timeToLive = attractorTTL.val;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(actor, pos);
		//.Find ("Scripts").GetComponent<MayhemSpawner>().SpawnBomb(pos);
	}
}