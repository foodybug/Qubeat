using UnityEngine;
using System.Collections;

public class SimpleSpinner : MonoBehaviour {

	Vector3 m_Rotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(m_Rotation * Time.deltaTime);
	}

	public void Init(Vector3 _rot)
	{
		m_Rotation = _rot;
	}
}
