using UnityEngine;
using System.Collections;

public class GroundTrap : MonoBehaviour
{
	public enum TrapType {DoT=0, Explode=1}
	public TrapType thisType;
	public int dmg;
	public float range,speed,s;
	public float health,maxhealth;
	public GameObject tempSound;
	public AudioClip dmgSound,destSound;
	public float dmgRange = 10,destRange=20;

	void Damage(int amount)
	{
		health-=amount;
		GameObject sound = (GameObject)Instantiate(tempSound, transform.position, transform.rotation);
		AudioSource oAS = sound.GetComponent<AudioSource>();
		oAS.clip=dmgSound;
		oAS.minDistance=dmgRange/20;
		oAS.maxDistance=dmgRange;
		oAS.Play();
		if(health<=0)
		{
			this.Destroy();
		}
		if(health>this.maxhealth)
		{
			health=this.maxhealth;
		}
	}

	public void Destroy()
	{
		Destroy(this.gameObject);
		GameObject sound = (GameObject)Instantiate(tempSound, transform.position, transform.rotation);
		AudioSource oAS = sound.GetComponent<AudioSource>();
		oAS.clip=destSound;
		oAS.minDistance=destRange/20;
		oAS.maxDistance=destRange;
		oAS.Play();
		if(thisType==TrapType.Explode)
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
			foreach(Collider2D c in cols)
			{
				if(c.tag=="Damagable")
				{
					float td = Vector3.Distance(transform.position, c.transform.position);
					int ddmg = (int)(dmg*(1-(td/range)));
					c.SendMessage("Damage", ddmg);
				}
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(thisType==TrapType.DoT)
		{
			if(other.tag=="Damagable")
			{
				s-=Time.deltaTime;
				if(s<=0)
				{
					s=speed;
					other.SendMessage("Damage", dmg);
				}
			}
		}
		else if(thisType==TrapType.Explode)
		{
			if(other.tag=="Damagable")
			{
				s-=Time.deltaTime;
				if(s<=0)
				{
					Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
					foreach(Collider2D c in cols)
					{
						if(c.tag=="Damagable")
						{
							float td = Vector3.Distance(transform.position, c.transform.position);
							int ddmg = (int)(dmg*(1-(td/range)));
							c.SendMessage("Damage", ddmg);
						}
					}
					this.health=0;
				}
			}
		}
	}
}