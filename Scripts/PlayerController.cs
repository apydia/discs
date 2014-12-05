using UnityEngine;
using System.Collections;

public class PlayerController : Photon.MonoBehaviour {

	public float speed = 0.2f;

	public Color color;
	public GameObject proj;
	public int playerID;

	public int gunAmmoAmount = 20;

	public bool defaultGunEnabled = false;

	void Start () {
		Physics.gravity = new Vector3(0, -120.0f, 0);
		PhotonNetwork.sendRate = 40;
		PhotonNetwork.sendRateOnSerialize = 20;

		Vector3 c = (Vector3)photonView.instantiationData [0];
		playerID = (int)photonView.instantiationData [1];

		color = new Color (c.x, c.y, c.z);
	}

	void CreateProjectile(Vector3 destination, double startTime) {
		Vector3 pos = rigidbody.position;
		pos.y = 3;
		GameObject projectile = (GameObject) Instantiate (proj, pos, Quaternion.identity); 
		projectile.GetComponent<Projectile>().origin = pos; 
		projectile.GetComponent<Projectile>().destination = destination; 
		projectile.GetComponent<Projectile>().createTime = startTime; 
		projectile.GetComponent<Projectile>().playerID = playerID; 
	}

	[RPC]
	void SpawnProjectile(Vector3 destination, PhotonMessageInfo info) {
		double timestamp = PhotonNetwork.time;
		
		if (info != null) {
			timestamp = info.timestamp;
		}
		CreateProjectile (destination, timestamp);
	}

	Vector3 ProjectMousePosition(Vector3 mousePos) {
		Ray ray = Camera.main.ScreenPointToRay (mousePos);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 2));
		return hit.point;
	}

	void Update() {
		GameMain main = GameObject.Find ("Scripts").GetComponent<GameMain>();

		if (Input.GetButtonDown ("Fire1") && defaultGunEnabled) {
			if (gunAmmoAmount > 0) {
				GameObject sp = GameObject.Find ("SoundPlayer");
				if (sp != null) {
					SoundPlayer soundPlayer = sp.GetComponent<SoundPlayer>();
					if (soundPlayer != null) {
						soundPlayer.Play(GameSound.DEFAULT_GUN);
					}
				}
				Vector3 hitPoint = main.mouseVec;
				photonView.RPC ("SpawnProjectile", PhotonTargets.All, hitPoint);
				--gunAmmoAmount;
				gameObject.GetComponent<PlayerLogic>().UpdatePowerUpHUD();
			}
		}
		if (Input.GetButtonDown ("Fire2")) {
			Vector3 hitPoint = main.mouseVec;
			PowerUp pUp = gameObject.GetComponent<PlayerLogic>().UseSelectedPowerUp();
			if (pUp != null) {
				pUp.Activate(gameObject, hitPoint);
			}
		}

		float upDown = Input.GetAxis("Mouse ScrollWheel");

		if (Input.GetKeyDown (KeyCode.E) || Input.GetKeyDown (KeyCode.Tab)) {
			gameObject.GetComponent<PlayerLogic>().SelectNextPowerUp();
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			gameObject.GetComponent<PlayerLogic>().SelectPrevPowerUp();
		}

		if (upDown < 0) {
			gameObject.GetComponent<PlayerLogic>().SelectPrevPowerUp();
		} else if (upDown > 0){
			gameObject.GetComponent<PlayerLogic>().SelectNextPowerUp();
		}

	}

	public string clipName = "Idle";
	public Vector3 mov;

	void FixedUpdate () {
		float moveHor = Input.GetAxis ("Horizontal");
		float moveVer = Input.GetAxis ("Vertical");

		mov = new Vector3 (-moveVer, 0.0f, moveHor);
		mov = mov * speed;

		transform.Translate (mov, Space.World);//AddForce (mov * 20);
		if (mov.magnitude > 0.05f) {
			rigidbody.transform.rotation = Quaternion.LookRotation(mov);//Quaternion.LookRotation(mov);
			animation["Walk"].speed = 1.5f;
			gameObject.GetComponent<Animation>().Play ("Walk");
			clipName = "Walk";
		} else {
			gameObject.GetComponent<Animation>().Play ("Idle");
			clipName = "Idle";
		}
	}
}