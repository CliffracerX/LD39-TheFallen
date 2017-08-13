using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingEntry
{
	public string name;
	public int cost;
	[TextArea]
	public string description;
	//public Texture2D icon;
	public Sprite sprite;
	public GameObject obj;
}

[System.Serializable]
public class MusicThing
{
	public AudioSource[] tracks;
	public float tick,speed;
	public int lastType,curType;

	public void StopAll()
	{
		foreach(AudioSource s in tracks)
		{
			s.volume=0;
		}
	}

	public void Update()
	{
		tick+=Time.deltaTime;
		tracks[lastType].volume = Mathf.Lerp(1, 0, tick/speed);
		tracks[curType].volume = Mathf.Lerp(0, 1, tick/speed);
	}

	public void Play(int track)
	{
		if(curType!=track)
		{
			lastType=curType;
			curType=track;
			tick=0;
		}
	}
}

public class Player : MonoBehaviour
{
	public float moveSpeed = 2.5f;
	public Animator anm;
	public GameObject bodyBase,armBase,armPistol;
	public bool pistolAcquired,pistolOut;
	public GameObject bulletObj;
	public int numShots = 1;
	public float spread = 0;
	public float bulletSpeed,fireRate,fireRateM;
	public int clip,maxClip,mags,maxMags;
	public bool reloadsAll = true;
	public bool fullAuto = false;
	public Texture2D clipIcon,emptyClipIcon,magIcon,emptyMagIcon;
	public int health,maxHealth,ore,maxOre,stam,maxStam;
	public Texture2D hpIcon,emptyHpIcon,oreIcon,emptyOreIcon,stamIcon,emptyStamIcon;
	public float sprintTime,lightTime,stamTime;
	public AudioSource fireSound;
	public Light flashlight;
	public bool flashlightOn;
	public LayerMask layMask;
	public PowerObject targ1,targ2;
	public GameObject wireObj;
	public BuildingEntry[] builds;
	public GameObject tempBuild;
	public bool buildMode;
	public int selectedBuild;
	public float buildChangeCool;
	public GUIStyle buildStyle;
	public static Player instance;
	public GameObject zamb;
	public Transform camera;
	public float shake;
	public MusicThing[] types;
	public AudioSource hurtNoise;
	public int healthPerRest,ammoPerRest;

	void Damage(int amount)
	{
		health-=amount;
		hurtNoise.Play();
		if(health<=0)
		{
			Destroy(this.gameObject);
			Instantiate(zamb, transform.position, transform.rotation);
		}
	}

	void Start()
	{
		tempBuild.transform.parent=null;
		Player.instance=this;
		foreach(MusicThing mt in types)
		{
			mt.StopAll();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag=="ResourceItem" && ore<maxOre)
		{
			ore+=1;
			Destroy(other.gameObject);
		}
		else if(other.tag=="Pistol")
		{
			pistolAcquired=true;
			pistolOut=true;
			Destroy(other.gameObject);
		}
	}

	void Update()
	{
		if(Input.GetButtonUp("Jump"))
		{
			pistolOut=!pistolOut;
			if(targ1)
				targ1.Unhighlight();
			targ1=null;
			if(targ2)
				targ2.Unhighlight();
			targ2=null;
		}
		buildChangeCool-=Time.deltaTime;
		if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
		{
			anm.SetBool("Moving", true);
			if(Input.GetButton("Sprint") && stam>0)
			{
				sprintTime+=Time.deltaTime;
				if(sprintTime>0.5f)
				{
					sprintTime=0;
					stam-=1;
				}
			}
		}
		else
		{
			anm.SetBool("Moving", false);
		}
		stamTime+=Time.deltaTime;
		if(stamTime>0.25f && stam<maxStam && !Input.GetButton("Sprint") && !flashlightOn)
		{
			stamTime=0;
			stam+=1;
		}
		if(Input.GetButtonUp("Flashlight"))
		{
			flashlightOn=!flashlightOn;
		}
		if(flashlightOn)
		{
			lightTime+=Time.deltaTime;
			if(lightTime>0.75f)
			{
				lightTime=0;
				stam-=1;
				if(stam<=0)
				{
					flashlightOn=false;
				}
			}
		}
		flashlight.enabled=flashlightOn;
		float smod = 1;
		if(stam>0 && Input.GetButton("Sprint"))
		{
			smod=2;
			anm.SetBool("Sprinting", true);
		}
		else
		{
			anm.SetBool("Sprinting", false);
		}
		transform.position+=new Vector3(Input.GetAxis("Horizontal")*moveSpeed*Time.deltaTime*smod, Input.GetAxis("Vertical")*moveSpeed*Time.deltaTime*smod);
		if(pistolAcquired && pistolOut)
		{
			armBase.SetActive(false);
			armPistol.SetActive(true);
		}
		else
		{
			armBase.SetActive(true);
			armPistol.SetActive(false);
		}
		fireRate-=Time.deltaTime;
		if(pistolAcquired && pistolOut)
		{
			if((fullAuto ? Input.GetButton("Fire1") : Input.GetButtonUp("Fire1")) && clip>0 && fireRate<=0)
			{
				//BANG
				clip-=1;
				fireSound.Play();
				for(int i = 0; i<numShots; i++)
				{
					//armPistol.transform.Rotate(0, 0, Random.Range(-spread, spread));
					GameObject bullet = (GameObject)Instantiate(bulletObj, armPistol.transform.position, armPistol.transform.rotation);
					//bullet.transform.Rotate(0, 0, Random.Range(-spread, spread));
					//bullet.transform.Rotate(0, 180, 0);
					bullet.GetComponent<Rigidbody2D>().AddForce((-bullet.transform.right+new Vector3(0, Random.Range(-spread, spread), 0))*bulletSpeed);
					camera.localPosition += new Vector3(Random.Range(-0.125f, 0.125f), Random.Range(-0.125f, 0.125f));
				}
				fireRate=fireRateM;
				shake=0;
			}
			if(Input.GetButtonUp("Reload") && mags>0 && clip<maxClip)
			{
				//CLICK
				mags-=1;
				if(reloadsAll)
					clip=maxClip;
				else
					clip+=1;
			}
		}
		shake+=Time.deltaTime;
		camera.localPosition = Vector3.Lerp(camera.localPosition, new Vector3(0, 0, -2.375f), shake);
		if(Input.GetButtonUp("Reload") && !pistolOut)
		{
			buildMode=!buildMode;
			if(targ1)
				targ1.Unhighlight();
			targ1=null;
			if(targ2)
				targ2.Unhighlight();
			targ2=null;
		}
		Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
		r.origin = new Vector3(r.origin.x, r.origin.y, 0);
		tempBuild.SetActive(buildMode);
		tempBuild.transform.position = new Vector3((int)r.origin.x, (int)r.origin.y, 0);
		if(Input.GetButtonUp("Fire1") && !pistolOut)
		{
			if(!buildMode)
			{
				RaycastHit2D rh = Physics2D.Raycast(r.origin, Vector3.up, 500f, layMask);
				if(rh)
				{
					Debug.Log(rh.point);
					PowerObject tempTarg = rh.collider.GetComponent<PowerObject>();
					if(tempTarg.type=="Generator")
					{
						Generator g = (Generator)tempTarg;
						if(ore>0)
						{
							g.fuel+=10;
							ore-=1;
							if(g.fuel>g.fuelMax)
								g.fuel=g.fuelMax;
						}
					}
					else
					{
						tempTarg.toggle = !tempTarg.toggle;
					}
				}
			}
			else
			{
				if(ore>=builds[selectedBuild].cost)
				{
					ore-=builds[selectedBuild].cost;
					Instantiate(builds[selectedBuild].obj, tempBuild.transform.position, tempBuild.transform.rotation);
				}
			}
		}
		if(Input.GetAxis("Mouse ScrollWheel")>0)
		{
			if(buildChangeCool<=0)
			{
				buildChangeCool=0.1f;
				selectedBuild+=1;
				if(selectedBuild<=0)
					selectedBuild=builds.Length-1;
				else if(selectedBuild>=builds.Length)
					selectedBuild=0;
				tempBuild.GetComponent<SpriteRenderer>().sprite=builds[selectedBuild].sprite;
			}
		}
		if(Input.GetAxis("Mouse ScrollWheel")<0)
		{
			if(buildChangeCool<=0)
			{
				buildChangeCool=0.1f;
				selectedBuild-=1;
				if(selectedBuild<0)
					selectedBuild=builds.Length-1;
				else if(selectedBuild>=builds.Length)
					selectedBuild=0;
				tempBuild.GetComponent<SpriteRenderer>().sprite=builds[selectedBuild].sprite;
			}
		}
		if(Input.GetButtonUp("Fire2") && !pistolOut)
		{
			RaycastHit2D rh = Physics2D.Raycast(r.origin, Vector3.up, 500f, layMask);
			if(rh)
			{
				if(!targ1 && !targ2)
				{
					targ1 = rh.collider.GetComponent<PowerObject>();
					targ1.Highlight();
				}
				else if (!targ2 && targ1)
				{
					targ2 = rh.collider.GetComponent<PowerObject>();
					targ2.Highlight();
				}
				else if(targ1 && targ2)
				{
					if(targ1==targ2)
					{
						targ1.ClearWires();
						targ1.Unhighlight();
						targ1=null;
						targ2=null;
					}
					else
					{
						PowerObject tempTarg = rh.collider.GetComponent<PowerObject>();
						if(tempTarg==targ1 || tempTarg==targ2)
						{
							targ1.Unhighlight();
							targ2.Unhighlight();
							GameObject go = (GameObject)Instantiate(wireObj, Vector3.zero, Quaternion.identity);
							Wire w = go.GetComponent<Wire>();
							w.targ1=targ1;
							w.targ2=targ2;
							targ1.wires.Add(w);
							targ2.wires.Add(w);
							targ1=null;
							targ2=null;
						}
						else
						{
							if(targ1)
							{
								targ1.Unhighlight();
								targ1=null;
							}
							if(targ2)
							{
								targ2.Unhighlight();
								targ1=null;
							}
						}
					}
				}
				else
				{

				}
			}
			else
			{
				if(targ1)
				{
					targ1.Unhighlight();
					targ1=null;
				}
				if(targ2)
				{
					targ2.Unhighlight();
					targ1=null;
				}
			}
		}
		Vector3 oPos = r.origin;
		oPos.z=0;
		armPistol.transform.LookAt(oPos, armPistol.transform.up);
		armPistol.transform.Rotate(0f, 90f, 0f);
		Quaternion q = armPistol.transform.rotation;
		q.eulerAngles = new Vector3(0, 0, q.eulerAngles.z);
		armPistol.transform.rotation=q;
		if((Input.GetAxis("Horizontal")<0 && !pistolOut) || (oPos.x<this.transform.position.x && pistolAcquired && pistolOut))
		{
			bodyBase.transform.localScale = armBase.transform.localScale = new Vector3(1, 1, 1);
			armPistol.transform.localScale = new Vector3(1, 1, 1);
		}
		if((Input.GetAxis("Horizontal")>0 && !pistolOut) || (oPos.x>this.transform.position.x && pistolAcquired && pistolOut))
		{
			bodyBase.transform.localScale = armBase.transform.localScale = new Vector3(-1, 1, 1);
			//armPistol.transform.Rotate(0f, 0f, 180f);
			Quaternion qT = armPistol.transform.rotation;
			//qT.eulerAngles = new Vector3(0, 0, qT.eulerAngles.z-180f);
			//armPistol.transform.rotation = qT;
			armPistol.transform.localScale = new Vector3(1, -1, 1);
		}
		int musState = 0;
		int numZombs = 0,numBuilds = 0;
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15);
		foreach(Collider2D c in cols)
		{
			if(c.GetComponent<ZambieScript>())
			{
				numZombs+=1;
			}
			else if(!c.GetComponent<ZambieScript>() && c.tag=="Damagable")
			{
				numBuilds+=1;
			}
		}
		foreach(MusicThing mt in types)
		{
			mt.StopAll();
			mt.Update();
		}
		if(health>maxHealth/25 && mags>maxMags/5)
		{
			types[0].Play(curTrack);
		}
		else
		{
			types[0].Play(0);
		}
		if(numZombs>10)
		{
			types[1].Play(curTrack);
		}
		else if(numZombs<5 && types[1].tick>=1)
		{
			types[1].Play(0);
		}
		if(numBuilds>25)
		{
			types[2].Play(curTrack);
		}
		else
		{
			types[2].Play(0);
		}
		if(pistolOut)
		{
			types[3].Play(curTrack);
		}
		else
		{
			types[3].Play(0);
		}
	}

	public int curTrack = 0;

	Rect ScalePos(int x, int y, int w, int h, int amount)
	{
		return new Rect(x*amount, y*amount, w*amount, h*amount);
	}

	void OnGUI()
	{
		GUI.color = new Color(0.75f, 0.75f, 0.75f);
		int ymT = 0;
		for(int i = 0; i<maxHealth; i++)
		{
			int xm = i;
			int ym = ymT;
			ym += hpIcon.height;
			while(xm>=100f/hpIcon.width)
			{
				xm-=100/hpIcon.width;
				ym += hpIcon.height;
			}
			if(i==maxHealth-1)
			{
				//ym-=hpIcon.height;
				ymT+=(ym-ymT);
			}
			if(health>i)
			{
				GUI.DrawTexture(ScalePos(5+(hpIcon.width*xm), ym, hpIcon.width, hpIcon.height, 4), hpIcon);
			}
			else
			{
				GUI.DrawTexture(ScalePos(5+(hpIcon.width*xm), ym, hpIcon.width, hpIcon.height, 4), emptyHpIcon);
			}
		}
		for(int i = 0; i<maxStam; i++)
		{
			if(stam>i)
			{
				GUI.DrawTexture(ScalePos(((Screen.width/2)-(8*maxStam))+(16*i), Screen.height-50, 16, 20, 1), stamIcon);
			}
			else
			{
				GUI.DrawTexture(ScalePos(((Screen.width/2)-(8*maxStam))+(16*i), Screen.height-50, 16, 20, 1), emptyStamIcon);
			}
		}
		if(pistolAcquired)
		{
			for(int i = 0; i<maxMags; i++)
			{
				int xm = i;
				int ym = ymT;
				ym += hpIcon.height;
				while(xm>=100f/magIcon.width)
				{
					xm-=100/magIcon.width;
					ym += magIcon.height;
				}
				if(i==maxMags-1)
				{
					//ym-=magIcon.height;
					ymT+=(ym-ymT);
				}
				if(mags>i)
				{
					GUI.DrawTexture(ScalePos(5+(magIcon.width*xm), ym, magIcon.width, magIcon.height, 4), magIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(magIcon.width*xm), ym, magIcon.width, magIcon.height, 4), emptyMagIcon);
				}
			}
			for(int i = 0; i<maxClip; i++)
			{
				int xm = i;
				int ym = ymT;
				ym += magIcon.height;
				while(xm>=100f/clipIcon.width)
				{
					xm-=100/clipIcon.width;
					ym += clipIcon.height;
				}
				if(i==maxClip-1)
				{
					//ym-=clipIcon.height;
					ymT+=(ym-ymT);
				}
				if(clip>i)
				{
					GUI.DrawTexture(ScalePos(5+(clipIcon.width*xm), ym, clipIcon.width, clipIcon.height, 4), clipIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(clipIcon.width*xm), ym, clipIcon.width, clipIcon.height, 4), emptyClipIcon);
				}
			}
			for(int i = 0; i<maxOre; i++)
			{
				int xm = i;
				int ym = ymT;
				ym += clipIcon.height;
				while(xm>=100f/oreIcon.width)
				{
					xm-=100/oreIcon.width;
					ym += oreIcon.height;
				}
				if(i==maxOre-1)
				{
					//ym-=oreIcon.height;
					ymT+=(ym-ymT);
				}
				if(ore>i)
				{
					GUI.DrawTexture(ScalePos(5+(oreIcon.width*xm), ym, oreIcon.width, oreIcon.height, 4), oreIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(oreIcon.width*xm), ym, oreIcon.width, oreIcon.height, 4), emptyOreIcon);
				}
			}
		}
		else
		{
			for(int i = 0; i<maxOre; i++)
			{
				int xm = i;
				int ym = ymT;
				ym += hpIcon.height;
				while(xm>=100f/oreIcon.width)
				{
					xm-=100/oreIcon.width;
					ym += oreIcon.height;
				}
				if(i==maxOre-1)
				{
					//ym-=oreIcon.height;
					ymT+=(ym-ymT);
				}
				if(ore>i)
				{
					GUI.DrawTexture(ScalePos(5+(oreIcon.width*xm), ym, oreIcon.width, oreIcon.height, 4), oreIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(oreIcon.width*xm), ym, oreIcon.width, oreIcon.height, 4), emptyOreIcon);
				}
			}
		}
		if(buildMode)
		{
			GUI.Label(new Rect(Screen.width-500, Screen.height/2-100, 475, 200), builds[selectedBuild].name+": "+builds[selectedBuild].cost+" ore\n"+builds[selectedBuild].description, buildStyle);
		}
		GUI.color=Color.white;
	}
}