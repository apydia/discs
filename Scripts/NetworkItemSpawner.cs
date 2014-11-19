using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

public class NetworkItemSpawner : Photon.MonoBehaviour
{
	public GameObject flagItem;
	public GameObject bombPowerUp;
	public GameObject rocketPowerUp;
	public GameObject pullInPowerUp;
	public GameObject teleportPowerUp;
	public GameObject pushAwayPowerUp;
	public GameObject ammoPowerUp;
	public GameObject shieldPowerUp;
	public GameObject slowDownPowerUp;
	public GameObject breakSpellsPowerUp;
	public GameObject bomb;
	public GameObject rocket;
	public GameObject magneticAttractor;
	public GameObject pushAway;

	Dictionary<string, GameObject> gameObjs;

	void Start() {
		gameObjs = new Dictionary<string, GameObject>();
		gameObjs.Add("FlagItem", flagItem);
		gameObjs.Add("PowerUpAmmo", ammoPowerUp);
		gameObjs.Add("PowerUpBomb", bombPowerUp);
		gameObjs.Add("PowerUpRocket", rocketPowerUp);
		gameObjs.Add("PowerUpPullIn", pullInPowerUp);
		gameObjs.Add("PowerUpTeleport", teleportPowerUp);
		gameObjs.Add("PowerUpPushAway", pushAwayPowerUp);
		gameObjs.Add("PowerUpShield", shieldPowerUp);
		gameObjs.Add("PowerUpSlowDown", slowDownPowerUp);
		gameObjs.Add("PowerUpBreakSpells", breakSpellsPowerUp);
		gameObjs.Add("Bomb", bomb);
		gameObjs.Add("Rocket", rocket);
		gameObjs.Add("MagneticAttractor", magneticAttractor);
		gameObjs.Add("PushAway", pushAway);
	}

	int itemID = 0;

	public GameObject Spawn(int id, string type, Vector3 position, Quaternion rotation, object[] initData, double createTime) {
		GameObject prefab;
		bool hasObj = gameObjs.TryGetValue(type, out prefab);
		if (hasObj) {
			GameObject spawned = (GameObject) Instantiate(prefab, position, rotation);
			Type t = Type.GetType ("Spawnable");
			Spawnable spawnable = (Spawnable)spawned.GetComponent(t);
			spawnable.Init (id, initData, createTime);
			spawned.name = "PowerUp"+id;
			return spawned;
		} else {
			Debug.LogError("PowerupSpawner->could not find gameobject of type: " + type);
			return null;
		}
	}

	void CreateItem(int id, string type, int discId, int sliceId, Vector3 pos, object[] initData, double startTime) {
		GameObject slice = GameObject.Find ("Disc"+discId+"Slice"+sliceId);

		GameObject gObject = Spawn(id, type, pos, Quaternion.identity, initData, startTime);
		
		if (gObject != null) {
			gObject.transform.parent = slice.GetComponent<DiscSlice>().sliceMesh.transform;
			float timePassed = (float)(PhotonNetwork.time - startTime );
			float pingInSeconds = PhotonNetwork.GetPing() * 0.001f;
			float totalTimePassed = timePassed;// + pingInSeconds;
			float predictTime = totalTimePassed;
			if (!PhotonNetwork.isMasterClient) {
				predictTime += pingInSeconds;
			}
			if (!PhotonNetwork.isMasterClient) {
				//timePassed -= PhotonNetwork.GetPing() * 0.001f;
			}
			float speed = slice.GetComponent<DiscSlice>().speed;
			gObject.transform.position = Quaternion.AngleAxis(speed * predictTime, Vector3.up) * gObject.transform.position;
		}
	}

	public void Spawn(Spawnable spawnable, Vector3 pos) {
		photonView.RPC ("SpawnItemRPC", PhotonTargets.AllViaServer, 
		                itemID++, spawnable.GetName(), pos, spawnable.GatherInitData());
	}

	[RPC]
	void SpawnItemRPC(int id, string type, Vector3 pos, object[] initData, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		Spawn (id, type, pos, Quaternion.identity, initData, timestamp);
	}

	[RPC]
	void SpawnItemOnSliceRPC(int id, string type, int discId, int sliceId, Vector3 pos, object[] initData, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		
		CreateItem (id, type, discId, sliceId, pos, initData, timestamp);
		
	}

	public void CreateItemOnRandomSlice(Spawnable spawnable) {
		GameObject discs = (GameObject)GameObject.Find ("DiscsMain");
		if (discs != null) {
			Discs discsClone = discs.GetComponent<Discs>();
			int numDiscs = discsClone.discs.Length;
			int randDiscs = UnityEngine.Random.Range (0, numDiscs - 1);
			
			Disc d = discs.GetComponent<Discs>().discs[randDiscs];
			int randSlices = UnityEngine.Random.Range (0, d.slices.Length - 1);
			
			Vector3 randPos = d.slices[randSlices].GetRandomPos();
			
			photonView.RPC ("SpawnItemOnSliceRPC", PhotonTargets.AllViaServer, itemID++, spawnable.GetName(), randDiscs, randSlices, randPos, spawnable.GatherInitData());
		}
	}

}


