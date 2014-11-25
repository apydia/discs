using UnityEngine;
using System.Collections;

public class ProceduralMesh : MonoBehaviour {
	MeshFilter filter;

	public GameObject objectWithSound;

	// Use this for initialization
	void Start () {

		filter = gameObject.AddComponent< MeshFilter >();
		buffer = new float[1024];
	}

	float [] buffer;

	void CreateMesh(float[] vols) {
		// You can change that line to provide another MeshFilter

		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		float length = 70f;
		float width = 70f;
		int resX = 7; // 2 minimum
		int resZ = 7;

		int cnt = 0;

		#region Vertices		
		Vector3[] vertices = new Vector3[ resX * resZ ];
		for(int z = 0; z < resZ; z++)
		{
			// [ -length / 2, length / 2 ]
			float zPos = ((float)z / (resZ - 1) - .5f) * length;
			for(int x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices[ x + z * resX ] = new Vector3( xPos, vols[cnt++] * 50f * ((cnt+1f)/4f), zPos );
			}
		}
		#endregion
		
		#region Normales
		Vector3[] normales = new Vector3[ vertices.Length ];
		for( int n = 0; n < normales.Length; n++ )
			normales[n] = Vector3.up;
		#endregion
		
		#region UVs		
		Vector2[] uvs = new Vector2[ vertices.Length ];
		for(int v = 0; v < resZ; v++)
		{
			for(int u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
			}
		}
		#endregion
		
		#region Triangles
		int nbFaces = (resX - 1) * (resZ - 1);
		int[] triangles = new int[ nbFaces * 6 ];
		int t = 0;
		for(int face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);
			
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
			
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	bool first = true;

	// Update is called once per frame
	void Update () {
		float [] vols = objectWithSound.GetComponent<AudioSource>().GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		for (int i = 0; i < 1024; i++) {
			buffer[i] = Mathf.Max (buffer[i]*(1f-(0.4f*Time.deltaTime)), vols[i]);
		}
		CreateMesh(buffer);
		transform.Rotate (new Vector3(0f, 5f*Time.deltaTime, 0f));
	}
}
