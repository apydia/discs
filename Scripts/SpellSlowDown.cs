using UnityEngine;
using System.Collections;

public class SpellSlowDown : SpellBase {

	public float rate = 0.5f;

	// Use this for initialization
	void Start () {
	
	}

	public override void Begin (GameObject player)
	{
		player.GetComponent<PlayerController>().speed *= this.rate;
		player.GetComponent<PlayerLogic>().spells.Add(this);
		base.Begin (player);
	}

	public override void Break (GameObject player)
	{
		End (player);
	}

	public override void End (GameObject player)
	{
		player.GetComponent<PlayerController>().speed /= this.rate;
		base.End (player);
	}

	public override string GetName ()
	{
		return "SpellSlowDown";
	}

	public override object[] GatherInitData ()
	{
		return new object[] {rate, rid, cid, duration};
	}

	public override void Init (int id, object[] initData, double spawnTime)
	{
		this.id = id;
		rate = (float)initData[0];
		rid = (int)initData[1];
		cid = (int)initData[2];
		duration = (float)initData[3];
		createTime = (float) spawnTime;
	}
}
