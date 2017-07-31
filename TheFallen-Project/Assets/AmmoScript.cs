using UnityEngine;
using System.Collections;

public class AmmoScript : PowerObject
{
	public float euU,healRange,healTick,healSpeed;
	public int healAmount;
	public Sprite onSprite,offSprite;
	public SpriteRenderer sr;
	public Light light;
	public bool powered = false;
	
	public override void Highlight()
	{
		sr.color = new Color(0.5f, 1.0f, 0.0f);
	}
	
	public override void Unhighlight()
	{
		sr.color = new Color(1.0f, 1.0f, 1.0f);
	}
	
	public override bool GetPower(float amount)
	{
		powered = true;
		return true;
	}
	
	public override float GetPowerUse()
	{
		return euU;
	}
	
	void Update()
	{
		light.enabled=false;
		sr.sprite=offSprite;
		if(powered)
		{
			healTick+=Time.deltaTime;
			if(healTick>=healSpeed)
			{
				healTick=0;
				if(Vector3.Distance(transform.position, Player.instance.transform.position)<=healRange)
				{
					Player.instance.mags+=healAmount;
					if(Player.instance.mags>Player.instance.maxMags)
						Player.instance.mags=Player.instance.maxMags;
				}
			}
			light.enabled=true;
			sr.sprite=onSprite;
			powered=false;
		}
	}
}