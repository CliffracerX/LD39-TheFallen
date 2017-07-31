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

	void Damage(int amount)
	{
		health-=amount;
		if(health<=0)
		{
			Destroy(this.gameObject);
			for(int i = 0; i<Random.Range(minDrop, maxDrop); i++)
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
		}
	}

	void Update()
	{
		Vector3 normal = (transform.position - Player.instance.transform.position);
		normal.Normalize();
		rb.AddForce(normal*speed);
		cooldown-=Time.deltaTime;
		//if(cooldown<=0)
		{

			/*if(Vector3.Distance(transform.position, Player.instance.transform.position)<range)
			{
				GameObject proj = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
				Rigidbody2D rbt = proj.GetComponent<Rigidbody2D>();
				rbt.AddForce(normal*projSpeed);
				anm.SetTrigger("Attack");
			}*/
		}
	}
}