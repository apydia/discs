using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Vector3 origin;
	public Vector3 destination;
	public double createTime = 0;
	public int playerID;

	public float speed = 30f;

	public float timeToLive = 10f;

	private Vector3 direction;

	public void Start () {
		direction = (destination - origin);
		direction.Normalize ();
	
		speed = 30f;
		timeToLive = 10f;
	}
	
	// Update is called once per frame
	public void Update () {
		if (direction != null) {
			float timePassed = (float)(PhotonNetwork.time - createTime);
			transform.position = origin + (direction * timePassed * speed);
			if (timePassed > timeToLive) {
				GameObject.Destroy(this.gameObject);
			}
		}
	}

	public void OnTriggerEnter(Collider other) {
		
		if(other.gameObject.tag == "Player") {
			if (playerID != other.gameObject.GetComponent<PlayerController>().playerID) {
				other.gameObject.GetComponent<PlayerLogic>().LoseRandomFlagItem(other.gameObject.GetComponent<PlayerController>().playerID);
			}
		}

	}
}
