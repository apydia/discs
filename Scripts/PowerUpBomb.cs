using UnityEngine;
using System.Collections;

public class PowerUpBomb : MonoBehaviour, PowerUp {

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
		return "PowerUpBomb";
	}

	public int GetId() {
		return id;
	}
	
	public object[] GatherInitData() {
		return new object[]{};
	}

	public void Init(int id, object[] initData, double createTime) {
		this.id  = id;
	}

	public void Activate(GameObject player, Vector3 pos) {
		Bomb bomb = new Bomb();
		bomb.position = player.transform.position;
		GameObject.Find ("Scripts").GetComponent<NetworkItemSpawner>().Spawn(bomb, pos);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerLogic>().PowerUpCollected(this);
		}
	}
}
