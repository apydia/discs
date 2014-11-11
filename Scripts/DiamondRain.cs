using UnityEngine;
using System.Collections;

public class DiamondRain : MonoBehaviour {

	public GameObject skyDiamond;
	public float duration;
	public float diamondsPerSecond;
	public Color color;

	float startTime;
	float nextDiamondTime;
	float deltaDiamondTime;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		deltaDiamondTime = 1f/diamondsPerSecond;
		nextDiamondTime = startTime + deltaDiamondTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime < duration) {
			if (Time.time > nextDiamondTime) {
				nextDiamondTime += deltaDiamondTime;
				Vector3 pos = new Vector3(Random.Range (-35f, 35f), Random.Range (20f, 35f),Random.Range (-35f, 35f));
				GameObject obj = (GameObject)Instantiate(skyDiamond, pos, Quaternion.identity);			
				obj.transform.Rotate(new Vector3(Random.Range (0f, 360f),Random.Range (0f, 360f), Random.Range (0f, 360f)));
				obj.renderer.material.color = color;
			}
		}
	}
}
