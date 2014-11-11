using UnityEngine;
using System.Collections;

public class FlashyTextGUITexture : MonoBehaviour {

	float createDate = 0f;
	public float timeToLive = 20f;
	public float fadeOutTime = 5f;
	public float lingerTime = 0.5f;
	public float growthFactor = 2.2f;
	public float minFontSize = 15f;
	public float beginAlpha = 1f;
	float aspectRatio = 1f;

	// Use this for initialization
	void Start () {
		createDate = Time.time;
		aspectRatio = (float)Screen.width / (float)Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
		float curTime = Time.time - createDate;
		if (curTime > lingerTime) {
			float xScale = Mathf.Pow (1f+curTime-lingerTime, growthFactor)/256f*15f;
			xScale = Mathf.Max (minFontSize/256f, xScale);
			float yScale = Mathf.Pow (1f+curTime-lingerTime, growthFactor)*aspectRatio/256f*15f;
			yScale = Mathf.Max (minFontSize * aspectRatio/256f, yScale);
			gameObject.transform.localScale = new Vector3(xScale, 
				                                          yScale, 
			                                              1f);
			Color c = gameObject.GetComponent<GUIText>().color;

			float alpha = Mathf.Sqrt (Mathf.Max(0f, 1f-((curTime-lingerTime) / fadeOutTime)));

			alpha *= beginAlpha;

			c.a = alpha;
			gameObject.GetComponent<GUIText>().color = c;
			if (curTime > timeToLive) {
				GameObject.Destroy(this.gameObject);
			}
		} else {
			Color c = gameObject.GetComponent<GUIText>().color;
			c.a = beginAlpha;
			gameObject.GetComponent<GUIText>().color = c;
			gameObject.transform.localScale = new Vector3(15f/256f, 
			                                              aspectRatio/256f*15f, 
			                                              1f);
		}
		//gameObject.GetComponent<GUIText>().fontSize = 15+(int)((Time.time-createDate)*100f);
	}
}
