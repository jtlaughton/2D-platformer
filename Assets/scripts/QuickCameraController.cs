using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickCameraController : MonoBehaviour {
	public float speed;
	public GameObject player;
	public PlayerControl playScript;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (player.transform.position.x, 0, -10);
		player = GameObject.FindGameObjectWithTag ("boi");
		playScript = GameObject.FindGameObjectWithTag ("boi").GetComponent<PlayerControl>();
	}

	void Update()
	{
		if (player == null)
			return;
		if(playScript.onLadder)
			transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, -10);
		else if(transform.position.y > 0 && transform.position.y < 33f)
			transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, -10);
		else
			transform.position = new Vector3 (player.transform.position.x, transform.position.y, -10);
	}
}
