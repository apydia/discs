using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundPlayer : MonoBehaviour {

	public GameObject countDownBeep;
	public GameObject explosion1;
	public GameObject waterDrip1;
	public GameObject scores1;
	public GameObject diamond1;
	public GameObject teletrans;
	public GameObject badMagic;
	public GameObject regeneration;
	public GameObject fallDown;
	public GameObject shieldUp;
	public GameObject shieldDown;
	public GameObject ammoPickup;
	public GameObject defaultGun;
	public GameObject rayGun;

	public List<GameObject> sounds;

	// Use this for initialization
	void Start () {
		sounds = new List<GameObject>();
	}

	public AudioSource Play(GameSound sound) {
		GameObject s;
		if (!PhotonNetwork.isMasterClient) {
			return null; // TODO: this has got to go!!!
		}
		switch(sound) {
		case GameSound.COUNTDOWN_BEEP:
			s = (GameObject)Instantiate (countDownBeep, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.EXPLOSION_1:
			s = (GameObject)Instantiate (explosion1, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.WATER_DRIP_1:
			s = (GameObject)Instantiate (waterDrip1, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.SCORES_1:
			s = (GameObject)Instantiate (scores1, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.PICKUP_DIAMOND_1:
			s = (GameObject)Instantiate (diamond1, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.TELETRANSPORTO:
			s = (GameObject)Instantiate (teletrans, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.BAD_MAGIC:
			s = (GameObject)Instantiate (badMagic, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.REGENERATION:
			s = (GameObject)Instantiate (regeneration, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.FALL_DOWN:
			s = (GameObject)Instantiate (fallDown, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.SHIELD_UP:
			s = (GameObject)Instantiate (shieldUp, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.SHIELD_DOWN:
			s = (GameObject)Instantiate (shieldDown, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.AMMO_PICK_UP:
			s = (GameObject)Instantiate (ammoPickup, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.DEFAULT_GUN:
			s = (GameObject)Instantiate (defaultGun, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		case GameSound.RAY_GUN:
			s = (GameObject)Instantiate (rayGun, new Vector3(100f, 125f, 0f), Quaternion.identity);
			return s.GetComponent<AudioSource>();
		default:
			return null;
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		GameObject[] soundArr = sounds.ToArray();
		List<GameObject> del = new List<GameObject>();
		for (int i = 0; i < soundArr.Length; i++) {
			AudioSource source = soundArr[i].GetComponent<AudioSource>();
			if (source != null) {
				if (!source.isPlaying) {
					del.Add (soundArr[i]);
				}
			}
		}
		foreach(GameObject o in del) {
			sounds.Remove (o);
			GameObject.Destroy(o);
		}
	}
}

public enum GameSound {
	COUNTDOWN_BEEP, EXPLOSION_1, WATER_DRIP_1, SCORES_1, PICKUP_DIAMOND_1, TELETRANSPORTO,
	BAD_MAGIC, REGENERATION, FALL_DOWN, SHIELD_UP, SHIELD_DOWN, AMMO_PICK_UP, DEFAULT_GUN,
	RAY_GUN
}
