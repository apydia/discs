using UnityEngine;
using System.Collections;

public class MayhemSpawner : Photon.MonoBehaviour {

	public GameObject bomb;
	public GameObject rocket;

	// Use this for initialization
	void Start () {
	
	}

	void CreateBomb(Vector3 destination, double startTime) {
		//destination.y = 20f;
		GameObject b = (GameObject) Instantiate (bomb, destination, Quaternion.identity); 
		b.GetComponent<Bomb>().position = destination; 
		b.GetComponent<Bomb>().createTime = startTime; 
	}

	[RPC]
	void SpawnBombRPC(Vector3 destination, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		CreateBomb (destination, timestamp);
		
	}

	public void SpawnBomb(Vector3 destination) {
		photonView.RPC ("SpawnBombRPC", PhotonTargets.All, destination);
	}

	void CreateRocket(Vector3 origin, Vector3 destination, double startTime) {
		//destination.y = 20f;
		GameObject r = (GameObject) Instantiate (rocket, origin, Quaternion.identity); 
		r.GetComponent<Rocket>().origin = origin; 
		r.GetComponent<Rocket>().destination = destination; 
		r.GetComponent<Rocket>().createTime = startTime; 
	}
	
	[RPC]
	void SpawnRocketRPC(Vector3 origin, Vector3 destination, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		CreateRocket (origin, destination, timestamp);
		
	}
	
	public void SpawnRocket(Vector3 origin, Vector3 destination) {
		photonView.RPC ("SpawnRocketRPC", PhotonTargets.All, origin, destination);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
