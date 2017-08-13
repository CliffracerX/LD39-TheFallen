using UnityEngine;
using System.Collections;

public class TurretScript : MonoBehaviour
{
	public BatteryScript bat;
	public float range,spread,bulletSpeed,powerRate,shotPower,fireRate,fr,shake;
	public GameObject bulletObj;
	public AudioSource fireSound;
	public Transform trans;
	public int numShots;
	public Collider2D thisCol;
	public LayerMask layMask;

	void Update()
	{
		if(bat.toggle)
		{
			if(bat.UsePower(powerRate*Time.deltaTime))
			{
				Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
				float farthestZombDist = range;
				GameObject zomb = null;
				foreach(Collider2D c in cols)
				{
					if(c.GetComponent<ZambieScript>())
					{
						float td = Vector3.Distance(transform.position, c.transform.position);
						if(td<farthestZombDist)
						{
							farthestZombDist=td;
							zomb = c.gameObject;
						}
					}
				}
				fr-=Time.deltaTime;
				if(zomb)
				{
					trans.LookAt(zomb.transform.position, trans.up);
					trans.Rotate(0f, 90f, 0f);
					Quaternion q = trans.rotation;
					q.eulerAngles = new Vector3(0, 0, q.eulerAngles.z);
					trans.rotation=q;
				}
				if(fr<=0 && zomb)
				{
					fr=fireRate;
					bool clear = true;
					Vector3 norm = (transform.position-zomb.transform.position);
					norm.Normalize();
					thisCol.enabled=false;
					RaycastHit2D rh = Physics2D.Raycast(transform.position, -trans.right, range, layMask);
					print(rh.collider.name);
					if(rh.collider.gameObject!=zomb)
						clear = false;
					if(clear)
					{
						if(bat.UsePower(shotPower))
						{
							fireSound.Play();
							for(int i = 0; i<numShots; i++)
							{
								//armPistol.transform.Rotate(0, 0, Random.Range(-spread, spread));
								GameObject bullet = (GameObject)Instantiate(bulletObj, trans.position, trans.rotation);
								//bullet.transform.Rotate(0, 0, Random.Range(-spread, spread));
								//bullet.transform.Rotate(0, 180, 0);
								bullet.GetComponent<Rigidbody2D>().AddForce((-bullet.transform.right+new Vector3(0, Random.Range(-spread, spread), 0))*bulletSpeed);
								if(Vector3.Distance(transform.position, Player.instance.transform.position)<=range)
									Camera.main.transform.localPosition += new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake));
							}
							if(Vector3.Distance(transform.position, Player.instance.transform.position)<=range)
								Player.instance.shake=0;
						}
					}
					thisCol.enabled=true;
				}
			}
		}
	}
}