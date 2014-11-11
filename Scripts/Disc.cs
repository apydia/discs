using UnityEngine;
using System.Collections;

public class Disc : MonoBehaviour {

	public GameObject slice;

	public int numSlices = 5;
	public float speed = 20f;

	public float innerRadius = 12f;
	public float outerRadius = 15f;

	public Discs discs = null;

	public int index;

	public DiscSlice[] slices = null;

	public bool isNetworked = true;
	public bool isDemo = false;

	public float avgHoleSize = 0.5f;

	float speedX;
	float speedZ;

	float curRotX = 0f;
	float curRotZ = 0f;

	// Use this for initialization
	void Start () {
		createSlices ();

		if (isDemo) {
			speedX = Random.Range (-7f, 7f);
			speedZ = Random.Range (-7f, 7f);
		}
	}

	void createSlices() {

		slices = new DiscSlice[numSlices];

		float delta = Mathf.PI*2 / (float) numSlices;

		for (int i = 0; i < numSlices; i++) {

			float size = delta - Random.Range (Mathf.Max (0f, avgHoleSize - 0.3f), Mathf.Min (avgHoleSize + 0.3f, delta*3f/8f));
			if (numSlices == 1) {
				size = delta;	
			}

			float curAngle = delta * (float) i;

			GameObject sliceClone = null;

			if (isNetworked) {
				sliceClone = PhotonNetwork.InstantiateSceneObject("DiscSlice", Vector3.zero, Quaternion.identity, 0, new object[0]);
			//int id = sliceClone.GetInstanceID();

				sliceClone.GetComponent<DiscSlice>().Init(innerRadius, outerRadius, curAngle, size, speed, index, i);
			} else {
				sliceClone = (GameObject)Instantiate(slice, Vector3.zero, Quaternion.identity);
				//int id = sliceClone.GetInstanceID();
				sliceClone.GetComponent<DiscSlice>().isNetworked = false;
				sliceClone.GetComponent<DiscSlice>().InitSlice(0, innerRadius, outerRadius, curAngle, size, speed, index, i);
			}
			/*
			sliceClone.GetComponent<DiscSlice>().speed = speed;
			sliceClone.GetComponent<DiscSlice>().startAngle = curAngle;
			sliceClone.GetComponent<DiscSlice>().size = size;

			sliceClone.GetComponent<DiscSlice>().innerRadius = innerRadius;
			sliceClone.GetComponent<DiscSlice>().outerRadius = outerRadius;
*/
			sliceClone.GetComponent<DiscSlice>().disc = this;
			sliceClone.transform.parent = this.transform;

			slices[i] = (DiscSlice)sliceClone.GetComponent<DiscSlice>();
		}
	}

	public void markSlice() {

		int rand = Random.Range (0, numSlices-1);
		Debug.Log ("markSlice " + rand + " " + slices.Length);
		slices [rand].mark ();
	}

	// Update is called once per frame
	void Update () {
		float deltaX = speedX * Time.deltaTime;
		curRotX += deltaX;
		float deltaZ = speedZ * Time.deltaTime;
		curRotZ += deltaZ;

		transform.Rotate (new Vector3 (deltaX, 0f, deltaZ));
		

	}
}
