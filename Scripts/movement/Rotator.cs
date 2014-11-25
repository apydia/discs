using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public float xRotPerSec;
	public float yRotPerSec;
	public float zRotPerSec;

	public bool rotRand = false;

	// Use this for initialization
	void Start () {
		if (rotRand) {
			xRotPerSec = Random.Range(-2f, 2f);
			yRotPerSec = Random.Range(-2f, 2f);
			zRotPerSec = Random.Range(-2f, 2f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(xRotPerSec*Time.deltaTime, yRotPerSec*Time.deltaTime, zRotPerSec*Time.deltaTime));
	}
}
