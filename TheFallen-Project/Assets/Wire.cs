using UnityEngine;
using System.Collections;

public class Wire : MonoBehaviour
{
	public PowerObject targ1,targ2;
	public LineRenderer ln;

	void Update()
	{
		ln.SetPosition(0, targ1.transform.position);
		ln.SetPosition(1, targ2.transform.position);
		float dt = Time.deltaTime;
		if((targ1.toggle && targ2.toggle) || (targ1.toggle && targ2.takesWhenOff))
		{
			if(targ1.HasPower(targ2.GetPowerUse()*dt))
			{
				targ2.GetPower(targ1.GetPowerOut()*dt);
				targ1.UsePower(targ2.GetPowerUse()*dt);
			}
		}
	}
}