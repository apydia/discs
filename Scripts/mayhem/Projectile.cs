using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Vector3 origin;
	public Vector3 destination;
	public double createTime = 0;
	public int playerID;

	public float speed = 300f;

	public float timeToLive = 10f;

	private Vector3 direction;

	public void Start () {
		speed = 30f;
		timeToLive = 10f;

		direction = (destination - origin);
		direction.Normalize ();
		/*
		float timePassed = (float)(PhotonNetwork.time - createTime);
		RaycastHit obstacleHit;
		Physics.Raycast(origin, direction, out obstacleHit, timePassed * speed, 1 << 12);
		if (obstacleHit.collider != null) {
			Debug.Log (obstacleHit.distance + " " +obstacleHit.collider.gameObject + " " +obstacleHit.collider.tag);
			if (obstacleHit.collider.tag == "Player") {
				Debug.Log ("hit player");
			}
		}
*/
		//&&  ObstacleHit.transform != OurShip && ObstacleHit.transform == Objective);
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
				if (!other.gameObject.GetComponent<PlayerLogic>().isShieldOn) {
					other.gameObject.GetComponent<PlayerLogic>().LoseRandomFlagItem(other.gameObject.GetComponent<PlayerController>().playerID);
				}
			}
		}

	}
}
