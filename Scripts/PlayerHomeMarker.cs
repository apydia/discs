using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHomeMarker : MonoBehaviour {

	public int playerID;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			Debug.Log("playerID: " + col.gameObject.GetComponent<PlayerLogic>().playerID + " " + playerID);
			if (col.gameObject.GetComponent<PlayerLogic>().playerID == playerID) {
				Debug.Log ("player (bay): " + playerID);
				//TODO: how to avoid this accessing global variables??
				GameObject scriptsMainMenu = (GameObject) GameObject.Find ("ScriptsMainMenu");
				int numFlagItems = 3;
				if (scriptsMainMenu != null) {
					MainMenu menu = scriptsMainMenu.GetComponent<MainMenu>();
					numFlagItems = (int)Mathf.Round (menu.gameData.numFlagItems);
				}
				List<FlagItem> items = col.gameObject.GetComponent<PlayerLogic>().flagItems; 
				int score = 0;
				for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
					PhotonPlayer p = PhotonNetwork.playerList[i];
					int count = 0;
					foreach (FlagItem item in items) {
						if (item.GetOwnerId() == p.ID) {
							count++;
						}
					}
					if (count == numFlagItems && playerID != p.ID) {
						score++;
					}
				}
				if (score > 0) {
					Debug.Log ("player scored (bay): " + playerID);
					GameObject.Find("Scripts").GetComponent<GameMain>().PlayerScored(playerID, score*score);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
