using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerControl : MonoBehaviour {

	public bool onLadder;
	private bool inLadder;
	public bool jumped = false;
	public bool grounded = false;
	public Transform groundedEnd;
	public Transform lineEnd;
	public float jumpForce;
	public float moveForce;
	public float maxVelocity;
	public Rigidbody2D rb;
	public GameObject ladder;
	public GameObject platform;
	public Animator anim;
	public bool attacked = false;
	public int health = 100;
	public int currentHealth;
	public LayerMask whatToHit;
	public Slider healthSlider;
	public int num;
	public GameObject deathBounds;
	public float flashSpeed = 5f;
	public Color flashColor = new Color (1f, 0f, 0f, 0.1f);
	public Image damageImage;
	public bool damaged = false;

	// Update is called once per frame
	void Start()
	{
		currentHealth = health;
		healthSlider.value = 100;
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D>();
		onLadder = false;
		inLadder = false;
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.F) && inLadder) {
			onLadder = !onLadder;
			Debug.Log ("OnLadder = " + onLadder);
			Debug.Log ("inLadder = " + inLadder);
			if(onLadder){
				transform.position = new Vector3 (ladder.transform.position.x, transform.position.y, transform.position.z);
				platform.GetComponent<Collider2D> ().enabled = false;
				deathBounds.GetComponent<Collider2D> ().enabled = false;
			}
			else {
				platform.GetComponent<Collider2D> ().enabled = true;
				deathBounds.GetComponent<Collider2D> ().enabled = true;
			}
		}
		if (onLadder) {
			rb.gravityScale = 0;
			rb.velocity = Vector2.zero;
		}
		else
			rb.gravityScale = 1;
		if (Input.GetKeyDown (KeyCode.Mouse0)&&!attacked) {
			attacked = true;
			StartCoroutine(Attack ());
		}
		if (currentHealth <= 0)
			GameManager.KillPlayer (this);
		if (damaged) {
			damageImage.color = flashColor;
		} else {
			damageImage.color = Color.Lerp(damageImage.color,Color.clear,flashSpeed*Time.deltaTime);
		}
		anim.SetFloat ("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
		Movement();
		damaged = false;
	}
	void FixedUpdate()
	{
		Raycasting ();
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("ladder")) {
			inLadder = true;
		} else if (other.gameObject.CompareTag ("deathBounds")) {
			DamagePlayer (9999999);
		} else if (other.gameObject.CompareTag ("heart")) {
			Debug.Log (currentHealth);
			currentHealth = 100;
			healthSlider.value = 100;
			Destroy (other.gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("ladder")) 
		{
			inLadder = false;
		}
	}
	IEnumerator Attack()
	{
		yield return new WaitForSeconds (.25f);
		rb.velocity = Vector3.zero;
		anim.SetTrigger("Attack");
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, lineEnd.position,num*2f, whatToHit);
		if (hit.collider != null) {
			EnemyAi enemy = hit.collider.GetComponent<EnemyAi>();
			enemy.DamageEnemy (25);
		}
		attacked = false;
			
	}
	public void DamagePlayer(int damage)
	{
		Debug.Log("Made It");
		currentHealth -= damage;
		Debug.Log(currentHealth);
		healthSlider.value = currentHealth;
		Debug.Log (healthSlider.value);
		damaged = true;
	}
	void Raycasting()
	{
		Debug.DrawLine(this.transform.position, groundedEnd.position, Color.green);
		grounded = Physics2D.Linecast(this.transform.position, groundedEnd.position, 1<< LayerMask.NameToLayer("Ground"));
		if (grounded)
			jumped = false;
	}
	void Movement()
	{
		if (Input.GetKey (KeyCode.D)&&!onLadder&&!attacked) {
			num = 1;
			if (Input.GetKey (KeyCode.LeftShift)) {
				if(Mathf.Abs(rb.velocity.x)<(maxVelocity*1.5f))
					rb.AddForce (Vector2.right.normalized * moveForce);
				if (Mathf.Abs (rb.velocity.x) > (maxVelocity*1.5f))
					rb.velocity =new Vector3(maxVelocity*1.5f, rb.velocity.y,0);
			} else {
				if(Mathf.Abs(rb.velocity.x)<maxVelocity)
					rb.AddForce (Vector2.right.normalized * moveForce);
				if (Mathf.Abs (rb.velocity.x) > (maxVelocity))
					rb.velocity =new Vector3(maxVelocity, rb.velocity.y,0);
			}
			transform.eulerAngles = new Vector2 (0, 0);
		}
		if (Input.GetKey (KeyCode.A)&&!onLadder&&!attacked) {
			num = -1;
			if (Input.GetKey (KeyCode.LeftShift)) {
				if(Mathf.Abs(rb.velocity.x)<(maxVelocity*1.5f))
					rb.AddForce (Vector2.left.normalized * moveForce);
				if (Mathf.Abs (rb.velocity.x) > (maxVelocity*1.5f))
					rb.velocity =new Vector3(-maxVelocity*1.5f, rb.velocity.y,0);
			} else {
				if(Mathf.Abs(rb.velocity.x)<maxVelocity)
					rb.AddForce (Vector2.left.normalized * moveForce);
				if (Mathf.Abs (rb.velocity.x) > (maxVelocity))
					rb.velocity =new Vector3(-maxVelocity, rb.velocity.y,0);
			}
			transform.eulerAngles = new Vector2 (0, 180);
		}

		if (Input.GetKeyDown (KeyCode.Space) && grounded && !onLadder) {
			rb.AddForce (new Vector3(0, jumpForce,0));
			jumped = true;
		}
		if (onLadder && Input.GetKey (KeyCode.W) && transform.position.y <= ladder.GetComponent<Collider2D>().bounds.size.y) {
			transform.Translate (Vector2.up * 4f * Time.deltaTime);
		}
		if (onLadder && Input.GetKey (KeyCode.S)) {
			transform.Translate (Vector2.down * 4f * Time.deltaTime);
		}
	}
}
