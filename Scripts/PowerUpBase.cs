using UnityEngine;
using System.Collections;

public class PowerUpBase : MonoBehaviour, PowerUp {
	
	public int id;
	public Texture texture;
	
	// Use this for initialization
	void Start () {
		renderer.material.mainTexture = texture;
	}

	public virtual string GetName() {
		return "";
	}

	// Update is called once per frame
	void Update () {
		
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
	
	public virtual void Activate(GameObject player, Vector3 pos) {
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerLogic>().PowerUpCollected(this);
		}
	}
}
