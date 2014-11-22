using UnityEngine;
using System.Collections;

public class FlagItem : Photon.MonoBehaviour, Spawnable {

	public int playerID;
	public Vector3 color;
	public int capturedByPlayer = -1;
	public int id = 0;

	// Use this for initialization
	void Start () {
		Physics.IgnoreLayerCollision(10, 10);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int GetOwnerId() {
		return playerID;
	}

	public int GetId() {
		return id;
	}

	public void Reset() {
		FlagItem flag = new FlagItem();
		flag.playerID = playerID;
		flag.color = color;
		if (PhotonNetwork.isMasterClient) {
			GameMain gameMain = GameObject.Find ("Scripts").GetComponent<GameMain>();
			gameMain.CreatePowerUpOnRandomSlice(flag);
		}
		if (this != null) {
			if (this.gameObject != null) {
				this.gameObject.transform.parent = null;
				GameObject.Destroy(this.gameObject);
			}
		}
	}

	public string GetName() {
		return "FlagItem";
	}

	public object[] GatherInitData() {
		return new object[] {playerID, color};
	}

	public void Init(int id, object[] initData, double spawnTime) {
		this.id = id;
		playerID = (int)initData[0];
		color = (Vector3)initData[1];
		renderer.material.color = new Color(color.x, color.y, color.z);
	}

	[RPC]
	void CapturedByPlayer(int playerID) {
		GameObject player = GameObject.Find ("Player"+playerID);
		transform.parent = player.transform;
		if (capturedByPlayer != playerID) {
			capturedByPlayer = playerID;
			player.GetComponent<PlayerLogic>().flagItems.Add (this);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			if (capturedByPlayer == -1) {
				other.gameObject.GetComponent<PlayerLogic>().CapturedFlagItem(id);
			}
		}
	}
}
