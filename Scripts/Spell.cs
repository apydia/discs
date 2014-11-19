using UnityEngine;
using System;

public interface Spell : Spawnable
{
	void SetReceivingPlayerID(int id);
	void SetCastingPlayerID(int id);
	void Begin (GameObject player);
	void Break (GameObject player);
	void End (GameObject player);
}