using UnityEngine;
using System.Collections;

public class Generator : PowerObject
{
	public float euT,euTMax;
	public float fuel,fuelMax;
	public Sprite[] sprites;
	public Sprite[] emptySprites;
	public float[] lightIntens;
	public int tempTick;
	public float blinkTick,blinkTickSpeed;
	public SpriteRenderer sr;
	public Light flashLight;

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
		if(euT<=euTMax)
		{
			euT+=amount;
			return true;
		}
		else return false;
	}
	
	public override bool HasPower(float amount)
	{
		if(euT>=0)
		{
			return true;
		}
		else return false;
	}
	
	public override bool UsePower(float amount)
	{
		if(euT>=0)
		{
			euT-=amount;
			if(euT<0)
				euT=0;
			return true;
		}
		else return false;
	}
	
	public override float GetPowerUse()
	{
		return 0f;
	}

	public override float GetPowerOut()
	{
		return euT;
	}

	public int thing;

	void Update()
	{
		if(toggle)
		{
			if(fuel>0)
			{
				euT=euTMax*Time.deltaTime;
				fuel-=Time.deltaTime;
				float euM = fuel/fuelMax;
				thing = (int)(euM*sprites.Length);
				if(thing>=sprites.Length)
					thing=sprites.Length-1;
				flashLight.intensity=0;
				sr.sprite=sprites[thing];
			}
			else
			{
				euT=0;
				fuel=0;
				blinkTick-=Time.deltaTime;
				if(blinkTick<0)
				{
					blinkTick=blinkTickSpeed;
					tempTick+=1;
					if(tempTick>=emptySprites.Length)
						tempTick=0;
				}
				sr.sprite=emptySprites[tempTick];
				flashLight.intensity=lightIntens[tempTick];
			}
		}
	}
}