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
		Vector3 pos = transform.position;
		pos.y = 5;
		transform.position = pos;
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
		if (other.gameObject.tag == "PlayerSelectCollider") {
			Debug.Log ("spell cast on player: " + other.gameObject.GetComponent<PlayerSelectCollider>());
			GameObject playerCasting = GameObject.Find ("Player"+this.castingPlayerID);
			Cast (playerCasting, other.gameObject.GetComponent<PlayerSelectCollider>().player);
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
