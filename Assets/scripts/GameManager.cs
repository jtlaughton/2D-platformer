using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager gm;
	public QuickCameraController main;

	void Start()
	{
		main = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<QuickCameraController>();
		if (gm == null)
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameManager>();
	}

	public Transform playerPrefab;
	public Transform spawnPoint;
	public float spawnDelay = 2;

	public static void KillPlayer(PlayerControl player)
	{
		Destroy (player.gameObject);
	}
	public static void KillEnemy(EnemyAi enemy)
	{
		Destroy (enemy.gameObject);
	}
}
