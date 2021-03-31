using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (Seeker))]
public class EnemyAi : MonoBehaviour {
	public Transform target;
	public float updateRate = 2f;
	private Seeker seeker;
	private Rigidbody2D rb;
	public Path path;
	public float speed = 300f;
	public ForceMode2D fMode;
	[HideInInspector]
	public bool pathIsEnded = false;
	public float nextWaypoinDistance = 3;
	private int currentWaypoint = 0;
	public bool playerInArea = false;
	public int health=100;
	public bool attacked = false;
	public LayerMask whatToHit;
	public Transform lineEnd;
	public Transform lineStart;
	public Vector3 dir;

	void Start()
	{
		seeker = GetComponent<Seeker> ();
		rb = GetComponent<Rigidbody2D> ();
		target = GameObject.FindGameObjectWithTag ("boi").transform;

		if (target == null) {
			Debug.LogError ("BOI WHERE THE PLAYER AT");
			return;
		}

		StartCoroutine (UpdatePath ());
	}
	public void DamageEnemy(int damage)
	{
		health -= damage;
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Hit Range")) {
			playerInArea = true;
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Hit Range")) {
			playerInArea = false;
			path = null;
		}
	}
	IEnumerator UpdatePath(){
		if (target == null) {
			//TODO: Insert a player search here.
			yield return false;
		}
		if(playerInArea)
			seeker.StartPath (transform.position, target.position, OnPathComplete);
		yield return new WaitForSeconds (1f / updateRate);
		StartCoroutine (UpdatePath ());
	}

	void OnPathComplete(Path p)
	{
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	IEnumerator Attack()
	{
		RaycastHit2D hit;
		hit = Physics2D.Raycast(this.transform.position, lineEnd.position, dir.x*2f, whatToHit);

		if (hit.collider != null) {
			Debug.Log("Made It");
			PlayerControl player = hit.collider.GetComponent<PlayerControl>();
			player.DamagePlayer (10);
			if (player.currentHealth <= 0)
				playerInArea = false;
		}
		Debug.Log ("Made It");
		yield return new WaitForSeconds (2.5f);
		attacked = false;
	}

	void Update()
	{
		if (health <= 0)
			GameManager.KillEnemy (this);
		if (!attacked) {
			Debug.Log (attacked);
			attacked = true;
			StartCoroutine (Attack ());
		}
	}
	void FixedUpdate()
	{
		if (target == null) {
			//TODO: Insert a player search here.
			return;
		}
		if (path == null)
			return;
		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded)
				return;
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		dir = new Vector3(((path.vectorPath [currentWaypoint] - transform.position).normalized).x,0,0);
		dir *= speed * Time.fixedDeltaTime;
		if (dir.x < 0)
			transform.eulerAngles = new Vector2(0, 0);
		else if(dir.x > 0)
			transform.eulerAngles = new Vector2 (0, 180);
		rb.AddForce (dir, fMode);

		float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if (dist < nextWaypoinDistance) {
			currentWaypoint++;
			return;
		}
	}
}
