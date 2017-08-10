using UnityEngine;
using System.Collections;

public class ZambieScript : MonoBehaviour
{
	public int health,maxHealth,damageDealt;
	public GameObject projectile,dropObj;
	public float projSpeed,doRange,doSpeed;
	public int minDrop,maxDrop;
	public float cooldown,coolMin,coolMax,range;
	public Rigidbody2D rb;
	public float speed;
	public Animator anm;
	public float[] resModFromDif;
	public float[] healthModFromDif;
	public AudioSource hurtNoise;

	void Start()
	{
		this.minDrop = (int)(this.minDrop*resModFromDif[MainMenu.curDif]);
		this.maxDrop = (int)(this.maxDrop*resModFromDif[MainMenu.curDif]);
		this.maxHealth = (int)(this.maxHealth*healthModFromDif[MainMenu.curDif]);
	}

	void Damage(int amount)
	{
		health-=amount;
		hurtNoise.Play();
		if(health<=0)
		{
			Destroy(this.gameObject);
			for(int i = 0; i<minDrop+Random.Range(0, maxDrop); i++)
			{
				GameObject go = (GameObject)Instantiate(dropObj, transform.position+new Vector3(Random.Range(-doRange, doRange), Random.Range(-doRange, doRange)), Quaternion.identity);
				if(go.GetComponent<Rigidbody2D>())
				{
					go.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-doSpeed, doSpeed), Random.Range(-doSpeed, doSpeed)));
				}
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.tag=="Damagable" && other.gameObject.name.StartsWith("Zambie")==false && cooldown<=0)
		{
			cooldown=Random.Range(coolMin, coolMax);
			other.gameObject.SendMessage("Damage", damageDealt);
			anm.SetTrigger("Attack");
		}
	}

	void Update()
	{
		if(Player.instance)
		{
			Vector3 normal = (transform.position - Player.instance.transform.position);
			normal.Normalize();
			rb.AddForce(normal*speed);
			cooldown-=Time.deltaTime;
			if(transform.position.x - Player.instance.transform.position.x < 0)
			{
				transform.localScale = new Vector3(-transform.localScale.z, transform.localScale.z, transform.localScale.z);
			}
			else
			{
				transform.localScale = new Vector3(transform.localScale.z, transform.localScale.z, transform.localScale.z);
			}
			if(cooldown<=0 && projectile!=null)
			{
				if(Vector3.Distance(transform.position, Player.instance.transform.position)<range)
				{
					GameObject proj = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
					Rigidbody2D rbt = proj.GetComponent<Rigidbody2D>();
					rbt.AddForce(normal*projSpeed);
					anm.SetTrigger("Attack");
				}
			}
		}
	}
}