using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour, Spawnable {

	public GameObject explosion;

	public Vector3 origin;
	public Vector3 destination;
	public double createTime;
	public int playerID;
	
	public float speed = 60f;

	public float timeToDestination;
	
	private Vector3 direction;
	// Use this for initialization
	public void Start () {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty rocketSpeed = main.GetPowerUpProperty("PowerUpRocket", "speed");
		this.speed = rocketSpeed.val;

		direction = (destination - origin);
		direction.Normalize ();
		transform.rotation = Quaternion.LookRotation (-direction);

		timeToDestination = Vector3.Distance(destination, origin) / speed;
	}

	bool isExploded = false;

	// Update is called once per frame
	public void Update () {
		float timePassed = (float)(PhotonNetwork.time - createTime);
		transform.position = origin + (direction * timePassed * speed);

		if (timePassed > timeToDestination && !isExploded) {
			isExploded = true;
			GameObject obj = GameObject.Find ("Scripts");
			GameMain main = obj.GetComponent<GameMain>();
			PowerUpProperty strength = main.GetPowerUpProperty("PowerUpRocket", "explosion strength");
			PowerUpProperty radius = main.GetPowerUpProperty("PowerUpRocket", "explosion radius");
			GameObject expl = (GameObject)Instantiate (explosion, gameObject.transform.position, Quaternion.identity);
			expl.GetComponent<Explosion>().explodeStrength = strength.val;
			expl.GetComponent<Explosion>().explodeRadius = radius.val;
			GameObject.Destroy (gameObject);
		}
	}

	public int id;

	public string GetName() {
		return "Rocket";
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
		return new object[] {origin, destination};
	}
	/**
	 * deserialization method
	 * */
	public void Init(int id, object[] initData, double createTime) {
		this.id = id;
		this.origin = (Vector3) initData[0];
		this.destination = (Vector3) initData[1];
		this.createTime = createTime;
		Start ();
	}

	public void OnTriggerEnter(Collider other) {
		
	}
}
