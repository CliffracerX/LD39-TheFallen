using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerObject : MonoBehaviour
{
	public bool toggle = true;
	public bool takesWhenOff = false;
	public List<Wire> wires;
	public float health,maxhealth;
	public string type;
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

	public void ClearWires()
	{
		foreach(Wire w in wires)
		{
			Destroy(w.gameObject);
		}
		wires.Clear();
	}

	public virtual void Highlight()
	{
		
	}

	public virtual void Unhighlight()
	{

	}

	public virtual void Destroy()
	{
		foreach(Wire w in wires)
		{
			if(w!=null)
			{
				Destroy(w.gameObject);
			}
		}
		Destroy(this.gameObject);
		GameObject sound = (GameObject)Instantiate(tempSound, transform.position, transform.rotation);
		AudioSource oAS = sound.GetComponent<AudioSource>();
		oAS.clip=destSound;
		oAS.minDistance=destRange/20;
		oAS.maxDistance=destRange;
		oAS.Play();
	}

	public virtual bool UsePower(float amount)
	{
		return false;
	}

	public virtual bool HasPower(float amount)
	{
		return false;
	}

	public virtual bool GetPower(float amount)
	{
		return false;
	}

	public virtual float GetPowerUse()
	{
		return 0f;
	}

	public virtual float GetPowerOut()
	{
		return 0f;
	}
}
