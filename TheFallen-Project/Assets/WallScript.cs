using UnityEngine;
using System.Collections;

public class WallScript : PowerObject
{
	public float euU;
	public Sprite onSprite,offSprite;
	public SpriteRenderer sr;
	public Collider2D col;
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
		col.isTrigger=true;
		sr.sprite=offSprite;
		if(powered)
		{
			col.isTrigger=false;
			sr.sprite=onSprite;
			powered=false;
		}
	}
}