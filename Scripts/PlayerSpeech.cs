using UnityEngine;
using System.Collections;

public class PlayerSpeech : MonoBehaviour {

	// Use this for initialization
	void Start () {
		visibleTime = 3f;
	}

	float talkTime;
	float visibleTime;

	public void Say(string what) {
		gameObject.GetComponent<TextMesh>().text = what;
		talkTime = Time.time;
	}

	// Update is called once per frame
	void Update () {
		Vector3 p = transform.position;
		GameObject cam = GameObject.Find ("Main Camera");
		Vector3 c = cam.transform.position;
		Vector3 look = p - c;
		transform.rotation = Quaternion.LookRotation(look);
		float timePassed = Time.time - talkTime;
		if ( timePassed < visibleTime / 2f) {
			float size = timePassed / (visibleTime / 2f) * 0.5f;
			transform.localScale = size * Vector3.one;
		} else if (timePassed < visibleTime) {
			float size = 0.5f- (timePassed-(visibleTime / 2f)) / (visibleTime / 2f) * 0.5f;
			transform.localScale = size * Vector3.one;
		}

		if (timePassed > visibleTime) {
			gameObject.GetComponent<TextMesh>().text = "";
		}

	}
}
