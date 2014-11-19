using UnityEngine;
using System.Collections;

public class TeleportBeacon : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "DiscSlice") {
			Debug.Log ("trigger");
			this.transform.parent = other.gameObject.GetComponent<DiscSlice>().sliceMesh.transform;
		}
	}

	void OnCollisionStay(Collision other) {
		if (other.gameObject.tag == "DiscSlice") {
			Debug.Log ("collision");
			this.transform.parent = other.gameObject.GetComponent<DiscSlice>().sliceMesh.transform;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
