using UnityEngine;
using System.Collections;

public class ComingFromGame : MonoBehaviour {

	public EMenuState switchToThisState;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);

		switchToThisState = EMenuState.MainMenu;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
