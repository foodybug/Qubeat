using UnityEngine;
using System.Collections;

public class PointerSpinner : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void LateUpdate()
	{
		Player player = YEntityManager.Instance.PlayerEntity;
		if( player != null)
		{
			transform.rotation = player.transform.rotation;
		}
	}
}
