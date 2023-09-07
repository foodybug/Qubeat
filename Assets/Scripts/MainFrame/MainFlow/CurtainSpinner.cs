using UnityEngine;
using System.Collections;

public class CurtainSpinner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
//		renderer.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
		
		float angular = 200;
		
		transform.Rotate(Vector3.forward, angular * Time.deltaTime);
	}
	
	void SM_Destroy()
	{
		Destroy(gameObject);
	}
	
	void SM_Color(Color _color)
	{
		GetComponent<Renderer>().material.color = _color;
	}
}
