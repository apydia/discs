using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {

	public Vector3 destScale;
	public Vector3 startScale;
	
	public float speed;
	
	Vector3 scaleDelta;
	float spawnTime;
	float eta;
	
	// Use this for initialization
	void Start () {
		if (startScale == null) {
			startScale = transform.localScale;
		}
		scaleDelta = destScale - startScale;
		scaleDelta.Normalize();
		
		float dist = Vector3.Distance(destScale, startScale);
		
		spawnTime = Time.time;
		eta = spawnTime + dist / speed;
	}
	
	// Update is called once per frame
	void Update () {
		float timePassed = Time.time - spawnTime;
		if (Time.time < eta) {
			transform.localScale = startScale + scaleDelta * speed * timePassed;
		}
	}
}
