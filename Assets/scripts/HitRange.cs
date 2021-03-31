using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRange : MonoBehaviour {

	public Transform player;
	// Update is called once per frame
	void Update () {
		if(player == null){
			Destroy(this);
		}
		else{
			transform.position = player.position;
		}	
	}
}
