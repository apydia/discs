using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class PowerUpAmmo : PowerUpBase {

	public override string GetName() {
		return "PowerUpAmmo";
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController>().gunAmmoAmount += 10;
			other.gameObject.GetComponent<PlayerLogic>().PowerUpDestroyed(id);
		}
		if (other.gameObject.tag == "FlagItem") {
			this.gameObject.renderer.enabled = false;
			//GameObject.Destroy(this.gameObject);
		} else {
			this.gameObject.renderer.enabled = true;
		}
		/*
		if (other.gameObject.tag == "PowerUpCrate") {
			GameObject doomed = other.gameObject;
			Debug.Log ("GetName(): " + GetName());


			Type t = Type.GetType ("PowerUpBase");
			PowerUpBase b = (PowerUpBase)other.gameObject.GetComponent(t);
			if (this.createTime > b.createTime) {
				doomed = this.gameObject;
			}
			Vector3 pos = doomed.transform.position;
			pos.y += 5f;
			doomed.transform.position = pos;
			//GameObject.Destroy(doomed);

		}*/

		/*
		if (other.gameObject.tag == "PowerUpCrate") {
			GameObject doomed = other.gameObject;
			if (this.createTime > other.gameObject.GetComponent<PowerUpBase>().createTime) {
				doomed = this.gameObject;
			}
			GameObject.Destroy(doomed);
		}*/
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "FlagItem") {
			this.gameObject.renderer.enabled = true;
			//GameObject.Destroy(this.gameObject);
		} 

	}
}
