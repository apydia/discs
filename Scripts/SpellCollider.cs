using UnityEngine;
using System.Collections;

public class SpellCollider : MonoBehaviour {

	double createTime;
	public int castingPlayerID;

	public string spellName;
	float timeToLive = 1f;

	public Spell spell;

	// Use this for initialization
	void Start () {
		castTime = Time.time;
	}

	bool isCast = false;
	float castTime;

	public void Cast (GameObject playerCasting, GameObject playerReceiving)
	{
		if (!isCast) {
			spell.SetCastingPlayerID(castingPlayerID);
			spell.SetReceivingPlayerID(playerReceiving.GetComponent<PlayerController>().playerID);
			playerReceiving.GetComponent<PlayerLogic>().AddSpell (spell);
			gameObject.GetComponent<SphereCollider>().enabled = false;
			isCast = true;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			Debug.Log ("spell cast on player: " + other.gameObject.GetComponent<PlayerController>().playerID);
			GameObject playerCasting = GameObject.Find ("Player"+this.castingPlayerID);
			Cast (playerCasting, other.gameObject);
		}
	}

	bool isDestroyed = false;

	// Update is called once per frame
	void Update () {
		if (Time.time > castTime + timeToLive && !isDestroyed) {
			GameObject.Destroy(this.gameObject);
			isDestroyed = true;
		}
	}
}
