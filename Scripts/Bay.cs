using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bay : Photon.MonoBehaviour {
	
	public Texture texture;

	public float radius = 35f;
	
	public float rotation = 0f;
	public int playerID;
	public Color color;

	public MeshFilter bayMesh = null;

	int numSlices = 25;

	void Start()
	{
		if (bayMesh == null)
		{
			Debug.LogError("Bay requires its target bayMesh to be assigned.");
		}
	}

	public void Init(float rotation, float radius, Vector3 color, int playerID) {
		photonView.RPC ("InitBay", PhotonTargets.All, photonView.viewID, rotation, radius, color, playerID);
	}

	[RPC]
	void InitBay(int id, float rotation, float radius, Vector3 color, int playerID) {
		if (photonView.viewID == id) { 
			this.rotation = rotation;
			this.radius = radius;
			this.color = new Color (color.x, color.y, color.z);
			this.playerID = playerID;
			GenerateMesh ();

			renderer.material.mainTexture = texture;
			//renderer.material.color = this.color;
			renderer.material.mainTextureScale = new Vector2 (0.1f, 0.1f);
		}
	}

	void GenerateMesh ()
	{
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		Vector3[] unfolded_verts = new Vector3[2*(numSlices)+2];
		
		for (int i = 0; i < numSlices; i++) {
			var curAngle = Mathf.PI/2 * ((float) i / (float) (numSlices + 1)); 
			Vector3 current_point = new Vector3();
			current_point.x = radius * Mathf.Cos (curAngle);
			current_point.y = 0.0f; 
			current_point.z = radius * Mathf.Sin (curAngle);
			
			unfolded_verts[i] = current_point;

			Vector3 current_point2 = new Vector3();
			current_point2.x = radius * Mathf.Cos (curAngle);
			current_point2.y = -5.0f; 
			current_point2.z = radius * Mathf.Sin (curAngle);
			unfolded_verts[i + numSlices] = current_point2;

		}

		unfolded_verts[numSlices*2+1] = new Vector3(radius,-5f,radius);
		unfolded_verts[numSlices*2] = new Vector3(radius,0f,radius);

		for (int i = 0; i < unfolded_verts.Length; i++) {
			uvs.Add (new Vector2(unfolded_verts[i].x+unfolded_verts[i].y, unfolded_verts[i].z+unfolded_verts[i].y));
		}

		for (int i = 0; i < numSlices-1; i++) {
			tris.Add (i);
			tris.Add (i+1);
			tris.Add (numSlices*2);

			tris.Add (i+1);
			tris.Add (i);
			tris.Add (numSlices*2);

			tris.Add (i);
			tris.Add (i+1);
			tris.Add (i+numSlices);

			tris.Add (i+1);
			tris.Add (i);
			tris.Add (i+numSlices);

			tris.Add (i+numSlices+1);
			tris.Add (i+1);
			tris.Add (i+numSlices);
			
			tris.Add (i+1);
			tris.Add (i+numSlices+1);
			tris.Add (i+numSlices);
		}

		tris.Add (numSlices*2+1);
		tris.Add (0);
		tris.Add (numSlices*2);

		tris.Add (0);
		tris.Add (numSlices*2+1);
		tris.Add (numSlices);

		tris.Add (numSlices*2+1);
		tris.Add (numSlices-1);
		tris.Add (numSlices*2-1);

		tris.Add (numSlices*2+1);
		tris.Add (numSlices*2);
		tris.Add (numSlices-1);

		// Generate the mesh object.
		Mesh ret = new Mesh();
		ret.vertices = unfolded_verts;
		ret.triangles = tris.ToArray();
		ret.uv = uvs.ToArray();
		
		// Assign the mesh object and update it.
		ret.RecalculateBounds();
		ret.RecalculateNormals();
		bayMesh.mesh = ret;
		
		bayMesh.transform.Rotate (new Vector3(0f, rotation, 0f));

		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;

		meshc.sharedMesh = ret;

		//meshc.convex = true;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnCollisionEnter(Collision col) {
		
		if (col.gameObject.tag == "Player") {
			col.transform.parent = bayMesh.transform;
		}
	}
	
	bool isInited = false;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!isInited) {
			Vector3 c = (Vector3)photonView.instantiationData [0];
			this.rotation = (float)photonView.instantiationData [1];
			this.radius = (float)photonView.instantiationData [2];
			this.playerID = (int)photonView.instantiationData [3];
			this.color = new Color (c.x, c.y, c.z);
			
			isInited = true;
		}
	}
	
}
