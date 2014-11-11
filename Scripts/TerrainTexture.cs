using UnityEngine;
using System.Collections;

public class TerrainTexture : MonoBehaviour {

	public Texture texture;

	// Use this for initialization
	void Start () {
		renderer.material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
