using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {
	
	[SerializeField] float speed_ = 2;
	[SerializeField] float angleSpeed = 1;
	Vector3 angle;
	
	Bouncer m_Target;
	
	CharacterController m_Controller;
	
	void Awake()
	{
		GetComponent<Renderer>().material.color = Color.white;
		
		m_Controller = GetComponent<CharacterController>();
	}
	
	void Start () {
		angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
		
		m_Target = Bouncer.GetRandomBouncer();
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate(angle * angleSpeed * Time.deltaTime);
		
		if(m_Target != null)
		{
			Vector3 direction = m_Target.transform.position - transform.position;
//			transform.position += direction.normalized * speed_ * Time.deltaTime;
			
			m_Controller.Move(direction.normalized * speed_ * Time.deltaTime);
		}
	}
	
	void OnTriggerEnter(Collider _col)
	{
		Bouncer bouncer = _col.GetComponent<Bouncer>();
		
		if(bouncer != null)
		{
//			GameObject obj = Instantiate(Resources.Load("Effect/Effect_Title")) as GameObject;
//			obj.transform.position = transform.position;
//			obj.GetComponent<ParticleSystem>().startColor = _col.GetComponent<Bouncer>().color;
//			Destroy(obj, 0.5f);

			if(ExplosionManager.Instance == null)
				Debug.LogError("Actor:: OnTriggerEnter: no ExplosionManager instance");
			else
				ExplosionManager.Instance.SetExplosion(transform, transform, _col.GetComponent<Bouncer>().color, 0.5f);
					
			Bouncer.RemoveBouncer(bouncer);
			m_Target = Bouncer.GetRandomBouncer();
		
			angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

            GetComponent<Renderer>().material.color = bouncer.color;
		}
	}
}
