using System;
using UnityEngine;

public interface PowerUp : Spawnable
{
	void Activate(GameObject player, Vector3 pos);
	void DeActivate(GameObject player);
}

