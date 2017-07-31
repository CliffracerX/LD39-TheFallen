using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
	public float tick = 0.1f;
	public int damageDealt = 1;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag=="Damagable" && tick<=0)
		{
			other.gameObject.SendMessage("Damage", damageDealt);
			Destroy(this.gameObject);
		}
	}

	void Update()
	{
		tick-=Time.deltaTime;
		if(tick<=-2.5f)
		{
			Destroy(this.gameObject);
		}
	}
}