using UnityEngine;
using System.Collections;

public class ResourceCrystal : MonoBehaviour
{
	public int health,maxHealth;
	public GameObject dropObj;
	public float doRange,doSpeed;

	void Damage(int amount)
	{
		health-=amount;
		if(health<=0)
		{
			Destroy(this.gameObject);
			for(int i = 0; i<5; i++)
			{
				GameObject go = (GameObject)Instantiate(dropObj, transform.position+new Vector3(Random.Range(-doRange, doRange), Random.Range(-doRange, doRange)), Quaternion.identity);
				if(go.GetComponent<Rigidbody2D>())
				{
					go.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-doSpeed, doSpeed), Random.Range(-doSpeed, doSpeed)));
				}
			}
		}
	}
}