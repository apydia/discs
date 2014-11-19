using UnityEngine;
using System.Collections;

public class SpellBase : MonoBehaviour, Spell {

	public float duration = 10f;
	
	protected float createTime;
	protected int id;
	protected int rid, cid;
	protected GameObject player;

	// Use this for initialization
	void Start () {
		
	}

	#region Spell implementation
	
	public void SetReceivingPlayerID (int id)
	{
		rid = id;
	}
	
	public void SetCastingPlayerID (int id)
	{
		cid = id;
	}

	public virtual void Begin (GameObject player)
	{
		this.player = player;
	}

	public virtual void Break (GameObject player)
	{
		hasEnded = true;
		player.GetComponent<PlayerLogic>().RemoveSpell(GetId());
		GameObject.Destroy(this.gameObject);
	}

	public virtual void End (GameObject player)
	{
		hasEnded = true;
		player.GetComponent<PlayerLogic>().RemoveSpell(GetId());
		GameObject.Destroy(this.gameObject);
	}
	
	#endregion
	
	#region Spawnable implementation
	
	public virtual string GetName ()
	{
		return "";
	}

	public int GetId ()
	{
		return id;
	}
	
	public virtual object[] GatherInitData ()
	{
		return new object[] {};
	}
	
	public virtual void Init (int id, object[] initData, double spawnTime)
	{
		this.id = id;

		createTime = (float) spawnTime;
	}
	
	#endregion
	
	protected bool hasEnded = false;
	
	// Update is called once per frame
	void Update () {
		if ((float)PhotonNetwork.time > createTime + duration && !hasEnded) {
			this.End (this.player);
		}
	}
}
