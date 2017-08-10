using UnityEngine;
using System.Collections;

public class PlayerPicker : MonoBehaviour
{
	public GameObject[] plays;

	void Start()
	{
		plays[MainMenu.curChar].SetActive(true);
	}
}