using UnityEngine;
using System.Collections;

public class PowerUpTeleport : PowerUpBase {
	
	public override string GetName() {
		return "PowerUpTeleport";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		/*Bomb bomb = new Bomb();
		bomb.position = player.transform.position;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(bomb, pos);*/
	}
}
