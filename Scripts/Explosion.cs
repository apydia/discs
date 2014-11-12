using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float explodeDuration = 0.1f;
	public float explodeRadius = 15f;
	public Transform parentTransform;
	public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
		transform.parent = parentTransform;
		renderer.material.color = new Color(0.5f,0.5f,0.5f,0.2f);
		GameObject obj = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);
		obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
		Vector3 pos = obj.transform.position;
		pos.y = 2f;
		obj.transform.position = pos;
	}

	double explodeStartTime = 0;
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
			if (explodeStartTime == 0) {
				explodeStartTime = Time.time;
				startScale = collider.transform.localScale;
				startPos = transform.position;
			}
			float timePassed = (float)(Time.time - explodeStartTime);
			//collider.transform.localScale = startScale * (1f + (timePassed / explodeDuration) * explodeRadius);
			//rigidbody.transform.localScale = startScale * (1f + (timePassed / explodeDuration) * explodeRadius);

			transform.localScale = startScale * (1f + (timePassed / explodeDuration) * explodeRadius);

			//transform.position 
			if (timePassed > explodeDuration) {
				GameObject.Destroy(gameObject);
				isExploded = true;
			}
		}
	}
}
