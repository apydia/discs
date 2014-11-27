using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class PowerUpBase : MonoBehaviour, PowerUp {
	
	public int id;
	public Texture texture;
	public double createTime;

	// Use this for initialization
	void Start () {
		renderer.material.mainTexture = texture;
		renderer.enabled = false;
	}

	public virtual string GetName() {
		return "";
	}

	// Update is called once per frame
	void Update () {
		if ((float)PhotonNetwork.time > createTime + 0.3f) {
			renderer.enabled = true;
		}
	}
	
	public int GetId() {
		return id;
	}
	
	public object[] GatherInitData() {
		return new object[]{};
	}
	
	public void Init(int id, object[] initData, double createTime) {
		this.id  = id;
		this.createTime = createTime;
	}
	
	public virtual void Activate(GameObject player, Vector3 pos) {
	}

	public virtual void DeActivate(GameObject player) {
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			if (other.gameObject.GetComponent<PlayerController>().playerID == PhotonNetwork.player.ID) {
				other.gameObject.GetComponent<PlayerLogic>().PowerUpCollected(this);
			}
		}
		if (other.gameObject.tag == "FlagItem") {
			//this.gameObject.renderer.enabled = false;
			renderer.material.color = new Color(1f, 1f, 1f, 0.3f);
			//GameObject.Destroy(this.gameObject);
		} else {
			//this.gameObject.renderer.enabled = true;
		}

		if (other.gameObject.tag == "PowerUpCrate") {
			GameObject doomed = other.gameObject;

			Type t = Type.GetType ("PowerUpBase");
			PowerUpBase b = (PowerUpBase)other.gameObject.GetComponent(t);
			if (this.createTime > b.createTime) {
				doomed = this.gameObject;
			}
			/*
			Vector3 pos = doomed.transform.position;
			pos.y += 5f;
			doomed.transform.position = pos;
			*/
			GameObject.Destroy(doomed);
			//
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "FlagItem") {
			renderer.material.color = new Color(1f, 1f, 1f, 0.3f);
			this.gameObject.renderer.enabled = true;
			//GameObject.Destroy(this.gameObject);
		} 
		
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "FlagItem") {
			renderer.material.color = new Color(1f, 1f, 1f, 1f);
			this.gameObject.renderer.enabled = true;
			//GameObject.Destroy(this.gameObject);
		} 
	
	}

}
