using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class PlayerLogic : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 correctPlayerVel;
	private Vector3 playerMov;

	public PowerUp[] powerUps;
	public int numPowerUps = 0;

	public List<FlagItem> flagItems;

	public List<Spell> spells;

	public float discSpeed = 0f;
	public int playerID;
	public int hasFlagOfPlayer = -1;
	public float initX = 0f;
	public float initZ = 0f;

	private bool _isShieldOn = false;
	
	public bool isShieldOn
	{
		get { return this._isShieldOn; }
	}

	public double lastUpdate = 0d;

	public int selectedPowerUpIndex = 0;

	public string clipName;

	public GameObject guiTextureBomb;
	public GameObject guiTextureRocket;
	public GameObject guiTexturePowerUpSelector;
	public GameObject guiTexturePullIn;
	public GameObject guiTextureTeleport;
	public GameObject guiTexturePushAway;
	public GameObject guiTextureShield;
	public GameObject guiTextureSlowDown;
	public GameObject guiTextureBreakSpells;
	public GameObject guiTextureGemRemover;

	public GameObject spellSlowDown;
	public GameObject spellBreakSpells;
	public GameObject spellTeleport;
	public GameObject spellGemRemover;

	public GameObject playerMarker;
	public GameObject playerSpeech;
	public GameObject playerSelectCollider;
	public GameObject playerShield;

	public GameObject respawnEffect;

	public GameObject speech;
	GameObject spawnedShield;

	Dictionary<string, GameObject> spellObjs;
	Dictionary<string, GameObject> gameObjs;

	void Start() {
		lastUpdate = PhotonNetwork.time;
		flagItems = new List<FlagItem>();
		spells = new List<Spell>();
		powerUps = new PowerUp[8];
		numPowerUps = 0;

		gameObjs = new Dictionary<string, GameObject>();
		gameObjs.Add("PowerUpBomb", guiTextureBomb);
		gameObjs.Add("PowerUpRocket", guiTextureRocket);
		gameObjs.Add("PowerUpPullIn", guiTexturePullIn);
		gameObjs.Add("PowerUpTeleport", guiTextureTeleport);
		gameObjs.Add("PowerUpPushAway", guiTexturePushAway);
		gameObjs.Add("PowerUpShield", guiTextureShield);
		gameObjs.Add("PowerUpSlowDown", guiTextureSlowDown);
		gameObjs.Add("PowerUpBreakSpells", guiTextureBreakSpells);
		gameObjs.Add("PowerUpGemRemover", guiTextureGemRemover);

		spellObjs = new Dictionary<string, GameObject>();
		spellObjs.Add("SpellSlowDown", spellSlowDown);
		spellObjs.Add("SpellBreakSpells", spellBreakSpells);
		spellObjs.Add("SpellTeleport", spellTeleport);
		spellObjs.Add("SpellGemRemover", spellGemRemover);

		UpdatePowerUpHUD();
	}
	
	void PlayerScored(int points) {
		GameObject.Find ("Scripts").GetComponent<GameMain> ().PlayerScored (playerID, points);
	}

	void PutFlagItemsInSlotsOnTopOfEachOther() {
		Vector3 pos = this.transform.position;
		
		float cnt = (float)PhotonNetwork.playerList.Length;//flagItems.Count;
		float delta = 2f*Mathf.PI/cnt;
		float cur = 0f;
		Dictionary<int, float> playerIDToIndex = new Dictionary<int, float>();
		Dictionary<int, int> itemsInSlots = new Dictionary<int, int>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			PhotonPlayer player = PhotonNetwork.playerList[i];
			playerIDToIndex.Add (player.ID, (float)i);
			itemsInSlots.Add (player.ID, 0);
		}
		
		int itemsInCurSlot = 0;
		
		List<FlagItem> deleteThese = new List<FlagItem>();
		foreach (FlagItem flag in flagItems) {
			if (flag != null) {
				itemsInSlots.TryGetValue(flag.playerID, out itemsInCurSlot);
				itemsInSlots.Remove(flag.playerID);
				itemsInSlots.Add(flag.playerID, ++itemsInCurSlot);
				playerIDToIndex.TryGetValue(flag.playerID, out cur);
				flag.transform.position = new Vector3(pos.x + 2f * Mathf.Sin ((float)cur*delta), (float)itemsInCurSlot*2f, 
				                                      pos.z + 2f * Mathf.Cos ((float)cur*delta) );
			} else {
				deleteThese.Add (flag);
			}
			cur++;
		}
		
		foreach (FlagItem flag in deleteThese) {
			flagItems.Remove (flag);
		}
	}

	void PutFlagItemsInSlots() {
		Vector3 pos = this.transform.position;
		
		float cnt = (float)flagItems.Count;
		float delta = 2f*Mathf.PI/cnt;
		int cur = 0;
		Vector3 scale = new Vector3(0.15f, 0.15f, 0.15f);
		if (cnt > 5f) {
			scale = new Vector3(0.1f, 0.1f, 0.1f);
		}
		if (cnt > 7f) {
			scale = new Vector3(0.2f/3f, 0.2f/3f, 0.2f/3f);
		}
		List<FlagItem> deleteThese = new List<FlagItem>();
		foreach (FlagItem flag in flagItems) {
			if (flag != null) {
				flag.transform.position = new Vector3(pos.x + 2f * Mathf.Sin ((float)cur*delta), flag.transform.position.y, 
				                                      pos.z + 2f * Mathf.Cos ((float)cur*delta) );
				flag.transform.localScale = scale;
				flag.gameObject.GetComponent<MeshCollider>().enabled = false;
				flag.gameObject.GetComponent<SphereCollider>().enabled = false;

			} else {
				deleteThese.Add (flag);
			}
			cur++;
		}
		
		foreach (FlagItem flag in deleteThese) {
			flagItems.Remove (flag);
		}
	}

	int spellID = 0;

	[RPC]
	public void AddSpellRPC(string name, int id, object[] initData, int castingPlayer) {
		GameObject sp;
		bool has = spellObjs.TryGetValue(name, out sp);
		if (has) {
			GameObject spellObj = (GameObject)Instantiate(sp, transform.position, Quaternion.identity);
			spellObj.transform.parent = gameObject.transform;
			spellObj.transform.Rotate (new Vector3(-90f, 0f, 0f));
			Type t = Type.GetType(name);
			Spell spell = (Spell)spellObj.GetComponent(t);
			spell.Init (id, initData, PhotonNetwork.time);
			spell.Begin (this.gameObject);

			//spells.Add (spell);
		}
	}

	public void AddSpell(Spell spell) {
		photonView.RPC ("AddSpellRPC", PhotonTargets.All, spell.GetName(), spellID++, spell.GatherInitData(), PhotonNetwork.player.ID);
	}

	[RPC]
	void RemoveSpellRPC(int id) {
		Spell spell = spells.Find (item => item.GetId() == id);
		if (spell != null) {
			spells.Remove(spell);
		}
	}

	public void RemoveSpell(int id) {
		photonView.RPC ("RemoveSpellRPC", PhotonTargets.All, id);
	}

	[RPC]
	public void RemoveAllSpellsRPC() {
		List<Spell> spellsToRemove = new List<Spell>();
		foreach (Spell spell in spells) {
			spellsToRemove.Add (spell);
		}
		foreach (Spell spell in spellsToRemove) {
			spell.Break (this.gameObject);
			spells.Remove (spell);
		}
		//spells = new List<Spell>();
	}

	public void RemoveAllSpells() {
		photonView.RPC ("RemoveAllSpellsRPC", PhotonTargets.All, new object[]{});
	}

	[RPC]
	public void CapturedFlagItemRPC(int itemId) {
		GameObject flagItem = GameObject.Find ("PowerUp"+itemId);
		if (flagItem.GetComponent<FlagItem>().capturedByPlayer == -1) {
			flagItem.GetComponent<FlagItem>().capturedByPlayer = this.playerID;
			flagItem.transform.parent = transform;
			flagItems.Add (flagItem.GetComponent<FlagItem>());

			PutFlagItemsInSlots();
		}
	}

	public void CapturedFlagItem(int itemId) {
		photonView.RPC ("CapturedFlagItemRPC", PhotonTargets.All, itemId);
	}

	[RPC]
	public void LostFlagItemRPC(int itemId) {
		GameObject flagItem = GameObject.Find ("PowerUp"+itemId);
		if (flagItem != null) {
			//if (flagItem.GetComponent<FlagItem>().capturedByPlayer == playerID) {
				flagItem.GetComponent<FlagItem>().capturedByPlayer = -1;
				flagItem.transform.parent = null;
				int idx = flagItems.FindIndex (item => item.GetId() == itemId);

				if (idx != -1) {
					flagItems.RemoveAt(idx);
				}
				if (flagItem.GetComponent<FlagItem>() != null) {
					flagItem.GetComponent<FlagItem>().Reset();
				}

				PutFlagItemsInSlots();
			//}
		} else {
			Debug.LogError ("flag-item was already destroyed...");
		}
	}

	public void LostFlagItem(FlagItem item) {
		photonView.RPC ("LostFlagItemRPC", PhotonTargets.All, item.id);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		List<FlagItem> removeThose = new List<FlagItem>();
		foreach (FlagItem item in flagItems) {
			if (item.playerID == player.ID) {
				removeThose.Add (item);
			}
		}
		foreach (FlagItem item in removeThose) {
			flagItems.Remove (item);
		}
		removeThose = new List<FlagItem>();
		PutFlagItemsInSlots ();
	}

	public void LoseFlagItem(int itemId) {
		FlagItem flagItem = flagItems.Find (item => item.id == itemId);
		if (flagItem != null) {
			photonView.RPC ("LostFlagItemRPC", PhotonTargets.All, itemId);
		}
	}

	public void LoseRandomFlagItem(int playerId) {
		if (flagItems.Count > 0) {
			if (PhotonNetwork.player.ID == playerId) {
				int rand = UnityEngine.Random.Range (0, flagItems.Count - 1);
				LostFlagItem(flagItems.ToArray()[rand]);
			}
		}
	}

	public void LoseAllFlagItems() {
		FlagItem[] items = flagItems.ToArray();
		for (int i = 0; i < items.Length; i++) {
			LostFlagItem(items[i]);
		}
	}

	[RPC]
	public void PowerUpDestroyedRPC(int itemId) {
		GameObject powerUp = GameObject.Find ("PowerUp"+itemId);
		if (powerUp != null) {
			GameObject.Destroy (powerUp);
		}
		//UpdatePowerUpHUD();
	}

	[RPC]
	public void PowerUpCollectedRPC(string type, int itemId) {
		if (numPowerUps < 8) {
			GameObject powerUp = GameObject.Find ("PowerUp"+itemId);
			Type t = Type.GetType(type);
			PowerUp p = (PowerUp) powerUp.GetComponent(t);
			this.powerUps[numPowerUps++] =  p;
			UpdatePowerUpHUD();

			photonView.RPC ("PowerUpDestroyedRPC", PhotonTargets.All, itemId);
		}
	}
	
	public void PowerUpCollected(PowerUp powerUp) {
		photonView.RPC ("PowerUpCollectedRPC", PhotonPlayer.Find(this.playerID), powerUp.GetName(), powerUp.GetId ());
	}

	public void PowerUpDestroyed(int itemId) {
		photonView.RPC ("PowerUpDestroyedRPC", PhotonTargets.All, itemId);
	}

	public void SelectPrevPowerUp() {
		if (numPowerUps > 0) {
			selectedPowerUpIndex--;
		}
		if (selectedPowerUpIndex == -1) {
			selectedPowerUpIndex = numPowerUps - 1;
		}
		UpdatePowerUpHUD();
	}

	public void SelectNextPowerUp() {
		if (numPowerUps > 0) {
			selectedPowerUpIndex = (selectedPowerUpIndex + 1) % numPowerUps;
		}
		UpdatePowerUpHUD();
	}

	public PowerUp GetSelectedPowerUp() {
		if (numPowerUps > 0) {
			if (selectedPowerUpIndex < numPowerUps) {
				return powerUps[selectedPowerUpIndex];
			}
		}
		return null;
	}

	List<PowerUp> usedPowerUps = new List<PowerUp>();

	public PowerUp UseSelectedPowerUp() {
		PowerUp pUp = GetSelectedPowerUp();
		if (pUp != null) {
			for (int i = selectedPowerUpIndex; i < numPowerUps - 1; i++) {
				powerUps[i] = powerUps[i+1];
			}
			--numPowerUps;
			if (selectedPowerUpIndex > 0) {
				--selectedPowerUpIndex;
			}

		}
		UpdatePowerUpHUD();
		usedPowerUps.Add (pUp);
		return pUp;
	}

	/*
	[RPC]
	public void PowerUpCollectedRPC(int itemId) {
		GameObject powerUp = GameObject.Find ("PowerUp"+itemId);
		if (powerUp != null) {
			GameObject.Destroy (powerUp);
		}
	}

	public void PowerUpCollected(PowerUp powerUp) {
		this.powerUps.Add (powerUp);
		UpdatePowerUpHUD();
		photonView.RPC ("PowerUpCollectedRPC", PhotonTargets.All, powerUp.GetId());
	}
*/
	public void UpdatePowerUpHUD() {
		GameObject[] objs = GameObject.FindGameObjectsWithTag("PowerUpGUITexture");

		for (int i = 0; i < objs.Length; i++) {
			GameObject.Destroy (objs[i]);
		}

		int w = Screen.width;
		int h = Screen.height;

		float curX = w/2 - 4 * 37;

		for (int cnt = 0; cnt < numPowerUps; cnt++) {
			PowerUp powerUp = powerUps[cnt];
			GUITexture texture = null;
			GameObject prefab;
			bool hasObj = gameObjs.TryGetValue(powerUp.GetName(), out prefab);

			if (hasObj) {
				GameObject obj = (GameObject) Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				texture = obj.GetComponent<GUITexture>();
			}

			if (texture != null) {
				Rect rect = new Rect(texture.pixelInset);
				
				//texture.color = new Color(1f, 1f, 1f, 0.5f);
				rect.x = w - 35;
				rect.y = h - (37*cnt) - 80;



				rect.x = (curX); 
				rect.y = 8;
				if (cnt == selectedPowerUpIndex) {
					rect.width = 64f;
					rect.height = 64f;
				}
				texture.pixelInset = rect;

				if (cnt == selectedPowerUpIndex) {
					GameObject objSel = (GameObject) Instantiate(guiTexturePowerUpSelector, new Vector3(0f, 0f, 0f), Quaternion.identity);
					GUITexture selector = objSel.GetComponent<GUITexture>();

					Rect r = new Rect(selector.pixelInset);

					r.x = w - 62;
					r.y = h - (37*cnt) - 85;

					r.x = curX - 10; 
					r.y = -2;

					selector.pixelInset = r;
					curX += 37f;
				}
				curX += 37f;
			}
		}

		int ammo = gameObject.GetComponent<PlayerController>().gunAmmoAmount;
		
		string s = "x " + ammo + "\n";

		GameObject gameObj = GameObject.Find ("AmmoText");
		gameObj.GetComponent<GUIText> ().richText = true;
		gameObj.GetComponent<GUIText> ().text = s;
	}

	bool doPrediction = true;
	public Vector3 extPos;
	// Update is called once per frame
	// TODO: better prediction model... movement relative to last position
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			doPrediction = !doPrediction;
		}

		if (photonView.isMine && rigidbody.transform.position.y < -33f && !didDie) {
			Die ();
		}

		if (photonView.isMine && Time.time > respawnTime && didDie) {
			rigidbody.transform.position = new Vector3(initX, 1f, initZ);
		    transform.position = new Vector3(initX, 1f, initZ);
		    rigidbody.velocity = Vector3.zero;
		    rigidbody.angularVelocity = Vector3.zero;

			didDie = false;
		}

		if ((float)PhotonNetwork.time < shieldDeactivateTime && !_isShieldOn) {
			Vector3 p = rigidbody.transform.position;
			spawnedShield = (GameObject) Instantiate (playerShield,  new Vector3(p.x, p.y+2f, p.z), Quaternion.identity);
			spawnedShield.transform.parent = rigidbody.transform;
			_isShieldOn = true;
			//spawnedShield.GetComponent<ParticleSystem>().particleEmitter.emit=true; 
		} else {
			if (_isShieldOn && (float)PhotonNetwork.time > shieldDeactivateTime) {
				GameObject.Destroy (spawnedShield.gameObject);
				_isShieldOn = false;
			}
			//spawnedShield.GetComponent<ParticleSystem>().particleEmitter.emit=false;
		}

		float pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
		float timeSinceLastUpdate = (float)(PhotonNetwork.time - lastUpdate);

		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

		float predictTime = totalTimePassed;
		if (!PhotonNetwork.isMasterClient) {
			predictTime += pingInSeconds;
		}

		if (!photonView.isMine)
		{
			if (this.clipName == "Walk") {
				animation["Walk"].speed = 1.5f;
			} 
			if (this.clipName != "") {
				gameObject.GetComponent<Animation>().Play (this.clipName);
			}

			extPos = this.correctPlayerPos + this.playerMov * predictTime;

			float radius = Vector3.Distance(Vector3.zero, extPos);

			float speed = GetSpeedOfDiscAtRadius(radius);

			if (transform.position.y > 1.0f) {
				speed = 0f;
			}

			extPos = Quaternion.AngleAxis(speed * predictTime, Vector3.up) *  this.correctPlayerPos + this.playerMov * predictTime;

			if (doPrediction) {

				transform.position = Vector3.Lerp(transform.position, extPos, Time.deltaTime * 8);
				transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 8);
			} else {
				transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 8);
				transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 8);
			}

			if (Vector3.Distance(transform.position, extPos) > 10f) {
				transform.position = extPos;
			}
		}
	}

	public float GetSpeedOfDiscAtRadius(float radius) {
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("DiscSlice");
		for (int i = 0; i < objs.Length; i++) {
			DiscSlice slice = objs[i].GetComponent<DiscSlice>();
			if (slice.innerRadius < radius && slice.outerRadius > radius) {
				return slice.speed;
			}
		}
		return 0f;
	}
	
	float shieldDeactivateTime;

	public void ActivateShield(float duration) {
		photonView.RPC ("ActivateShieldRPC", PhotonTargets.All, duration);
	}

	public void DeActivateShield() {
		photonView.RPC ("DeActivateShieldRPC", PhotonTargets.All);
	}

	[RPC]
	void DeActivateShieldRPC() {
		shieldDeactivateTime = 0f;
	}

	[RPC]
	void ActivateShieldRPC(float duration, PhotonMessageInfo info) {
		shieldDeactivateTime = (float)info.timestamp + duration;
	}

	[RPC]
	public void DieRPC(int playerID, float initX, float initZ) {
		//rigidbody.transform.position = new Vector3 (initX, 1f, initZ);
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;

		foreach (FlagItem item in flagItems) {
			item.Reset ();
		}
		if(initX != 0f || initZ != 0f) {
			Instantiate(respawnEffect, new Vector3(initX, 0f, initZ), Quaternion.identity);
		}
		foreach (PowerUp powerUp in usedPowerUps) {
			if (powerUp != null) {
				powerUp.DeActivate(this.gameObject);
			}
		}

		RemoveAllSpells();

		usedPowerUps = new List<PowerUp>();
		flagItems = new List<FlagItem>();
		powerUps = new PowerUp[8];
		//spells = new List<Spell>();
		numPowerUps = 0;
		selectedPowerUpIndex = 0;

		gameObject.GetComponent<PlayerController>().gunAmmoAmount = 
			Math.Min (gameObject.GetComponent<PlayerController>().gunAmmoAmount, 20);

		if (PhotonNetwork.player.ID == playerID) {
			UpdatePowerUpHUD();
		}
	}

	float respawnTime;
	bool didDie = false;

	public void Die() {

		didDie = true;
		respawnTime = Time.time + 1f;

		photonView.RPC ("DieRPC", PhotonTargets.All, this.playerID, initX, initZ);
	}

	[RPC]
	void SayRPC(string what) {
		speech.GetComponent<PlayerSpeech>().Say(what);
	}
	
	public void Say(string what) {
		photonView.RPC ("SayRPC", PhotonTargets.All, what);
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Terrain") {
			this.Die ();
		}
	}

	bool isInited = false;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!isInited) {
			Vector3 c = (Vector3)photonView.instantiationData [0];
			playerID = (int)photonView.instantiationData [1];

			gameObject.name = "Player" + playerID;

			Color color = new Color (c.x, c.y, c.z);
			Vector3 p = rigidbody.transform.position;
			GameObject marker = (GameObject) Instantiate (playerMarker,  new Vector3(p.x, p.y + 1.5f * 6f, p.z), Quaternion.identity);
			marker.transform.parent = rigidbody.transform;
			marker.renderer.material.color = color;

			GameObject coll = (GameObject) Instantiate (playerSelectCollider,  new Vector3(p.x, p.y + 1.5f * 2f, p.z), Quaternion.identity);
			coll.transform.parent = rigidbody.transform;
			coll.GetComponent<PlayerSelectCollider>().player = this.gameObject;

			speech = (GameObject) Instantiate (playerSpeech,  new Vector3(p.x, p.y + 2f * 6f, p.z), Quaternion.identity);
			speech.transform.parent = rigidbody.transform;
			speech.GetComponent<TextMesh>().color = color;

			isInited = true;
		}
		PlayerController con = gameObject.GetComponent<PlayerController>();
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(gameObject.GetComponent<PlayerController>().clipName);
			stream.SendNext(gameObject.rigidbody.velocity+con.mov);
		}
		else
		{
			// Network player, receive data
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			this.clipName = (string)stream.ReceiveNext();
			this.playerMov = (Vector3)stream.ReceiveNext();
			lastUpdate = info.timestamp;
		}
	}
}