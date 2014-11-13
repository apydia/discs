using UnityEngine;
using System.Collections;

public class PowerUpPullIn : MonoBehaviour, PowerUp {
	public int id;
	public Texture texture;
	
	// Use this for initialization
	void Start () {
		renderer.material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public string GetName() {
		return "PowerUpPullIn";
	}
	
	public int GetId() {
		return id;
	}
	
	public object[] GatherInitData() {
		return new object[]{};
	}
	
	public void Init(int id, object[] initData, double spawnTime) {
		this.id = id;
	}
	
	public void Activate(GameObject player, Vector3 pos) {
		GameObject obj = GameObject.Find ("Scripts");
		GameMain main = obj.GetComponent<GameMain>();
		PowerUpProperty attractorStrength = main.GetPowerUpProperty("PowerUpPullIn", "strength");
		PowerUpProperty attractorRadius = main.GetPowerUpProperty("PowerUpPullIn", "radius");
		PowerUpProperty attractorTTL = main.GetPowerUpProperty("PowerUpPullIn", "time to live");
		MagneticActor actor = new MagneticActor();
		actor.power = -1f*attractorStrength.val;
		actor.radius = attractorRadius.val;
		actor.timeToLive = attractorTTL.val;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(actor, pos);
		//.Find ("Scripts").GetComponent<MayhemSpawner>().SpawnBomb(pos);
	}
	
	void OnTriggerEnter(Collider other) {
		// TODO: abstract this for all Collectible PowerUps?!
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerLogic>().PowerUpCollected(this);
		}
	}
}