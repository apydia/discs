using UnityEngine;
using System.Collections;

public class PowerUpGemRemover : PowerUpBase {
	
	public GameObject spellCollider;
	
	public override string GetName() {
		return "PowerUpGemRemover";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		GameObject obj = (GameObject) Instantiate (spellCollider, pos, Quaternion.identity);
		obj.GetComponent<SpellCollider>().castingPlayerID = PhotonNetwork.player.ID;
		obj.GetComponent<SpellCollider>().spellName = "SpellGemRemover";
		SpellGemRemover spell = new SpellGemRemover();
		spell.duration = 5f;
		obj.GetComponent<SpellCollider>().spell = spell;
	}
}