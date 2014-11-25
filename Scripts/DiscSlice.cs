

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscSlice : Photon.MonoBehaviour {

	public Texture texture;
	public PhysicMaterial physMat;

	public float innerRadius = -1f;
	public float outerRadius = -1f;

	public float startAngle = 1.5f;
	public float size = Mathf.PI/2f;

	public int numSlices = 3;

	public float speed = 1f; // radians per second

	public float curRot = 0f;

	public MeshFilter sliceMesh = null;

	public Vector3 middle;

	public Disc disc = null;

	public double lastUpdate = 0;

	public bool isNetworked = true;

	void Start() {

		lastUpdate = PhotonNetwork.time;
		PhotonNetwork.sendRate = 40;
		PhotonNetwork.sendRateOnSerialize = 20;

		if (sliceMesh == null)
		{
			Debug.LogError("DiscSlice requires its target sliceMesh to be assigned.");
		}

		lastTime = Time.time;
	}

	public void mark() {
		//renderer.material.color = Color.green;
		GameObject lighty = GameObject.Find ("Cylinder");

		Matrix4x4 thisMatrix = sliceMesh.transform.localToWorldMatrix;
		Vector3 middleTransformed = thisMatrix.MultiplyPoint3x4 (middle);

		lighty.transform.position = middleTransformed;
		lighty.transform.parent = sliceMesh.transform;
	}

	public void Init(float ir, float or, float sa, float size, float speed, int discId, int sliceId) {
		photonView.RPC ("InitSlice", PhotonTargets.All, photonView.viewID, ir, or, sa, size, speed, discId, sliceId);
	}

	[RPC]
	public void InitSlice(int id, float ir, float or, float sa, float size, float speed, int discId, int sliceId)
	{
		if (photonView.viewID == id || !isNetworked) {
			innerRadius = ir;
			outerRadius = or;
			startAngle = sa;
			this.size = size;
			this.speed = speed;

			this.gameObject.name = "Disc"+discId+"Slice"+sliceId;

			GenerateMesh ();

			renderer.material.mainTexture = texture;
			renderer.material.color = Color.white;
			renderer.material.mainTextureScale = new Vector2(0.03f, 0.03f);

		}
	}
	
	void GenerateMesh ()
	{
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		Vector3[] unfolded_verts = new Vector3[(numSlices+1)*4];


		for (int i = 0; i < numSlices + 1; i++) {
			var curAngle = startAngle + size * ((float) i / (float) (numSlices)); 
			Vector3 current_point = new Vector3();
			current_point.x = innerRadius * Mathf.Cos (curAngle);
			current_point.y = 0.0f; 
			current_point.z = innerRadius * Mathf.Sin (curAngle);

			unfolded_verts[i*4] = current_point;
			uvs.Add (new Vector2(current_point.x, current_point.z));

			Vector3 current_point2 = new Vector3();
			current_point2.x = outerRadius * Mathf.Cos (curAngle);
			current_point2.y = 0.0f; 
			current_point2.z = outerRadius * Mathf.Sin (curAngle);

			unfolded_verts[i*4 + 1] = current_point2;
			uvs.Add (new Vector2(current_point2.x, current_point2.z));

			if (i == numSlices / 2) {
				middle = current_point + (current_point2 - current_point) / 2;
				middle.y = 5f;
			}

			current_point = new Vector3();
			current_point.x = innerRadius * Mathf.Cos (curAngle);
			current_point.y = -5f; 
			current_point.z = innerRadius * Mathf.Sin (curAngle);
			
			unfolded_verts[i*4 + 2] = current_point;
			uvs.Add (new Vector2(current_point.x - 2.5f, current_point.z - 2.5f));
			
			current_point2 = new Vector3();
			current_point2.x = outerRadius * Mathf.Cos (curAngle);
			current_point2.y = -5f; 
			current_point2.z = outerRadius * Mathf.Sin (curAngle);
			
			unfolded_verts[i*4 + 3] = current_point2;
			uvs.Add (new Vector2(current_point2.x + 2.5f, current_point2.z+2.5f));
		}

		tris.Add (1);
		tris.Add (3);
		tris.Add (0);
		
		tris.Add (3);
		tris.Add (2);
		tris.Add (0);

		int max = (numSlices)*4;

		tris.Add (max + 3);
		tris.Add (max + 1);
		tris.Add (max);
		
		tris.Add (max + 2);
		tris.Add (max + 3);
		tris.Add (max);

		for (int i = 0; i < numSlices; i++) {
			// upside
			tris.Add (i*4);
			tris.Add (i*4+5);
			tris.Add (i*4+1);
			
			tris.Add (i*4);
			tris.Add (i*4+4);
			tris.Add (i*4+5);

			// inner side
			tris.Add (i*4);
			tris.Add (i*4+2);
			tris.Add (i*4+4);

			tris.Add (i*4+2);
			tris.Add (i*4+6);
			tris.Add (i*4+4);

			//outer side
			tris.Add (i*4+1);
			tris.Add (i*4+5);
			tris.Add (i*4+3);

			tris.Add (i*4+3);
			tris.Add (i*4+5);
			tris.Add (i*4+7);

			//downside
			tris.Add (i*4+6);
			tris.Add (i*4+2);
			tris.Add (i*4+3);
			
			tris.Add (i*4+6);
			tris.Add (i*4+3);
			tris.Add (i*4+7);
		}
		
		// Generate the mesh object.
		Mesh ret = new Mesh();
		ret.vertices = unfolded_verts;
		ret.triangles = tris.ToArray();
		ret.uv = uvs.ToArray();
		
		// Assign the mesh object and update it.
		ret.RecalculateBounds();
		ret.RecalculateNormals();
		sliceMesh.mesh = ret;
		//sliceMesh.light.transform.Translate(new Vector3 (10f,10f,10f), Space.World);

		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		//meshc.isTrigger = true;
		meshc.sharedMesh = ret;
		if (physMat != null) {
			meshc.material = physMat;
		}
		//meshc.convex = true;
		//meshc.isTrigger = true;
	}

	public Vector3 GetRandomPos() {
		float randGamma = Random.Range (startAngle, startAngle + size);
		float randEpsilon = Random.Range (innerRadius, outerRadius);

		Vector3 vec =  new Vector3(Mathf.Cos(randGamma) * randEpsilon, 1f, Mathf.Sin (randGamma) * randEpsilon);

		Matrix4x4 thisMatrix = sliceMesh.transform.localToWorldMatrix;
		Vector3 vecTransformed = thisMatrix.MultiplyPoint3x4 (vec);

		return vecTransformed;
	}

	float lastTime;

	bool doPrediction = true;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			doPrediction = !doPrediction;
		}

		float delta = speed * Time.deltaTime;

		sliceMesh.transform.Rotate (new Vector3 (0f, delta, 0f));

		curRot += delta;

		float pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
		float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastUpdate);
		
		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

		if (!photonView.isMine && isNetworked) {
			float predictedMasterRotation = lastMasterRot + speed * totalTimePassed;

			if (Mathf.Abs(predictedMasterRotation - curRot) > 1f) { 
				sliceMesh.transform.Rotate (new Vector3 (0f, predictedMasterRotation - curRot, 0f));
				curRot = predictedMasterRotation;
			}
		} 
	}

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Player") {
			//mark();
			col.gameObject.GetComponent<PlayerLogic>().discSpeed = this.speed;
			col.transform.parent = sliceMesh.transform;
			col.transform.parent.position = new Vector3(0f,0f,0f);
		}
	}

	void OnCollisionStay(Collision col) {
		if(col.gameObject.tag == "Player") {
			//mark();
			col.gameObject.GetComponent<PlayerLogic>().discSpeed = this.speed;
			col.transform.parent = sliceMesh.transform;
			col.transform.parent.position = new Vector3(0f,0f,0f);
		}
	}
	
	void OnCollisionExit(Collision col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerLogic>().discSpeed = 0f;
			col.transform.parent = null;
		}
	}

	Quaternion correctRot;

	public float lastMasterRot = 0f;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (Time.time - lastTime > 1f && isNetworked) {
			lastTime = Time.time;
			if (stream.isWriting) {
				// We own this player: send the others our data
				stream.SendNext (transform.rotation);
				stream.SendNext (curRot);
			} else {
				// Network player, receive data
				correctRot = (Quaternion)stream.ReceiveNext ();
				lastMasterRot = (float)stream.ReceiveNext ();
				lastUpdate = info.timestamp;
			}
		}
	}
}
