using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	float speed = 0.3f;
	float curRot = 0f;
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Input.mousePosition;
		float delta = Time.deltaTime * speed;
		curRot = mousePos.x / 1000;
		float curRotZ = mousePos.y / 1000;
		transform.position = new Vector3(Mathf.Sin (curRot)*30f+Mathf.Cos (curRotZ)*30f, -Mathf.Sin (curRotZ)*60f, Mathf.Cos (curRot) * 60f);
		transform.rotation = Quaternion.LookRotation (-transform.position);
	}
}
