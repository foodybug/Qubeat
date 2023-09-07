using UnityEngine;
using System.Collections;

public class CollisionTest_Player : MonoBehaviour {
	
	Vector3 m_Direction;
	Vector3 m_Destination;
	CharacterController m_Controller;

	// Use this for initialization
	void Start () {
		m_Destination = transform.position;
		name = "Player : " + gameObject.GetInstanceID();
		m_Controller = GetComponent<CharacterController>();
//		InvokeRepeating("ChangeDirection", 2, 2 + Random.Range(0, 1f));
	}
	
	void ChangeDirection()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(transform.position.y < 0) m_Direction.y = 0.5f;
		else m_Direction.y = -0.5f;
		
		m_Controller.Move(m_Direction * Time.deltaTime);
		
		Vector3 mag = transform.position - m_Destination; mag.y = 0;
		if(Vector3.Magnitude(mag) < 0.1f)
			m_Direction = Vector3.zero;
	}
	
	void OnTriggerEnter(Collider _collider)
	{
		if(_collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
		{
			Debug.Log("OnTriggerEnter : object hit : " + _collider.gameObject.name);
		}
	}
	
	void OnCollisionEnter(Collision _hit)
	{
		if(_hit.gameObject.layer != LayerMask.NameToLayer("Ground"))
		{
			Debug.Log("OnCollisionEnter : object hit : " + _hit.gameObject.name);
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit _hit)
	{
		if(_hit.gameObject.layer != LayerMask.NameToLayer("Ground"))
		{
			Debug.Log("OnControllerColliderHit : object hit : " + _hit.gameObject.name);
		}
	}
	
	public void SetDestination(Vector3 _pos)
	{
		m_Destination = _pos;
		
		m_Direction = m_Destination - transform.position;
		m_Direction.Normalize();
	}
}
