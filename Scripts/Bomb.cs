using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour, Spawnable {

	public GameObject explosion;

	public Vector3 position;
	public double createTime;
	public float explodeDelay = 5f;

	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty expDelay = main.GetPowerUpProperty("PowerUpBomb", "explosion delay");

		this.explodeDelay = expDelay.val;
	}

	public bool freshCreated = true;

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "DiscSlice") {
			this.gameObject.transform.parent = other.gameObject.GetComponent<DiscSlice>().sliceMesh.transform;

			if (freshCreated) {
				freshCreated = false;
				GameObject slice = other.gameObject;

				gameObject.transform.parent = slice.GetComponent<DiscSlice>().sliceMesh.transform;
				float timePassed = (float)(PhotonNetwork.time - createTime );
				if (!PhotonNetwork.isMasterClient) {
					timePassed -= PhotonNetwork.GetPing() * 0.001f;
				}
				float speed = slice.GetComponent<DiscSlice>().speed;
				gameObject.transform.position = Quaternion.AngleAxis(speed * timePassed, Vector3.up) * gameObject.transform.position;
			}
		}
	}

	public int id;
	
	public string GetName() {
		return "Bomb";
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
		return new object[] {position};
	}
	/**
	 * deserialization method
	 * */
	public void Init(int id, object[] initData, double createTime) {
		this.position = (Vector3) initData[0];
		this.createTime = createTime;
	}

	// Update is called once per frame
	void Update () {
		if ((float)(PhotonNetwork.time - createTime) > explodeDelay) {
			GameObject.Destroy(gameObject);
			GameObject expl = (GameObject)Instantiate (explosion, gameObject.transform.position, Quaternion.identity);
			expl.GetComponent<Explosion>().parentTransform = gameObject.transform.parent;
		}
	}
}
