using UnityEngine;
using System;

public interface Spawnable {
	string GetName();
	
	object[] GatherInitData();
	void Init(int id, object[] initData);
}
