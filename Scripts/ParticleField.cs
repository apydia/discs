using UnityEngine;
using System.Collections;

public class ParticleField : MonoBehaviour {

	public Rect startRect;
	public Vector3 direction;
	public Vector3 emitterCenter;

	public GameObject tetra;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.Range (0f, 1f) > 0.2f) {
			float xOff = Random.Range (startRect.x, startRect.width);
			float yOff = Random.Range (startRect.y, startRect.height);
			Vector3 pos = emitterCenter + new Vector3(xOff, yOff, 0f);
			GameObject o = (GameObject) Instantiate (tetra, pos, Quaternion.identity);
			o.GetComponent<Tetrahedron>().timeToLive = Random.Range (20f, 40f);
			o.GetComponent<Tetrahedron>().c = new Color(Random.Range (0,1f),Random.Range (0,1f),Random.Range (0,1f));
			o.GetComponent<Mover>().speed = Random.Range (10f, 40f);
			o.GetComponent<Mover>().startPos = pos;
			o.GetComponent<Mover>().destination = pos + direction*1000;

			o.transform.parent = transform;
		}
	}
}
