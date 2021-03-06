﻿using UnityEngine;
using System.Collections;

public class SpellTeleport : SpellBase {

	public Vector3 destination;
	public GameObject beacon;
	GameObject spawnedBeacon;

	// Use this for initialization
	void Start () {
		
	}
	
	public override void Begin (GameObject player)
	{
		player.GetComponent<PlayerLogic>().spells.Add(this);
		base.Begin (player);
	}

	public override void Break (GameObject player)
	{
		GameObject.Destroy(spawnedBeacon);
		base.Break (player);
	}

	public override void End (GameObject player)
	{
		GameObject sp = GameObject.Find ("SoundPlayer");
		if (sp != null) {
			SoundPlayer soundPlayer = sp.GetComponent<SoundPlayer>();
			if (soundPlayer != null) {
				soundPlayer.Play(GameSound.TELETRANSPORTO);
			}
		}
		player.transform.position = spawnedBeacon.transform.position;
		GameObject.Destroy(spawnedBeacon);
		base.End (player);
	}
	
	public override string GetName ()
	{
		return "SpellTeleport";
	}
	
	public override object[] GatherInitData ()
	{
		return new object[] {destination, rid, cid, duration};
	}
	
	public override void Init (int id, object[] initData, double spawnTime)
	{
		this.id = id;
		destination = (Vector3)initData[0];
		rid = (int)initData[1];
		cid = (int)initData[2];
		duration = (float)initData[3];
		createTime = (float) spawnTime;

		spawnedBeacon = (GameObject) Instantiate (beacon, destination, Quaternion.identity);
	}
}
