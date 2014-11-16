using UnityEngine;
using System.Collections;

public class PushAway : Photon.MonoBehaviour {

	float size;
	public float strength;
	public float fireTime;

	public GameObject effect;
	
	public GameObject player;

	// Use this for initialization
	void Start () {
		size = 9f;
		strength = 9000f;
	}

	void CheckCollision(Collider other) {
		if (other.attachedRigidbody && other.gameObject.tag == "Player" && other.gameObject.name != player.gameObject.name) {
			Debug.Log ("collision " + player + " " + other.gameObject.transform.position + " " + player.gameObject.transform.position);
			float dist = Vector3.Distance(other.gameObject.transform.position, player.gameObject.transform.position);
			Debug.Log ("dist: " + dist);
			Vector3 dir = (other.gameObject.transform.position - player.gameObject.transform.position).normalized;
			Debug.Log ("dir: " + dir);
			other.attachedRigidbody.AddForce(dir * strength / (dist+1f));
		}
	}

	void OnTriggerEnter(Collider other) {
		CheckCollision(other);
	}

	void OnTriggerStay(Collider other) {
		CheckCollision(other);
	}
	/*
	void OnCollisionEnter(Collision other) {
		CheckCollision(other.collider);
	}
	
	void OnCollisionStay(Collision other) {
		CheckCollision(other.collider);
	}*/

	//TODO: move to util class!!
	Vector3 ProjectMousePosition(Vector3 mousePos) {
		Ray ray = Camera.main.ScreenPointToRay (mousePos);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 2));
		return hit.point;
	}

	bool isInited = false;
	int playerID;
	GameObject spawnedEffect; 
	float createTime;
	// Update is called once per frame
	void Update () {
		if (!isInited) {
			isInited = true;
			createTime = (float)PhotonNetwork.time;

			playerID = (int) photonView.instantiationData[0];
			strength = (float) photonView.instantiationData[1];
			fireTime = (float) photonView.instantiationData[2];
			player = GameObject.Find ("Player"+playerID);
			//transform.parent = player.transform;
			transform.localScale = new Vector3(3f, size, 3f);
			spawnedEffect = (GameObject)Instantiate(effect, transform.position, Quaternion.identity);
			spawnedEffect.transform.parent = transform;
			spawnedEffect.transform.Rotate(new Vector3(-90f, 0f, 0f));
			spawnedEffect.transform.Translate (new Vector3(0f, -2f, -5f));
		}

		float timePassed = (float)PhotonNetwork.time - createTime;
		if (timePassed > fireTime) {
			if (photonView.isMine) {
				PhotonNetwork.Destroy (this.gameObject);
			} else {
				GameObject.Destroy(spawnedEffect.gameObject);
			}
		}

		if (player != null) {
			transform.position = player.transform.position;
		}
		transform.Translate(0f ,-size, 0f);

		if (photonView.isMine && player != null) {
			GameMain main = GameObject.Find ("Scripts").GetComponent<GameMain>();
			Vector3 hitPoint = main.mouseVec;//ProjectMousePosition(Input.mousePosition);
			transform.rotation = Quaternion.LookRotation(hitPoint - player.gameObject.transform.position);
			transform.Rotate (new Vector3(-90f, 0f, 0f));
			//
			//transform.Translate(0f, -size, 0f);

			//transform.position = player.transform.position;
			//transform.position = Vector3.Lerp(transform.position, player.GetComponent<PlayerLogic>().extPos, Time.deltaTime * 8);
		} else {
			//transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 8);
			//transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 8);
		}

	}
}
