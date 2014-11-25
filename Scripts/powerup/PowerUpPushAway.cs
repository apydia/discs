using UnityEngine;
using System.Collections;

public class PowerUpPushAway : PowerUpBase {

	GameObject pa;

	public override string GetName() {
		return "PowerUpPushAway";
	}
	
	public override void Activate(GameObject player, Vector3 pos) {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty strength = main.GetPowerUpProperty("PowerUpPushAway", "strength");
		PowerUpProperty fireTime = main.GetPowerUpProperty("PowerUpPushAway", "fire time");

		//GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(pushAway, player.transform.position);
		pa = PhotonNetwork.Instantiate("PushAway", 
		                                          player.transform.position, 
		                                          Quaternion.identity, 
		                                          0, 
		                                          new object[]{ PhotonNetwork.player.ID, 
																strength.val, 
																fireTime.val });
		pa.GetComponent<PushAway>().player = player;
		pa.name = "PushAway" + GetId ();
		//.Find ("Scripts").GetComponent<MayhemSpawner>().SpawnBomb(pos);
	}

	public override void DeActivate(GameObject player) {
		//GameObject o = GameObject.Find ("PushAway" + GetId ());
		if (pa != null) {
			pa.GetComponent<PushAway>().Kill ();
		}
	}
}
