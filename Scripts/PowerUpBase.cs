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
		this.createTime = createTime;
	}
	
	public virtual void Activate(GameObject player, Vector3 pos) {
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerLogic>().PowerUpCollected(this);
		}
		if (other.gameObject.tag == "FlagItem") {
			this.gameObject.renderer.enabled = false;
			//GameObject.Destroy(this.gameObject);
		} else {
			//this.gameObject.renderer.enabled = true;
		}

		if (other.gameObject.tag == "PowerUpCrate") {
			GameObject doomed = other.gameObject;
			Debug.Log ("GetName(): " + GetName());

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
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "FlagItem") {
			this.gameObject.renderer.enabled = true;
			//GameObject.Destroy(this.gameObject);
		} 
	
	}

}
