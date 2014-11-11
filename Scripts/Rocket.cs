using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

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
	
	// Update is called once per frame
	public void Update () {
		float timePassed = (float)(PhotonNetwork.time - createTime);
		transform.position = origin + (direction * timePassed * speed);

		if (timePassed > timeToDestination) {
			GameObject expl = (GameObject)Instantiate (explosion, gameObject.transform.position, Quaternion.identity);
			expl.GetComponent<Explosion>().parentTransform = gameObject.transform.parent;
			GameObject.Destroy (gameObject);
		}

	}
	
	public void OnTriggerEnter(Collider other) {
		

		
	}
}
