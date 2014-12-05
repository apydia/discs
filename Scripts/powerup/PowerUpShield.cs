using UnityEngine;
using System.Collections;

public class PowerUpShield : PowerUpBase {
	
	public override string GetName() {
		return "PowerUpShield";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty duration = main.GetPowerUpProperty("PowerUpShield", "duration");
		player.GetComponent<PlayerLogic>().ActivateShield(duration.val);
	}

	public override void DeActivate(GameObject player) {
		player.GetComponent<PlayerLogic>().DeActivateShield();
	}
}