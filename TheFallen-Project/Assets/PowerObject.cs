using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerObject : MonoBehaviour
{
	public bool toggle = true;
	public List<Wire> wires;
	public float health,maxhealth;
	public string type;

	void Damage(int amount)
	{
		health-=amount;
		if(health<=0)
		{
			this.Destroy();
		}
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
