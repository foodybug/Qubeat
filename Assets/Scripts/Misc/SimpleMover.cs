using UnityEngine;
using System.Collections;

public class SimpleMover : MonoBehaviour {

	public enum eType {Normal, Translate}
	eType m_Type;
	Vector3 m_Direction;

	// Use this for initialization
	void Start () {
	//
	}
	
	// Update is called once per frame
	void Update () {
	
		switch(m_Type)
		{
		case eType.Normal:
			transform.position += m_Direction * Time.deltaTime;
			break;
		case eType.Translate:
			transform.Translate(m_Direction * Time.deltaTime);
			break;
		}
	}

	public void Init(Vector3 _dir, eType _type = eType.Normal)
	{
		m_Type = _type;
		m_Direction = _dir;
	}
}
