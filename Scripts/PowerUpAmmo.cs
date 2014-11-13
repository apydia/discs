using UnityEngine;
using System.Collections;

public class PowerUpAmmo : PowerUpBase {

	public override string GetName() {
		return "PowerUpAmmo";
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController>().gunAmmoAmount += 10;
			other.gameObject.GetComponent<PlayerLogic>().PowerUpDestroyed(id);
		}
	}
}
