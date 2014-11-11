using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public Vector3 destination;
	public Vector3 startPos;

	public float speed;

	Vector3 moveVec;
	float spawnTime;
	float eta;

	// Use this for initialization
	void Start () {
		moveVec = destination - startPos;
		moveVec.Normalize();

		float dist = Vector3.Distance(destination, startPos);

		spawnTime = Time.time;
		eta = spawnTime + dist / speed;
	}
	
	// Update is called once per frame
	void Update () {
		float timePassed = Time.time - spawnTime;
		if (Time.time < eta) {
			transform.position = startPos + moveVec * speed * timePassed;
		}
		//transform.Rotate (new Vector3(0.0f, 1f, 0.0f));
	}
}
