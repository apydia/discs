using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float explodeDuration = 0.5f;
	public float explodeRadius = 15f;
	public float explodeStrength = 80000f;
	public Transform parentTransform;
	public GameObject explosionEffect;
	public GameObject magneticActor;

	// Use this for initialization
	void Start () {
		explodeDuration = 0.3f;
		//transform.parent = parentTransform;
		renderer.material.color = new Color(0.5f,0.5f,0.5f,0.2f);
		GameObject obj = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);
		obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
		Vector3 pos = obj.transform.position;
		pos.y = 2f;
		obj.transform.position = pos;
	}

	float explodeStartTime = 0f;
	Vector3 startScale;
	Vector3 startPos;
	bool isExploded = false;

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerLogic>().LoseAllFlagItems();
		}
	}

	// Update is called once per frame
	void Update () {
		if (!isExploded) {
			Collider collider = gameObject.GetComponent<Collider>();
			if (explodeStartTime == 0f) {
				explodeStartTime = Time.time;
				startScale = collider.transform.localScale;
				startPos = transform.position;
				GameObject boom = (GameObject)Instantiate(magneticActor, gameObject.transform.position, Quaternion.identity);
				boom.GetComponent<MagneticActor>().power = explodeStrength;
				boom.GetComponent<MagneticActor>().radius = explodeRadius;
				boom.GetComponent<MagneticActor>().timeToLive = explodeDuration;
			}
			float timePassed = Time.time - explodeStartTime;

			//transform.localScale = startScale * (1f + (timePassed / explodeDuration) * explodeRadius);

			if (timePassed > explodeDuration) {
				GameObject.Destroy(gameObject);
				isExploded = true;
			}
		}
	}
}