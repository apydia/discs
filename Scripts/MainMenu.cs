using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenu : Photon.MonoBehaviour {

	public GUISkin skin;

	public EMenuState state = EMenuState.EnterName;

	// TODO: read/write to NoSQL thing
	public GameData gameData;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		PhotonNetwork.ConnectUsingSettings("0.1");

		LoadGameData();
	}
	
	void OnGUI() {
		GUI.skin = skin;

		int w = Screen.width;
		int h = Screen.height;
		Event e = Event.current;

		if (state == EMenuState.EnterName) {
			EnterName(w, h, e);
		}

		if (state == EMenuState.MainMenu) {
			MainMenuState(w, h, e);
		}

		if (state == EMenuState.CreateRoom) {
			CreateRoom (w, h, e);
		}

		if (state == EMenuState.JoinRoom) {
			JoinRoom (w, h, e);
		}

		if (state == EMenuState.WaitingForPlayers) {
			WaitingForPlayers(w, h, e);
		}

		if (state == EMenuState.MatchSettings) {
			FirstRoundSettings(w, h, e);
		}

		if (state == EMenuState.RoundSettings && !show_chat_window) {
			LevelBuilder(w, h, e);
		}

		if (talk_mode) {
			TalkMode (w, h, e);
		}

		if (show_chat_window) {
			ShowChatWindow (w, h, e);
		}

		if (!gameStarted) {
			GUI.Label (new Rect(0, h-15, 300, 15), PhotonNetwork.connectionStateDetailed.ToString (), "labelsmall");
		}

		GUI.SetNextControlName("");
	}

	void EnterName(int w, int h, Event e) {
		if (e.keyCode == KeyCode.Return && gameData.playerName.Trim() != "") {
			SaveGameData();
			state = EMenuState.MainMenu;
			PhotonNetwork.playerName = gameData.playerName.Trim();
		} else {
			GUI.Label (new Rect(w/2-150, h/2-70, 300, 30), "Enter thy name", "label");
			gameData.playerName = GUI.TextField(new Rect(w/2-150, h/2-30, 300, 38), gameData.playerName, 25, "textfield");
		}
		if (gameData.playerName.Trim() == "") {
			gameData.playerName = "";
		}
	}

	void MainMenuState(int w, int h, Event e) {
		GUI.Label (new Rect(w/2-150, h/2-130, 300, 30), "Main Menu", "label");
		
		if (GUI.Button (new Rect(w/2-150, h/2-70, 300, 30), "Change name", "button")) {
			state = EMenuState.EnterName;
		}
		if (GUI.Button (new Rect(w/2-150, h/2-35, 300, 30), "Create Room", "button")) {
			state = EMenuState.CreateRoom;
		}
		
		if (GUI.Button (new Rect(w/2-150, h/2-0, 300, 30), "Join Room", "button")) {
			state = EMenuState.JoinRoom;
		}
	}

	void CreateRoom(int w, int h, Event e) {
		if (e.keyCode == KeyCode.Return) {
			state = EMenuState.WaitingForPlayers;
			SaveGameData();
			PhotonNetwork.CreateRoom(gameData.roomName);
			
		} else {
			GUI.Label (new Rect(w/2-150, h/2-130, 300, 30), "Create Room", "label");
			
			gameData.roomName = GUI.TextField(new Rect(w/2-150, h/2-90, 300, 38), gameData.roomName, 25, "textfield");
			
			if (GUI.Button (new Rect(w/2-150, h/2+120, 300, 30), "Create Room", "button")) {
				PhotonNetwork.CreateRoom(gameData.roomName);
				state = EMenuState.WaitingForPlayers;
			}
			
			if (GUI.Button (new Rect(w/2-150, h/2+150, 300, 30), "Back", "button")) {
				state = EMenuState.MainMenu;
			}
		}
	}

	void JoinRoom(int w, int h, Event e) {
		GUI.Label (new Rect(w/2-150, h/2-130, 300, 30), "Pick A Room To Join", "label");
		
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		for (int i = 0; i < rooms.Length; i++) {
			string roomName = rooms[i].name;
			if (!rooms[i].open) {
				roomName += " (closed)";
			}
			if (GUI.Button (new Rect(w/2-150, h/2-90+35*i, 300, 30), roomName, "button")) {
				if (rooms[i].open) {
					PhotonNetwork.JoinRoom(rooms[i].name);
					state = EMenuState.WaitingForPlayers;
				}
			}
		}
		if (GUI.Button (new Rect(w/2-150, h/2+120, 300, 30), "Back", "button")) {
			state = EMenuState.MainMenu;
		}
	}

	void WaitingForPlayers(int w, int h, Event e) {
		GUI.Label (new Rect(w/2-150, h/2-130, 300, 30), "Waiting For Players", "label");
		
		PhotonPlayer[] players = PhotonNetwork.playerList;
		for (int i = 0; i < players.Length; i++) {
			GUI.Label (new Rect(w/2-150, h/2-90+35*i, 300, 30), players[i].name, "label");
		}
		
		if (GUI.Button (new Rect(w/2-150, h/2+85, 300, 30), "Back", "button")) {
			PhotonNetwork.LeaveRoom();
			if (PhotonNetwork.isMasterClient) {
				state = EMenuState.CreateRoom;
			} else {
				state = EMenuState.JoinRoom;
			}
		}
		if (PhotonNetwork.isMasterClient) {
			if (GUI.Button (new Rect(w/2-150, h/2+120, 300, 30), "Close Room For Other Players", "button")) {
				PhotonNetwork.room.open = false;
				state = EMenuState.MatchSettings;
				photonView.RPC ("GentlemenStartYourEngines", PhotonTargets.All, new object[0]);
			}
		}
	}

	void FirstRoundSettings(int w, int h, Event e) {
		if (e.keyCode == KeyCode.Return) {
			state = EMenuState.RoundSettings;
			SaveGameData();
			
		} else {
			GUI.Label (new Rect(w/2-150, h/2-130, 300, 30), "Match Settings", "label");
			
			GUI.Label (new Rect(w/2-150, h/2-35, 300, 12), "win " + (int)Mathf.Round (gameData.gamesToWin) + " rounds to win game" , "labelsmall");
			gameData.gamesToWin = GUI.HorizontalSlider(new Rect(w/2-150, h/2-20, 300, 25), gameData.gamesToWin, 1f, 9f, "horizontalslider", "horizontalsliderthumb");
			
			if (GUI.Button (new Rect(w/2-150, h/2+120, 300, 30), "Setup Level", "button")) {
				state = EMenuState.RoundSettings;
			}
			
			if (GUI.Button (new Rect(w/2-150, h/2+150, 300, 30), "Back", "button")) {
				state = EMenuState.MainMenu;
			}
		}
	}

	int selVal = 0;
	Vector2 scrollPosition = Vector2.zero;
	Vector2 scrollPosition2 = Vector2.zero;
	float buildDiscsActivateTime;

	void LevelBuilder(int w, int h, Event e) {
		string[] tts = new string[] {"level","powerups","tweak"};

		if (PhotonNetwork.isMasterClient) {
			float offsetY = 90f;

			selVal = GUI.Toolbar(new Rect(w/2-150, h/2-78-offsetY, 300, 20), selVal, tts);

			GUI.Box (new Rect(w/2-170, h/2-55-offsetY, 340, 275),"");

			// level
			if (selVal == 0) {
				GUI.Label (new Rect(w/2-150, h/2-45-offsetY, 300, 12), "average hole size [" + Mathf.Round(gameData.avgHoleSize * 18000)/100/Mathf.PI  + "]", "labelsmall");
				gameData.avgHoleSize = GUI.HorizontalSlider(new Rect(w/2-150, h/2-30-offsetY, 300, 25), gameData.avgHoleSize, 0f, 1.5f, "horizontalslider", "horizontalsliderthumb");
				
				GUI.Label (new Rect(w/2-150, h/2-15-offsetY, 300, 12), "maximum disc speed [" + Mathf.Round(gameData.maxDiscSpeed * 100)/100 + "]", "labelsmall");
				gameData.maxDiscSpeed = GUI.HorizontalSlider(new Rect(w/2-150, h/2-0-offsetY, 300, 25), gameData.maxDiscSpeed, 10.0f, 80.0f, "horizontalslider", "horizontalsliderthumb");
				
				GUI.Label (new Rect(w/2-150, h/2+15-offsetY, 300, 12), "number of discs [" + (int)Mathf.Round (gameData.numDiscs) + "]", "labelsmall");
				gameData.numDiscs = GUI.HorizontalSlider(new Rect(w/2-150, h/2+30-offsetY, 300, 25), gameData.numDiscs, 3f, 9f, "horizontalslider", "horizontalsliderthumb");
				
				GUI.Label (new Rect(w/2-150, h/2+45-offsetY, 300, 12), "average number of slices [" + (int)Mathf.Round (gameData.avgNumSlices) + "]", "labelsmall");
				gameData.avgNumSlices = GUI.HorizontalSlider(new Rect(w/2-150, h/2+60-offsetY, 300, 25), gameData.avgNumSlices, 3f, 7f, "horizontalslider", "horizontalsliderthumb");
				
				GUI.Label (new Rect(w/2-150, h/2+75-offsetY, 300, 12), "gems per player [" + (int)Mathf.Round (gameData.numFlagItems) + "]", "labelsmall");
				gameData.numFlagItems = GUI.HorizontalSlider(new Rect(w/2-150, h/2+90-offsetY, 300, 25), gameData.numFlagItems, 2f, 7f, "horizontalslider", "horizontalsliderthumb");
				
				GUI.Label (new Rect(w/2-150, h/2+105-offsetY, 300, 12), "game duration [" + (int)Mathf.Round (gameData.gameTime) + " sec]", "labelsmall");
				gameData.gameTime = GUI.HorizontalSlider(new Rect(w/2-150, h/2+120-offsetY, 300, 25), gameData.gameTime, 60f, 600f, "horizontalslider", "horizontalsliderthumb");
				gameData.gameTime = Mathf.Round(gameData.gameTime / 30f) * 30f;
			}

			// powerups
			if (selVal == 1) {
				scrollPosition2 = GUI.BeginScrollView(new Rect(w/2-170, h/2-55-offsetY, 340, 200), scrollPosition2, new Rect(0, 0, 320, gameData.powerUps.Length*30f+7f));
				for (int i = 0; i < gameData.powerUps.Length; i++) {
					PowerUpInfo cur = gameData.powerUps[i];
					string text = "Spawn " + cur.name + " every " + cur.spawnFrequency + " secs";
					if (cur.isDisableable && cur.spawnFrequency == 80f) {
						text = cur.name + " disabled";
					}
					GUI.Label (new Rect(12f, 7f+(float)i*30f, 300, 12), text, "labelsmall");
					cur.spawnFrequency = GUI.HorizontalSlider(new Rect(12f, 7f+15f+(float)i*30f, 300, 25), cur.spawnFrequency, 5f, 80.0f, "horizontalslider", "horizontalsliderthumb");
				}
				GUI.EndScrollView();
			}

			// tweak
			if (selVal == 2) {
				float curHeight = 0;
				// calculate height of window inside scrollbars 
				for (int i = 0; i < gameData.powerUps.Length; i++) {
					PowerUpInfo cur = gameData.powerUps[i];
					for (int t = 0; t < cur.properties.Length; t++) {
						curHeight += 30f;
					}
					if (cur.properties.Length > 0) {
						curHeight += 15f;
					}
				}
				curHeight += 25f;

				scrollPosition = GUI.BeginScrollView(new Rect(w/2-170, h/2-55-offsetY, 340, 200), scrollPosition, new Rect(0, 0, 320, curHeight)); 

				curHeight = 0;
				for (int i = 0; i < gameData.powerUps.Length; i++) {
					PowerUpInfo cur = gameData.powerUps[i];
					if (cur.properties.Length != 0) {
						GUI.Label (new Rect(12, 7f+curHeight, 300, 12), cur.name, "labelsmall");
					}
					for (int t = 0; t < cur.properties.Length; t++) {
						PowerUpProperty prop = cur.properties[t];
						string text = prop.name + ": " + prop.val + " " + prop.unit;
						GUI.Label (new Rect(12, 7f+curHeight+15f, 300, 12), text, "labelsmall");
						prop.val = GUI.HorizontalSlider(new Rect(12, 7f+curHeight+30f, 300, 25), prop.val, prop.min, prop.max, "horizontalslider", "horizontalsliderthumb");
						curHeight += 30f;
					}
					if (cur.properties.Length > 0) {
						curHeight += 15f;
					}
				}
				GUI.EndScrollView();
			}

			if (Time.time < buildDiscsActivateTime) {
				GUI.enabled = false;
			}
			if (GUI.Button (new Rect(w/2-150, h/2+150-offsetY, 300, 30), "Build Discs", "button")) {
				GameObject scripts = GameObject.Find ("Scripts");
				buildDiscsActivateTime = Time.time + 1f;
				if (scripts != null) {
					GameMain main = scripts.GetComponent<GameMain>();
					main.CreateDiscs();
				}
			}
			GUI.enabled = true;
			
			if (GUI.Button (new Rect(w/2-150, h/2+180-offsetY, 300, 30), "Start Game", "button")) {
				state = EMenuState.GameStarted;
				SaveGameData();
				GameObject scripts = GameObject.Find ("Scripts");
				SendPowerUpProperties();
				if (scripts != null) {
					GameMain main = scripts.GetComponent<GameMain>();
					main.StartGame();
				}
			}
		}
	}

	[RPC]
	void SetPowerUpProperty(string typeName, string propName, float val) {
		PowerUpProperty prop = gameData.GetPowerUpProperty(typeName, propName);
		prop.val = val;
	}

	void SendPowerUpProperties() {
		PowerUpInfo[] powerUps = gameData.powerUps;
		for (int i = 0; i < powerUps.Length; i++) {
			for (int z = 0; z < powerUps[i].properties.Length; z++) {
				PowerUpProperty prop = powerUps[i].properties[z];
				photonView.RPC ("SetPowerUpProperty", PhotonTargets.Others, powerUps[i].typeName, prop.name, prop.val);
			}
		}
	}

	List<string> chatHistory = new List<string>();

	[RPC]
	void Chat(string s) {
		chatHistory.Add (s);
	}

	void TalkMode(int w, int h, Event e) {
		if (e.keyCode == KeyCode.Return) {
			talk_mode = false;
			GameObject player = GameObject.Find ("Player"+PhotonNetwork.player.ID);
			player.GetComponent<PlayerLogic>().Say(talkString);
			photonView.RPC ("Chat", PhotonTargets.All, PhotonNetwork.player.name + ": " + talkString);
			GUI.FocusControl("");
		} else {
			GUI.SetNextControlName("talkField");
			talkString = GUI.TextField(new Rect(w/2-150, h/2, 300, 38), talkString);
			
			if (GUI.GetNameOfFocusedControl() != "talkField") {
				GUI.FocusControl("talkField");
			}
		}
	}

	void ShowChatWindow(int w, int h, Event e) {
		List<string> disp = chatHistory.GetRange(Mathf.Max (0, chatHistory.Count - 11), Mathf.Min (chatHistory.Count, 10));
		GUI.Box (new Rect(w/2-170, h/2-145, 340, 275),"");
		int cnt = 0;
		foreach (string s in disp) {
			int y = h/2-130+cnt*15;
			GUI.Label (new Rect(w/2-150, y, 300, 12), s, "labelsmall");
			cnt++;
		}
	}

	[RPC]
	void GentlemenStartYourEngines() {
		if (!PhotonNetwork.isMasterClient) {
			state = EMenuState.GameStarted;
		}
		Application.LoadLevel("MainGame");
	}

	public bool gameStarted = false;

	void SaveGameData() {
		FileStream file = null;
		if (!File.Exists(Application.persistentDataPath + "/gameInfo.dat")) {
			file = File.Create (Application.persistentDataPath + "/gameInfo.dat");
		} else {
			file = File.Open (Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
		}
		BinaryFormatter bf = new BinaryFormatter();

		bf.Serialize(file, gameData);
		file.Close ();
	}

	void LoadGameData() {
		//TODO: if new powerup is created defaults need to be used
		if (File.Exists(Application.persistentDataPath + "/gameInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
			gameData = (GameData)bf.Deserialize(file);
			file.Close ();
			GameData gd = new GameData();
			// if version of GameData has changed, set defaults...
			if (gameData.version != gd.version) {
				gameData = new GameData();
			}
		} else {
			gameData = new GameData();
		}
	}

	void OnJoinedLobby()
	{

	}
	
	void OnPhotonRandomJoinFailed()
	{		
		Debug.Log("Can't join random room!");
	}
	
	void OnCreatedRoom()
	{
		SaveGameData();
	}
	
	void OnJoinedRoom()
	{

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.T) && 
		    (state == EMenuState.GameStarted || state == EMenuState.RoundSettings) 
		    && !talk_mode) {
			talk_mode = true;
			talkString = "";
		}
		if (Input.GetKeyUp(KeyCode.C) && 
		    (state == EMenuState.GameStarted || state == EMenuState.RoundSettings) 
		    && !talk_mode) {
			show_chat_window = !show_chat_window;
		}
	}

	bool show_chat_window = false;
	bool talk_mode = false;
	public string talkString = "";
}

public enum EMenuState {
	EnterName, MainMenu, CreateRoom, JoinRoom, WaitingForPlayers, MatchSettings, RoundSettings, GameStarted
}

[Serializable]
public class GameData
{
	// level
	public int version;
	public string playerName;
	public float gameTime;
	public float numDiscs;
	public float avgNumSlices;
	public float maxDiscSpeed;
	public float avgHoleSize;
	public float numFlagItems;
	public float gamesToWin;
	public string roomName;

	// powerups
	public PowerUpInfo[] powerUps;

	public GameData() {
		SetDefaults();
	}

	public void SetDefaults() {

		//remember to increase version when changing structure
		this.version = 12;

		this.playerName = "";
		this.roomName = "";
		this.maxDiscSpeed = 50f;
		this.avgHoleSize = 0.7f;
		this.avgNumSlices = 5f;
		this.numDiscs = 5f;
		this.numFlagItems = 3f;
		this.gameTime = 180f;
		this.gamesToWin = 3f;

		PowerUpInfo rocketInfo = new PowerUpInfo(10f, "PowerUpRocket", "rocket", true);
		PowerUpProperty rocketSpeed = new PowerUpProperty("speed", 20f, 500f, 80f, "px/sec");
		PowerUpProperty rocketRadius = new PowerUpProperty("explosion radius", 10f, 30f, 15f, "");
		PowerUpProperty rocketPower = new PowerUpProperty("explosion strength", 1000f, 15000f, 3500f, "");
		rocketInfo.properties = new PowerUpProperty[] {rocketSpeed,rocketRadius,rocketPower};

		PowerUpInfo bombInfo = new PowerUpInfo(10f, "PowerUpBomb", "bomb", true);
		PowerUpProperty bombTimer = new PowerUpProperty("explosion delay", 0.5f, 7f, 3f, "secs");
		PowerUpProperty bombRadius = new PowerUpProperty("explosion radius", 10f, 30f, 15f, "");
		PowerUpProperty bombPower = new PowerUpProperty("explosion strength", 1000f, 15000f, 3500f, "");
		bombInfo.properties = new PowerUpProperty[] {bombTimer,bombRadius,bombPower};

		PowerUpInfo pullInInfo = new PowerUpInfo(10f, "PowerUpPullIn", "magnetic attractor", true);
		PowerUpProperty strength = new PowerUpProperty("strength", 50f, 1000f, 500f, "");
		PowerUpProperty radius = new PowerUpProperty("radius", 3f, 20f, 10f, "");
		PowerUpProperty ttl = new PowerUpProperty("time to live", 1f, 20f, 5f, "secs");
		pullInInfo.properties = new PowerUpProperty[] {strength, radius, ttl};

		PowerUpInfo teleportInfo = new PowerUpInfo(10f, "PowerUpTeleport", "teleport", true);

		PowerUpInfo pushAwayInfo = new PowerUpInfo(10f, "PowerUpPushAway", "push away", true);
		PowerUpProperty str = new PowerUpProperty("strength", 7000f, 15000f, 9000f, "megatons");
		PowerUpProperty fireTime = new PowerUpProperty("fire time", 1f, 20f, 7f, "secs");
		pushAwayInfo.properties = new PowerUpProperty[] {str, fireTime};

		PowerUpInfo shieldInfo = new PowerUpInfo(10f, "PowerUpShield", "shield", true);
		PowerUpProperty duration = new PowerUpProperty("duration", 1f, 20f, 10f, "secs");
		shieldInfo.properties = new PowerUpProperty[] {duration};

		PowerUpInfo slowDownInfo = new PowerUpInfo(10f, "PowerUpSlowDown", "slow down", false);

		PowerUpInfo breakSpellsInfo = new PowerUpInfo(10f, "PowerUpBreakSpells", "break spells", false);

		PowerUpInfo gemRemoverInfo = new PowerUpInfo(10f, "PowerUpGemRemover", "gem remover", false);

		PowerUpInfo ammoInfo = new PowerUpInfo(10f, "PowerUpAmmo", "ammo", false);

		//remember to increase version when changing structure

		powerUps = new PowerUpInfo[] {rocketInfo, bombInfo, ammoInfo, pullInInfo, teleportInfo, shieldInfo, pushAwayInfo, slowDownInfo, breakSpellsInfo, gemRemoverInfo};
	}

	public PowerUpProperty GetPowerUpProperty(string typeName, string propertyName) {
		for (int i = 0; i < powerUps.Length; i++) {
			if (powerUps[i].typeName == typeName) {
				return powerUps[i].GetPowerUpProperty(propertyName);
			}
		}
		return null;
	}
}

[Serializable]
public class PowerUpProperty {

	public PowerUpProperty(string name,
	                       float min,
	                       float max,
	                       float defaultVal,
	                       string unit) {
		this.name = name;
		this.min = min;
		this.max = max;
		this.defaultVal = defaultVal;
		this.val = this.defaultVal;
		this.unit = unit;
	}

	public string name;
	public float min;
	public float max;
	public float val;
	public float defaultVal;
	public string unit = "";
}

[Serializable]
public class PowerUpInfo {

	public PowerUpInfo(float spawnFrequency,
	                   string typeName,
	                   string name,
	                   bool isDisableable) {
		this.spawnFrequency = spawnFrequency;
		this.typeName = typeName;
		this.name = name;
		this.isDisableable = isDisableable;
		this.isDisabled = false;
		this.nextSpawnTime = 0f;
		properties = new PowerUpProperty[0];
	}

	public PowerUpProperty[] properties = new PowerUpProperty[0];

	public float spawnFrequency;
	public string typeName;
	public string name;
	public bool isDisableable;
	public bool isDisabled;

	public float nextSpawnTime = 0f;

	public float IncreaseNextSpawnTime() {
		float min = spawnFrequency * 0.8f;
		float max = spawnFrequency * 1.2f;
		nextSpawnTime = Time.time + UnityEngine.Random.Range (min, max);
		return nextSpawnTime;
	}

	public PowerUpProperty GetPowerUpProperty(string name) {
		for (int i = 0; i < properties.Length; i++) {
			if (properties[i].name == name) {
				return properties[i];
			}
		}
		return null;
	}
}