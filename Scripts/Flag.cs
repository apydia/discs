using UnityEngine;
using System.Collections;

public class Flag : Photon.MonoBehaviour {

	public Color color;
	public int playerID;
	public float initX = 0f;
	public float initZ = 0f;

	public int capturedByPlayer = -1;

	// Use this for initialization
	void Start () {
		renderer.material.color = color;
	}

	public int GetOwnerId() {
		return playerID;
	}

	public void Init(Vector3 color, int playerID, float initX, float initZ) {
		photonView.RPC ("InitFlag", PhotonTargets.All, photonView.viewID, color, playerID, initX, initZ);
	}
	
	[RPC]
	void InitFlag(int id, Vector3 color, int playerID,  float initX, float initZ) {
		if (photonView.viewID == id) { 
			this.color = new Color (color.x, color.y, color.z);
			this.playerID = playerID;
			this.initX = initX;
			this.initZ = initZ;

			renderer.material.color = this.color;
		}
	}

	[RPC]
	void ResetFlag(int playerID) {
		Debug.Log ("esetFlag: " + playerID + " flag.playerID, " + this.playerID);
		if (this.playerID == playerID) {
			transform.parent = null;

			capturedByPlayer = -1;
			transform.position = new Vector3(initX, 0.5f, initZ);
		}
	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y < -33f) {
			this.ResetFlagLocal();
		}
	}

	public void ResetFlagLocal() {
		this.photonView.RPC ("ResetFlag", PhotonTargets.All, this.playerID);   
		this.ResetFlag(this.playerID);
	}

	public void Reset() {
		ResetFlagLocal();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			if (playerID != other.gameObject.GetComponent<PlayerLogic>().playerID) {
				transform.parent = other.transform;
				other.gameObject.GetComponent<PlayerLogic>().hasFlagOfPlayer = playerID;
				capturedByPlayer = other.gameObject.GetComponent<PlayerLogic>().playerID;
				//other.gameObject.GetComponent<PlayerLogic>().flagItems.Add(this);
			}
		}
		if(other.gameObject.tag == "Bay") {
			/*if (other.gameObject.GetComponent<Bay>().playerID == capturedByPlayer) {
				Debug.Log ("player scored: " + capturedByPlayer);
				GameObject.Find("Scripts").GetComponent<GameMain>().PlayerScored(playerID, 1);
				this.ResetFlagLocal();
			}*/
		}
	}
}
