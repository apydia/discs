using UnityEngine;
using System.Collections;

public class MagneticActor : Photon.MonoBehaviour, Spawnable {

	public float power;
	public float radius;
	public float timeToLive;
	//TODO: time to live...

	float createTime;

	public GameObject effect;
	GameObject spawnedEffect;

	bool effectSpawned = false;

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<SphereCollider>().radius = radius;
		if (!effectSpawned) {
			spawnedEffect = (GameObject)Instantiate(effect, transform.position, Quaternion.identity);
			Vector3 pos = spawnedEffect.transform.position;
			pos.y = 4f;
			spawnedEffect.transform.position = pos;
			effectSpawned = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ((float)PhotonNetwork.time - createTime > timeToLive) {
			GameObject.Destroy(spawnedEffect.gameObject);
			GameObject.Destroy(this.gameObject);
		}
	}

	public int id;
	
	public string GetName() {
		return "MagneticActor";
	}
	/**
	 * unique id
	 * */
	public int GetId() {
		return id;
	}
	/*
	 * this is a small serializable implementation
	 * collects all data for this object, sends it over network
	 * and passes it to the init funciton on the other side
	 * */
	public object[] GatherInitData() {
		return new object[] {power, radius, timeToLive};
	}
	/**
	 * deserialization method
	 * */
	public void Init(int id, object[] initData, double createTime) {
		this.id = id;
		this.power = (float) initData[0];
		this.radius = (float) initData[1];
		this.timeToLive = (float) initData[2];
		this.createTime = (float)createTime;
		Start ();
	}

	void CheckCollision(Collider other) {
		if (other.attachedRigidbody && other.gameObject.tag == "Player") {
			float dist = Vector3.Distance(other.gameObject.transform.position, transform.position);
			
			Vector3 dir = other.gameObject.transform.position - transform.position;
			other.attachedRigidbody.AddForce (dir*power/(dist));
			//other.rigidbody.AddForce (dir);
		}
	}

	void OnTriggerEnter(Collider other) {
		CheckCollision(other);
	}

	void OnTriggerStay(Collider other) {
		CheckCollision(other);
	}
}
