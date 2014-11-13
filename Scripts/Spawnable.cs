using UnityEngine;
using System;

public interface Spawnable {
	/**
	 *	Get type name, as in class name, of the Spawnable 
	 * */
	string GetName();
	/**
	 * unique id
	 * */
	int GetId();
	/*
	 * this is a small serializable implementation
	 * collects all data for this object, sends it over network
	 * and passes it to the init funciton on the other side
	 * */
	object[] GatherInitData();
	/**
	 * deserialization method
	 * */
	void Init(int id, object[] initData, double spawnTime);
}
