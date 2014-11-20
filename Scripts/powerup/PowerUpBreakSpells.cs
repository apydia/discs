using UnityEngine;
using System.Collections;

public class PowerUpBreakSpells : PowerUpBase {
	
	public GameObject spellCollider;
	
	public override string GetName() {
		return "PowerUpBreakSpells";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		GameObject obj = (GameObject) Instantiate (spellCollider, pos, Quaternion.identity);
		obj.GetComponent<SpellCollider>().castingPlayerID = PhotonNetwork.player.ID;
		obj.GetComponent<SpellCollider>().spellName = "SpellBreakSpells";
		SpellBreakSpells spell = new SpellBreakSpells();
		spell.duration = 3f;
		obj.GetComponent<SpellCollider>().spell = spell;
	}
}