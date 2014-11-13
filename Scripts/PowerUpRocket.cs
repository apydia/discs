using UnityEngine;
using System.Collections;

public class PowerUpRocket : PowerUpBase {
	
	public override string GetName() {
		return "PowerUpRocket";
	}

	public override void Activate(GameObject player, Vector3 pos) {
		Rocket rocket = new Rocket();
		rocket.origin = player.transform.position;
		rocket.destination = pos;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(rocket, player.transform.position);
		//.Find ("Scripts").GetComponent<MayhemSpawner>().SpawnBomb(pos);
	}
}