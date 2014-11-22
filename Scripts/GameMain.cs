using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class GameMain : Photon.MonoBehaviour {

	public GUISkin skin;

	public List<PlayerStats> players;
	public GameObject discs;
	public GameObject playerHomeMarker;
	public GameObject playerSpeech;
	
	public GameObject zoomFadeText;
	public GameObject winStar;
	public GameObject diamondRain;

	public GameObject gameNotStartedText;

	public float gameDuration = 183f;

	float gamesToWin;
	float startTime;
	bool isStarted = false;
	bool isInited = false;
	bool isStopped = false;
	bool matchEnded = false;
	string state = "just_loaded";

	void Start() {
		players = new List<PlayerStats> ();

		int w = Screen.width;
		int h = Screen.height;
		
		GameObject objAmmo = GameObject.Find ("AmmoThingyGUITexture");
		GUITexture guiText = objAmmo.GetComponent<GUITexture>();
		
		Rect rec = new Rect(guiText.pixelInset);
		
		rec.x = w/2 - 100;
		rec.y = h/2 - 53;
		
		guiText.pixelInset = rec;
	}

	public Vector3 mouseVec;

	Vector3 ProjectMousePosition(Vector3 mousePos) {
		Ray ray = Camera.main.ScreenPointToRay (mousePos);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8));
		return hit.point;
	}

	/**
	 * displays time left in game
	 **/
	void OnGUI() {
		GUI.skin = skin;

		int w = Screen.width;
		int h = Screen.height;

		if (isStarted) {
			float timeLeft = gameDuration-(float)PhotonNetwork.time+startTime;
			if (isStopped) {
				timeLeft = 0f;
			}
			timeLeft = Mathf.Ceil(timeLeft);
			int mins = Mathf.FloorToInt(timeLeft / 60f);
			int secs = Mathf.FloorToInt(timeLeft % 60f);
			string sexString = secs < 10 ? "0" + secs : secs+"";
			GUI.Label (new Rect((float)w/2f-100f, 0f, 200f, 38f), mins+":"+sexString);
		}
	}

	[RPC]
	void SetStateRPC(string state) {
		this.state = state;
	}

	[RPC]
	void GameStartedRPC(float gameTime, float gemsPerPlayer, PhotonMessageInfo info) {
		isStarted = true;
		isInited = false;
		isStopped = false;
		matchEnded = false;
		isLevelSelectMode = false;
		state = "in_game";
		foreach (PlayerStats player in players) {
			player.score = 0;
		}
		// what?! why does scriptsmain menu have to be available to
		// set gameDuration from the variable passed to here???!!
		GameObject scriptsMainMenu = (GameObject) GameObject.Find ("ScriptsMainMenu");
		if (scriptsMainMenu != null) {
			MainMenu menu = scriptsMainMenu.GetComponent<MainMenu>();
			this.gameDuration = gameTime;
			menu.gameData.numFlagItems = gemsPerPlayer;
		}
		if (gameNotStartedText != null) {
			GameObject.Destroy(gameNotStartedText);
		}
		startTime = (float)info.timestamp;
		UpdateScoreBoard();
	}

	[RPC]
	void UpdateScoreBoardRPC() {
		UpdateScoreBoard();
	}

	void UpdateScoreBoard() {
		string s = "";
		for (int i = 1; i <= 4; i++) {
			GameObject gameObj = GameObject.Find ("Score" + i);
			gameObj.GetComponent<TextMesh> ().text = "";
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			PhotonPlayer pp = PhotonNetwork.playerList[i];
			PlayerStats pl = players.Find (item => item.playerID == pp.ID);
			s = pl.name + ":\t" + pl.score;
			GameObject gameObj = GameObject.Find ("Score"+(pp.ID));
			gameObj.GetComponent<TextMesh> ().text = s;
		}
	}

	[RPC]
	void EnablePlayerControls(bool enable) {
		GameObject player = GameObject.Find ("Player"+PhotonNetwork.player.ID);
		if (player != null) {
			PlayerController controller = player.GetComponent<PlayerController>();
			controller.enabled = enable;
		}
	}

	[RPC]
	void EnableDefaultGun(bool enable) {
		GameObject player = GameObject.Find ("Player"+PhotonNetwork.player.ID);
		if (player != null) {
			PlayerController controller = player.GetComponent<PlayerController>();
			controller.defaultGunEnabled = enable;
		}
	}

	[RPC]
	public void PlayerScoredRPC(int playerID, int points) {
		PlayerStats playerStats = players.Find (item => item.playerID == playerID);
		if (playerStats != null) {
			playerStats.score += points;
		}
		GameObject player = GameObject.Find ("Player"+playerID);

		if (player != null) {
			PlayerLogic playerLogic = player.GetComponent<PlayerLogic>();

			if (playerLogic != null) {
				List<FlagItem> items = player.GetComponent<PlayerLogic>().flagItems;

				foreach (FlagItem item in items) {
					item.Reset ();
				}
			} 
			playerLogic.flagItems = new List<FlagItem>();
		}
		if (playerStats != null) {
			GameObject text = (GameObject) Instantiate(zoomFadeText, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
			text.GetComponent<GUIText>().text = playerStats.name + " scores! +" + points;
			text.GetComponent<FlashyTextGUITexture>().fadeOutTime = 2f;
			text.GetComponent<FlashyTextGUITexture>().lingerTime = 0.2f;
		}
		UpdateScoreBoard ();
	}
	
	public void PlayerScored(int playerID, int points) {
		//if (PhotonNetwork.isMasterClient) {
			photonView.RPC ("PlayerScoredRPC", PhotonTargets.AllViaServer, playerID, points);
		//}
	}

	public void CreatePlayers() {
		float delta = UnityEngine.Random.Range (80f, (360f/(float)PhotonNetwork.playerList.Length)+10f);
		float offset = UnityEngine.Random.Range (0f, 360f);
		bool alreadyCreated = false;
		if (players.Count > 0)  {
			alreadyCreated = true;
		}

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			float curRot = i * 180f;
			if (i > 1) {
				curRot -= 90f;
			}
			
			PhotonPlayer player = PhotonNetwork.playerList[i];

			float rot = offset + delta * (float)i;
			while (rot > 360f) {
				rot -= 360f;
			}
			
			Color clr = HSBColor.FromAhsb(255, rot, UnityEngine.Random.Range (0.8f, 1f), UnityEngine.Random.Range (0.4f, 0.7f));
			Vector3 c = new Vector3(UnityEngine.Random.Range (0f, 1f),UnityEngine.Random.Range (0f, 1f),UnityEngine.Random.Range (0f, 1f));
			c = new Vector3(clr.r/255f, clr.g/255f, clr.b/255f);
			float x = (35f - 4f) * Mathf.Sqrt(2) * Mathf.Sin(Mathf.PI*(curRot+45f)/180f);
			float z = (35f - 4f) * Mathf.Sqrt(2) * Mathf.Cos(Mathf.PI*(curRot+45f)/180f);
			if (!alreadyCreated) {
				this.GetComponent<PhotonView>().RPC("CreatePlayer", PhotonTargets.All, 35f, curRot, c, player.name, new Vector2(x, z), player.ID, false);
			} else {
				PlayerStats pStats = players.Find (item => item.playerID == player.ID);
				c = new Vector3(pStats.color.r, pStats.color.g, pStats.color.b);
				this.GetComponent<PhotonView>().RPC("CreatePlayer", PhotonTargets.All, 35f, pStats.bayAngle, c, pStats.name, pStats.homePos, pStats.playerID, true);
			}
		}
	}

	[RPC]
	public void CreatePlayer(float radius, float rot, Vector3 color, string playerName, Vector2 pos, int playerID, bool onlyPlayer)
	{
		Color c = new Color (color.x, color.y, color.z);
		float x = pos.x;
		float z = pos.y;
		//Debug.Log("CreatePlayer " + rot + " " + c + " " + playerID);
		if (PhotonNetwork.player.ID == playerID) {
			GameObject player = PhotonNetwork.Instantiate ("Lumberjack3", new Vector3 (x, 2f, z), 
			                                               Quaternion.identity, 0, new object[]{color, playerID});
			PlayerController controller = player.GetComponent<PlayerController>();
			controller.enabled = true;
			controller.playerID = playerID;
			controller.color = c;

			player.rigidbody.useGravity = true;
			player.GetComponent<CapsuleCollider>().enabled = true;

			PlayerLogic character = player.GetComponent<PlayerLogic>();
			
			character.initX = x;
			character.initZ = z;

			if (!onlyPlayer) {
				CreateBay(radius, rot, color, playerID);
			}
		}

		if (!onlyPlayer) {
			GameObject obj = (GameObject) Instantiate(playerHomeMarker, new Vector3(x, 0f, z), Quaternion.identity);
			obj.renderer.material.color = new Color(color.x, color.y, color.z);
			obj.GetComponent<PlayerHomeMarker>().playerID = playerID;
			obj.name = "HomeMarker" + playerID;

			players.Add (new PlayerStats(playerID, 0, playerName, c, new Vector2(x, z), rot));
		}
	}

	void CreateBay(float radius, float rot, Vector3 color, int playerID) {
		GameObject bayClone = PhotonNetwork.Instantiate("Bay", new Vector3(0, 0, 0), 
		                                                Quaternion.identity, 0, new object[]{color, rot, radius, playerID});
		
		bayClone.GetComponent<Bay> ().Init (rot, radius, color, playerID);	
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		GameObject pl = GameObject.Find ("HomeMarker" + player.ID);
		GameObject.Destroy(pl);
		PlayerStats stats = players.Find(item => item.playerID == player.ID);
		players.Remove(stats);
		GameObject plObj = (GameObject)GameObject.Find ("Player" + PhotonNetwork.player.ID);

		GameObject[] gems = GameObject.FindGameObjectsWithTag("FlagItem");
		for (int i = 0; i < gems.Length; i++) {
			FlagItem fl = gems[i].GetComponent<FlagItem>();
			if (fl.playerID == player.ID) {
				GameObject.Destroy(gems[i]);
			}
			//TODO: what if player is carrying flag item???
		}
		UpdateScoreBoard();
		// TODO: check if alone :(

		if (PhotonNetwork.playerList.Length == 1) {
			// TODO: display message that you are alone and this is fatal....
			PhotonNetwork.LeaveRoom();
			Application.LoadLevel("Menu");
		}
	}

	public void CreateDiscs() {
		TearItDown ();

		GameObject d = (GameObject) Instantiate(discs, Vector3.zero, Quaternion.identity);
		
		d.name = "DiscsMain";
		GameData gameData = GetGameData();

		d.GetComponent<Discs>().numDiscs = (int)Mathf.Round(gameData.numDiscs);
		d.GetComponent<Discs>().avgHoleSize = gameData.avgHoleSize;
		d.GetComponent<Discs>().avgNumSlices = (int)Mathf.Round(gameData.avgNumSlices);
		d.GetComponent<Discs>().maxDiscSpeed = gameData.maxDiscSpeed;
		gameDuration = gameData.gameTime + 3f;
		gamesToWin = Mathf.RoundToInt(gameData.gamesToWin);

		CreatePlayers();

		photonView.RPC ("LevelSelectModeRPC", PhotonTargets.Others, new object[0]);
		photonView.RPC ("EnableDefaultGun", PhotonTargets.AllViaServer, false);
		photonView.RPC ("SetStateRPC", PhotonTargets.All, "level_select_mode");
		DestroyPodest();
	}

	void DestroyPodest() {
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Podest");
		for (int i = 0; i < objs.Length; i++) {
			GameObject.Destroy(objs[i]);
		}
		objs = GameObject.FindGameObjectsWithTag("RainDiamond");
		for (int i = 0; i < objs.Length; i++) {
			GameObject.Destroy(objs[i]);
		}
	}

	bool isLevelSelectMode = true;

	[RPC]
	void LevelSelectModeRPC() {
		if (gameNotStartedText == null) {
			gameNotStartedText = (GameObject) Instantiate(zoomFadeText, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
			FlashyTextGUITexture text = gameNotStartedText.GetComponent<FlashyTextGUITexture>();
			text.lingerTime = 200f;
			text.timeToLive = 200f;
			text.beginAlpha = 0.5f;
			gameNotStartedText.GetComponent<GUIText>().text = "Game not started yet";
		}
		isLevelSelectMode = true;
		// clean up celebration mess
		DestroyPodest();
	}

	public PowerUpProperty GetPowerUpProperty(string typeName, string propName) {
		return GetGameData().GetPowerUpProperty(typeName, propName);
	}

	public GameData GetGameData() {
		GameObject scriptsMainMenu = (GameObject) GameObject.Find ("ScriptsMainMenu");
		if (scriptsMainMenu != null) {
			MainMenu menu = scriptsMainMenu.GetComponent<MainMenu>();
			return menu.gameData;
		} else {
			return new GameData();
		}
	}

	public void StartGame() {
		GameData gameData = GetGameData();

		gameDuration = gameData.gameTime + 3f;
		gamesToWin = Mathf.RoundToInt(gameData.gamesToWin);

		photonView.RPC ("PlayerScoredRPC", PhotonTargets.AllViaServer, -1, 0);
		photonView.RPC ("GameStartedRPC", PhotonTargets.AllViaServer, gameDuration, gameData.numFlagItems);
		photonView.RPC ("ResetPlayers", PhotonTargets.AllViaServer, new object[0]);
		photonView.RPC ("EnablePlayerControls", PhotonTargets.AllViaServer, false);
		photonView.RPC ("EnableDefaultGun", PhotonTargets.AllViaServer, false);

	}

	public GameObject podestCube;

	void BuildPodest() {
		int cnt = 0;
		players.Sort(
			delegate(PlayerStats p1, PlayerStats p2)
			{
				return p2.matchWins - p1.matchWins; 
			}
		);
		int winnerMatchWins = players.ToArray ()[0].matchWins+1;
		float offsetZ = (float)((players.Count+1) % 2) * 4f;
		foreach (PlayerStats player in players) {
			float zPos = Mathf.Ceil(cnt/2f) * 8f * (Mathf.Pow (-1, cnt))+offsetZ;
			Vector3 pos = new Vector3(12f, 2f, zPos);
			GameObject podest = (GameObject)Instantiate (podestCube, pos, Quaternion.identity);
			podest.GetComponent<Scaler>().startScale = new Vector3(8f,0f,8f);
			podest.GetComponent<Scaler>().destScale = new Vector3(8f,4f*(float)(player.matchWins+1)/(float)winnerMatchWins*4f,8f);
			podest.GetComponent<Scaler>().speed = 12f;
			podest.GetComponent<Mover>().startPos = new Vector3(12f,2f,zPos);
			podest.GetComponent<Mover>().destination = new Vector3(12f,2f*(float)(player.matchWins+1)/(float)winnerMatchWins*4f,zPos);
			podest.GetComponent<Mover>().speed = 6f;
			++cnt;
			if (player.playerID == PhotonNetwork.player.ID) {
				GameObject plObj = GameObject.Find ("Player"+player.playerID);
				plObj.transform.position = new Vector3(12f, 4f, zPos);
				plObj.transform.rotation = new Quaternion(0f, Mathf.PI/2, 0f, 1f);
				plObj.transform.parent = null;
			}
		}
		GameObject rain = (GameObject) Instantiate (diamondRain, Vector3.zero, Quaternion.identity);
		rain.GetComponent<DiamondRain>().duration = 20f;
		rain.GetComponent<DiamondRain>().diamondsPerSecond = 20f;
		rain.GetComponent<DiamondRain>().color = players.ToArray()[0].color;
	}

	[RPC]
	void StopGameRPC(int winnerID, int matchWins, bool winsGame) {
		if (!winsGame) {
			state = "between_rounds";
		} else {
			state = "win_ceremony";
		}
		Debug.Log ("StopGameRPC: " + winnerID + " " + matchWins + " " + winsGame);
		if (!isStopped) {
			GameObject text = (GameObject) Instantiate(zoomFadeText, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
			text.GetComponent<FlashyTextGUITexture>().fadeOutTime = 1f;
			text.GetComponent<FlashyTextGUITexture>().lingerTime = 5f;
			text.GetComponent<FlashyTextGUITexture>().growthFactor = 3.5f;
			text.GetComponent<FlashyTextGUITexture>().beginAlpha = 1f;

			if (winnerID != -1) {
				PlayerStats winner = players.Find( item => item.playerID == winnerID );
				winner.matchWins = matchWins;
				Debug.Log ("winnerID: " + winnerID + " winner.playerID: " + winner.playerID);
				GameObject obj = GameObject.Find ("Score" + winnerID);
				//obj.transform.localScale = new Vector3(30f, 30f, 30f);
				GameObject cam = GameObject.Find ("Main Camera");
				Vector3 c = cam.transform.position;
				Vector3 scorePos = obj.transform.position;
				Vector3 starSlot = new Vector3(scorePos.x , scorePos.y + 2.5f, scorePos.z+ 2.5f * (float)(matchWins-1)); 

				GameObject star = (GameObject) Instantiate (winStar, c, Quaternion.identity);

				star.GetComponent<Mover>().speed = 88f;
				star.GetComponent<Mover>().startPos = c;
				star.GetComponent<Mover>().destination = starSlot;

				text.GetComponent<GUIText>().text = winner.name + " wins!";
			} else {
				text.GetComponent<GUIText>().text = "Its a tie...";
			}
			if (winsGame) {
				BuildPodest();
				foreach (PlayerStats player in players) {
					player.matchWins = 0;
				}
				GameObject[] objs = GameObject.FindGameObjectsWithTag("WinStar");
				for (int i = 0; i < objs.Length; i++) {
					GameObject.Destroy(objs[i]);
				}
			}
			isStopped = true;
			matchEnded = winsGame;
		}
	}

	void StopGame() {
		PlayerStats[] playerArr = players.ToArray();
		players.Sort(
			delegate(PlayerStats p1, PlayerStats p2)
			{
				return p2.score - p1.score;
			}
		);
		photonView.RPC ("EnablePlayerControls", PhotonTargets.All, false);
		photonView.RPC ("EnableDefaultGun", PhotonTargets.AllViaServer, false);
		bool isTie = false;

		PlayerStats[] stats = players.ToArray ();
		if (stats.Length > 1) {
			if (stats[0].score == stats[1].score) {
				isTie = true;
			}
		}

		newLevelTime = Time.time + 7f;

		PlayerStats winner = players.ToArray ()[0];
		if (!isTie) {
			++winner.matchWins;
			bool gameEnd = winner.matchWins == gamesToWin;
			Debug.Log ("gamesToWin: " + gamesToWin);
			if (gameEnd) {
				newLevelTime = Time.time + 22f;
			}
			photonView.RPC ("StopGameRPC", PhotonTargets.All, winner.playerID, winner.matchWins, gameEnd);
		} else {
			photonView.RPC ("StopGameRPC", PhotonTargets.All, -1, -1, false);
		}
		isStopped = true;
	}

	float newLevelTime = -1f;

	[RPC]
	void ResetPlayers() {/*
		PhotonPlayer player = PhotonNetwork.player;
		GameObject pl = GameObject.Find ("Player" + player.ID);
		if (pl != null) {
			// TODO: reset--- 
			pl.transform.parent = null;
			pl.GetComponent<PlayerLogic>().Die ();
		}*/
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			PhotonPlayer play = PhotonNetwork.playerList[i];
			GameObject pl2 = GameObject.Find ("Player" + play.ID);
			if (pl2 != null) {
				pl2.transform.parent = null;
				pl2.GetComponent<PlayerLogic>().Die ();
				//pl.transform.position = pl.rigidbody.transform.position = new Vector3(pl.GetComponent<PlayerLogic>().initX, 1f, pl.GetComponent<PlayerLogic>().initZ);
			}
		}
	}
	/*
	public void CreateDiscs() {

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			PhotonPlayer player = PhotonNetwork.playerList[i];
			GameObject pl = GameObject.Find ("Player" + player.ID);
			if (pl != null) {
				pl.GetComponent<PlayerLogic>().Die ();
				pl.transform.position = pl.rigidbody.transform.position = new Vector3(pl.GetComponent<PlayerLogic>().initX, 1f, pl.GetComponent<PlayerLogic>().initZ);
			}
		}
*/
	[RPC]
	void TearItDownRPC() {
		GameObject player = GameObject.Find ("Player" + PhotonNetwork.player.ID);
		PhotonNetwork.Destroy(player);
	}

	void TearItDown() {
		photonView.RPC ("TearItDownRPC", PhotonTargets.All, new object[0]);
		GameObject discs = (GameObject)GameObject.Find ("DiscsMain");
		if (discs != null) {
			Discs discsClone = discs.GetComponent<Discs>();
			int numDiscs = discsClone.discs.Length;
			for (int i = 0; i < numDiscs; i++) {
				Disc d = discsClone.discs[i];
				int numSlices = d.slices.Length;
				for (int t = 0; t < numSlices; t++) {
					DiscSlice slice = d.slices[t];
					PhotonNetwork.Destroy(slice.gameObject);
					GameObject.Destroy(slice);
				}
				GameObject.Destroy(d.gameObject);
				GameObject.Destroy(d);
			}
			GameObject.Destroy(discsClone.gameObject);
			GameObject.Destroy(discsClone);
		} else {
			GameObject[] slices = GameObject.FindGameObjectsWithTag("DiscSlice");
			for (int i = 0; i < slices.Length; i++) {
				PhotonNetwork.Destroy(slices[i].gameObject);
				GameObject.Destroy(slices[i]);
			}
		}
	}

	public void CreatePowerUpOnRandomSlice(Spawnable powerUp) {
		GameObject scriptsComp = (GameObject)GameObject.Find ("Scripts");
		if (discs != null) {
			NetworkItemSpawner spawner = scriptsComp.GetComponent<NetworkItemSpawner>();
			if (spawner != null) {
				spawner.CreateItemOnRandomSlice(powerUp);
			}
		}
	}

	void SpawnPowerUp(PowerUpInfo info) {
		Type t = Type.GetType(info.typeName);
		Spawnable powerUp = (Spawnable)Activator.CreateInstance(t);
		CreatePowerUpOnRandomSlice(powerUp);
	}

	void RoundEnds() {
		newLevelTime = -1f;
		if (PhotonNetwork.isMasterClient) {
			// display some podest with players in order on it..
			CreateDiscs();
		}
		
		GameObject scriptsMainMenu = (GameObject) GameObject.Find ("ScriptsMainMenu");
		if (scriptsMainMenu != null) {
			MainMenu menu = scriptsMainMenu.GetComponent<MainMenu>();
			if (!matchEnded) {
				menu.state = EMenuState.RoundSettings;
			} else {
				menu.state = EMenuState.MatchSettings;
			}
		}
		photonView.RPC ("EnablePlayerControls", PhotonTargets.All, true);
		photonView.RPC ("EnableDefaultGun", PhotonTargets.AllViaServer, false);
	}

	void OnMasterClientSwitched() // definitely seen when the host drops out, not sure if it's when becoming master or just when switching
	{
		if ( PhotonNetwork.isMasterClient ) 
		{
			if (isLevelSelectMode) {
				//players = new List<PlayerStats>();
				TearItDown ();
				RoundEnds ();
			}
		}
	}

	void CountDown() {
		float timePassed = (float)PhotonNetwork.time - startTime;
		int countDownNew = Mathf.CeilToInt(3f - timePassed);
		if (countDownNew != countDown) {
			countDown = countDownNew;
			if (countDownText != null) {
				GameObject.Destroy(countDownText);
			}
			countDownText = (GameObject) Instantiate(zoomFadeText, new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
			countDownText.GetComponent<FlashyTextGUITexture>().fadeOutTime = 1f;
			countDownText.GetComponent<FlashyTextGUITexture>().lingerTime = 0f;
			countDownText.GetComponent<FlashyTextGUITexture>().growthFactor = 2.5f;
			if (countDown != 0) {
				countDownText.GetComponent<GUIText>().text = "" + countDown;
			} else {
				countDownText.GetComponent<GUIText>().text = "GO!";
			}
		}
	}

	void InitGame() {
		photonView.RPC ("EnablePlayerControls", PhotonTargets.AllViaServer, true);
		photonView.RPC ("EnableDefaultGun", PhotonTargets.AllViaServer, true);
		isInited = true;
		GameData gameData = GetGameData();
		
		int numFlagItems = (int)Mathf.Round (gameData.numFlagItems);
		
		// TODO: depend on number of players...

		for (int i = 0; i < gameData.powerUps.Length; i++) {
			PowerUpInfo info = gameData.powerUps[i];
			info.IncreaseNextSpawnTime();
		}

		foreach (PlayerStats pl in players) {
			FlagItem flag = new FlagItem();
			flag.playerID = pl.playerID;
			flag.color = new Vector3( pl.color.r,pl.color.g,pl.color.b);
			for (int i= 0; i < numFlagItems; i++) {
				CreatePowerUpOnRandomSlice(flag);
			}
		}
		photonView.RPC ("UpdateScoreBoardRPC", PhotonTargets.All, new object[0]);
	}

	GameObject countDownText;
	int countDown = -1;

	// Update is called once per frame
	void Update () {
		// we go to 3.1f so in the fourth second we can display the GO!-text
		if (isStarted && (float)PhotonNetwork.time - startTime < 3.1f) {
			CountDown ();
		}

		mouseVec = ProjectMousePosition(Input.mousePosition);

		if (((float)PhotonNetwork.time - startTime > 3f) && (PhotonNetwork.isMasterClient) && isStarted) {
			if (!isInited) {
				InitGame ();
				isInited = true;
			}
		}
		if (isInited && !isStopped && isStarted && PhotonNetwork.isMasterClient) {
			GameData gameData = GetGameData();
			for (int i = 0; i < gameData.powerUps.Length; i++) {
				PowerUpInfo info = gameData.powerUps[i];
				if (Time.time > info.nextSpawnTime && !info.isDisabled) {
					SpawnPowerUp(info);
					info.IncreaseNextSpawnTime();
				}
			}
		}
		if (Time.time > newLevelTime && newLevelTime != -1f) {
			RoundEnds();
		}
		if (((float)PhotonNetwork.time - startTime > gameDuration) && (PhotonNetwork.isMasterClient) && !isStopped && isStarted) {
			StopGame();
		}
	}
}
