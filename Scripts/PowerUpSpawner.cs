using UnityEngine;

public class PowerUpSpawner : Photon.MonoBehaviour
{
	public GameObject prePostFlag;
	public GameObject bomb;
	public GameObject rocket;
	public GameObject ammo;

	int powerUpId = 0;

	public GameObject Spawn(int id, string type, Vector3 position, Quaternion rotation, object[] powerUpData) {
		switch (type) {
			case "FlagItem":
				GameObject obj = (GameObject) Instantiate(prePostFlag, position, rotation);
				obj.GetComponent<FlagItem>().Init (id, powerUpData);
				obj.name = "PowerUp"+id;
				return obj;
			case "PowerUpAmmo":
				position.y = 1;
				GameObject objAmmo = (GameObject) Instantiate(ammo, position, rotation);
				objAmmo.GetComponent<PowerUpAmmo>().Init (id, powerUpData);
				objAmmo.name = "PowerUp"+id;
				return objAmmo;
			case "PowerUpBomb":
				position.y = 1;
				GameObject objBomb = (GameObject) Instantiate(bomb, position, rotation);
				objBomb.GetComponent<PowerUpBomb>().Init (id, powerUpData);
				objBomb.name = "PowerUp"+id;
				return objBomb;
			case "PowerUpRocket":
				position.y = 1;
				GameObject objRocket = (GameObject) Instantiate(rocket, position, rotation);
				objRocket.GetComponent<PowerUpRocket>().Init (id, powerUpData);
				objRocket.name = "PowerUp"+id;
				return objRocket;
			default:
				return null;
		}
	}

	void CreatePowerUp(int id, string type, int discId, int sliceId, Vector3 pos, object[] powerUpData, double startTime) {
		GameObject slice = GameObject.Find ("Disc"+discId+"Slice"+sliceId);
		
		//PowerUpSpawner spawner = GameObject.Find ("Scripts").GetComponent<PowerUpSpawner>();
		GameObject gObject = Spawn(id, type, pos, Quaternion.identity, powerUpData);
		
		if (gObject != null) {
			gObject.transform.parent = slice.GetComponent<DiscSlice>().sliceMesh.transform;
			float timePassed = (float)(PhotonNetwork.time - startTime );
			if (!PhotonNetwork.isMasterClient) {
				//timePassed -= PhotonNetwork.GetPing() * 0.001f;
			}
			float speed = slice.GetComponent<DiscSlice>().speed;
			gObject.transform.position = Quaternion.AngleAxis(speed * timePassed, Vector3.up) * gObject.transform.position;
		}
	}
	
	[RPC]
	void SpawnPowerUpRPC(int id, string type, int discId, int sliceId, Vector3 pos, object[] powerUpData, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		
		CreatePowerUp (id, type, discId, sliceId, pos, powerUpData, timestamp);
		
	}

	public void CreatePowerUpOnRandomSlice(PowerUp powerUp) {
		GameObject discs = (GameObject)GameObject.Find ("DiscsMain");
		if (discs != null) {
			
			Discs discsClone = discs.GetComponent<Discs>();
			int numDiscs = discsClone.discs.Length;
			int randDiscs = Random.Range (0, numDiscs - 1);
			
			Disc d = discs.GetComponent<Discs>().discs[randDiscs];
			int randSlices = Random.Range (0, d.slices.Length - 1);
			
			Vector3 randPos = d.slices[randSlices].GetRandomPos();
			
			photonView.RPC ("SpawnPowerUpRPC", PhotonTargets.AllViaServer, powerUpId++, powerUp.GetName(), randDiscs, randSlices, randPos, powerUp.GatherInitData());
		}
	}

}


