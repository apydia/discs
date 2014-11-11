using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomMatchMaker : Photon.MonoBehaviour
{
	public bool isQuickConnect = false;

	// Use this for initialization
	void Start()
	{

		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
		if (isQuickConnect) {
			PhotonNetwork.ConnectUsingSettings("0.1");
		}
	}

	public bool gameStarted = false;

	void OnGUI()
	{
		if (!gameStarted && isQuickConnect) {
			GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
		} else {
			GUILayout.Label ("");
		}
	}

	void OnJoinedLobby()
	{
		if (isQuickConnect) {
			PhotonNetwork.JoinRandomRoom();
		}
	}
	
	void OnPhotonRandomJoinFailed()
	{
		if (isQuickConnect) {
			Debug.Log("Can't join random room!");
			PhotonNetwork.CreateRoom(null);
		}
	}
	
	void OnCreatedRoom()
	{
		Debug.Log ("created");

	}

	void OnJoinedRoom()
	{	
		if (isQuickConnect) {
			PhotonNetwork.playerName = names[PhotonNetwork.playerList.Length -1];
		}
	}

	string [] names = {"Claude", "Illian", "Lysatra", "Void"};

	[RPC]
	void GameStarted() {
		this.gameStarted = true;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			if (PhotonNetwork.isMasterClient) {
				gameStarted = true;
				gameObject.GetComponent<GameMain>().CreatePlayers();
				gameObject.GetComponent<GameMain>().CreateDiscs();
				gameObject.GetComponent<GameMain>().StartGame();
			}
		}
		if (!isQuickConnect && !gameStarted && PhotonNetwork.isMasterClient) {
			gameStarted = true;
			//gameObject.GetComponent<GameMain>().InitGame();
			gameObject.GetComponent<GameMain>().CreateDiscs();
			photonView.RPC ("GameStarted", PhotonTargets.Others, new object[0]);
		}
	}
}