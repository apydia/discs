using UnityEngine;
using System.Collections;

public class MagneticAttractor : Photon.MonoBehaviour, Spawnable {
	
	public float power;
	public float radius;
	public float timeToLive;
	//TODO: time to live...
	
	float createTime;

	public GameObject magneticActor;
	public GameObject effect;
	GameObject spawnedEffect;
	GameObject spawnedActor;

	bool spawned = false;
	
	// Use this for initialization
	void Start () {
		if (!spawned) {
			spawnedEffect = (GameObject)Instantiate(effect, transform.position, Quaternion.identity);
			Vector3 pos = spawnedEffect.transform.position;
			pos.y = 4f;
			spawnedEffect.transform.position = pos;
			spawned = true;
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
		return "MagneticAttractor";
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
		spawnedActor = (GameObject)Instantiate(magneticActor, transform.position, Quaternion.identity);
		spawnedActor.GetComponent<MagneticActor>().radius = radius;
		spawnedActor.GetComponent<MagneticActor>().power = power;
		spawnedActor.GetComponent<MagneticActor>().timeToLive = timeToLive;
		spawnedActor.GetComponent<MagneticActor>().method = "strongInMiddle";
		Start ();
	}
}
