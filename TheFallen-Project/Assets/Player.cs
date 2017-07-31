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

public class Player : MonoBehaviour
{
	public float moveSpeed = 2.5f;
	public Animator anm;
	public GameObject bodyBase,armBase,armPistol;
	public bool pistolAcquired,pistolOut;
	public GameObject bulletObj;
	public float bulletSpeed,fireRate,fireRateM;
	public int clip,maxClip,mags,maxMags;
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

	void Damage(int amount)
	{
		health-=amount;
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
			smod=2;
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
		fireRateM-=Time.deltaTime;
		if(pistolAcquired && pistolOut)
		{
			if(Input.GetButtonUp("Fire1") && clip>0 && fireRate<=0)
			{
				//BANG
				clip-=1;
				fireSound.Play();
				GameObject bullet = (GameObject)Instantiate(bulletObj, armPistol.transform.position, armPistol.transform.rotation);
				//bullet.transform.Rotate(0, 180, 0);
				bullet.GetComponent<Rigidbody2D>().AddForce(-bullet.transform.right*bulletSpeed);
				fireRate=fireRateM;
				camera.localPosition += new Vector3(Random.Range(-0.125f, 0.125f), Random.Range(-0.125f, 0.125f));
				shake=0;
			}
			if(Input.GetButtonUp("Reload") && mags>0 && clip<maxClip)
			{
				//CLICK
				mags-=1;
				clip=maxClip;
			}
		}
		shake+=Time.deltaTime;
		camera.localPosition = Vector3.Lerp(camera.localPosition, new Vector3(0, 0, -2.375f), shake);
		if(Input.GetButtonUp("Reload") && !pistolOut)
		{
			buildMode=!buildMode;
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
	}

	Rect ScalePos(int x, int y, int w, int h, int amount)
	{
		return new Rect(x*amount, y*amount, w*amount, h*amount);
	}

	void OnGUI()
	{
		GUI.color = new Color(0.75f, 0.75f, 0.75f);
		for(int i = 0; i<maxHealth; i++)
		{
			if(health>i)
			{
				GUI.DrawTexture(ScalePos(5+(4*i), 5, 4, 5, 4), hpIcon);
			}
			else
			{
				GUI.DrawTexture(ScalePos(5+(4*i), 5, 4, 5, 4), emptyHpIcon);
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
				if(mags>i)
				{
					GUI.DrawTexture(ScalePos(5+(20*i), 10, 20, 4, 4), magIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(20*i), 10, 20, 4, 4), emptyMagIcon);
				}
			}
			for(int i = 0; i<maxClip; i++)
			{
				int xm = i;
				int ym = 0;
				if(xm>=10)
				{
					xm-=10;
					ym = 4;
				}
				if(clip>i)
				{
					GUI.DrawTexture(ScalePos(5+(10*xm), 14+ym, 10, 4, 4), clipIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(10*xm), 14+ym, 10, 4, 4), emptyClipIcon);
				}
			}
			for(int i = 0; i<maxOre; i++)
			{
				int xm = i;
				int ym = 0;
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(ore>i)
				{
					GUI.DrawTexture(ScalePos(5+(4*xm), 22+ym, 4, 5, 4), oreIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(4*xm), 22+ym, 4, 5, 4), emptyOreIcon);
				}
			}
		}
		else
		{
			for(int i = 0; i<maxOre; i++)
			{
				int xm = i;
				int ym = 0;
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(xm>=25)
				{
					xm-=25;
					ym += 4;
				}
				if(ore>i)
				{
					GUI.DrawTexture(ScalePos(5+(4*xm), 10+ym, 4, 5, 4), oreIcon);
				}
				else
				{
					GUI.DrawTexture(ScalePos(5+(4*xm), 10+ym, 4, 5, 4), emptyOreIcon);
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