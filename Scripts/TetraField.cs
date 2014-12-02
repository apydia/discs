using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetraField : MonoBehaviour {

	public GameObject tetra;
	public GameObject objectWithSound;
	
	float[] buffer = new float[1024];
	public float innerRadius, outerRadius;
	public int numTetras;

	public float speed;

	List<GameObject> tetras = new List<GameObject>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < numTetras; i++) {
			float rad = Random.Range (innerRadius, outerRadius);
			float rot = Random.Range (0, 2f*Mathf.PI);
			Vector3 pos = new Vector3(rad*Mathf.Sin(rot), 0, rad*Mathf.Cos(rot));
			GameObject o = (GameObject)Instantiate(tetra, pos, Quaternion.identity);
			o.GetComponent<Tetrahedron>().c = new Color(Random.Range (0,1f),Random.Range (0,1f),Random.Range (0,1f));
			o.GetComponent<Tetrahedron>().reactTo = Mathf.RoundToInt(Random.Range (0f, 32f)); 
			o.transform.parent = transform;
			tetras.Add (o);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float [] vols = objectWithSound.GetComponent<AudioSource>().GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		for (int i = 0; i < 1024; i++) {
			buffer[i] = Mathf.Max (buffer[i]*(1f-(0.4f*Time.deltaTime)), vols[i]);
		}
		foreach(GameObject tet in tetras) {
			int idx = tet.GetComponent<Tetrahedron>().reactTo;
			float s = Mathf.Max (buffer[idx],0.02f);
			tet.transform.localScale = Vector3.one * s * (idx+1)*5;
		}
	}
}
