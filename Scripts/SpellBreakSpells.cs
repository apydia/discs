using UnityEngine;
using System.Collections;

public class SpellBreakSpells : SpellBase {
	
	// Use this for initialization
	void Start () {
		
	}
	
	public override void Begin (GameObject player)
	{
		base.Begin (player);
	}
	
	public override void End (GameObject player)
	{
		player.GetComponent<PlayerLogic>().RemoveAllSpells();
		base.End (player);
	}
	
	public override string GetName ()
	{
		return "SpellBreakSpells";
	}
	
	public override object[] GatherInitData ()
	{
		return new object[] {rid, cid, duration};
	}
	
	public override void Init (int id, object[] initData, double spawnTime)
	{
		this.id = id;
		rid = (int)initData[0];
		cid = (int)initData[1];
		duration = (float)initData[2];
		createTime = (float) spawnTime;
	}
}
