using UnityEngine;
using System.Collections;

public class MKGlowSystemMobileInit : MonoBehaviour 
{
	//On some mobile devices the glow has the false resolution
	//if you have on runtime errors with the glow call this function
	private void Start () 
	{
		this.GetComponent<MKGlowSystem.MKGlow>().ReloadMKGlowSystem();
	}
}
