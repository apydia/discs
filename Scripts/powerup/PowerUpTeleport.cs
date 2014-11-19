using UnityEngine;
using System.Collections;

public class PowerUpTeleport : PowerUpBase {

	public GameObject teleportEffect;

	public override string GetName() {
		return "PowerUpTeleport";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		SpellTeleport spell = new SpellTeleport();
		spell.SetCastingPlayerID(PhotonNetwork.player.ID);
		spell.SetReceivingPlayerID(PhotonNetwork.player.ID);
		spell.duration = 3f;
		spell.destination = pos;
		player.GetComponent<PlayerLogic>().AddSpell (spell);		
		/*
		GameObject obj = (GameObject)Instantiate(teleportEffect, player.transform.position, Quaternion.identity);
		obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
		player.transform.position = pos;
		Vector3 pos3 = obj.transform.position;
		pos3.y = -2f;
		obj.transform.position = pos3;
		obj = (GameObject)Instantiate(teleportEffect, player.transform.position, Quaternion.identity);
		obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
		Vector3 pos2 = obj.transform.position;
		pos2.y = -2f;
		obj.transform.position = pos2;*/

	}
}
