using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {
	
	Vector3 m_Direction;
	
	void OnGUI()
	{
	}

	// Use this for initialization
	void Start () {
		name = "Cube : " + gameObject.GetInstanceID();
		GetComponent<CharacterController>();
		InvokeRepeating("ChangeDirection", 0, 2 + Random.Range(0, 1f));
		
		float startTime = Time.realtimeSinceStartup;
		for( int i=0; i<1000; ++i)
		{
			Msg_Spin_Stop stop = new Msg_Spin_Stop();
			SendMessage("Test_SendMessage", stop);
		}
		
		Debug.Log("SendMessage time = " + (startTime - Time.realtimeSinceStartup));
		startTime = Time.realtimeSinceStartup;
		
		for( int i=0; i<1000; ++i)
		{
			Msg_Spin_Stop stop = new Msg_Spin_Stop();
			Test_NewAndFunction( stop);
		}
		
		Debug.Log("SendMessage time = " + (startTime - Time.realtimeSinceStartup));
	}
	
	void ChangeDirection()
	{
		m_Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(m_Direction * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider _collider)
	{
		if(_collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log("OnTriggerEnter : object hit : " + _collider.gameObject.name);
			Death();
		}
	}
	
	void OnCollisionEnter(Collision _hit)
	{
		if(_hit.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log("OnCollisionEnter : object hit : " + _hit.gameObject.name);
			Death();
		}
	}
	
//	void OnControllerColliderHit(ControllerColliderHit _hit)
//	{
//		if(_hit.gameObject.layer == LayerMask.NameToLayer("Player"))
//		{
//			Debug.Log("OnControllerColliderHit : object hit : " + _hit.gameObject.name);
//			Death();
//		}
//	}
	
	void Death()
	{
		Instantiate(Resources.Load("Player/Effect/Test"), transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
	
	void Test_SendMessage( Msg_Spin_Stop _stop)
	{
	}
	
	void Test_NewAndFunction( Msg_Spin_Stop _stop)
	{
	}
}
