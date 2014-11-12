using UnityEngine;
using System.Collections;

public class MagneticActor : MonoBehaviour {

	public float power;
	public float radius;

	// Use this for initialization
	void Start () {
		power = -1000f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody && other.gameObject.tag == "Player") {

			float dist = Vector3.Distance(other.gameObject.transform.position, transform.position);

			Vector3 dir = other.gameObject.transform.position - transform.position;
			other.attachedRigidbody.AddForce (dir*power/(dist*dist));
			other.rigidbody.AddForce (dir);
		}
	}
}
