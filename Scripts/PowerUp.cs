using System;
using UnityEngine;

public interface PowerUp : Spawnable
{
	int GetId();
	void Activate(GameObject player, Vector3 pos);
}

