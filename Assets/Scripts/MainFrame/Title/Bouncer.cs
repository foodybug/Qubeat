using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bouncer : MonoBehaviour {
	
	static List<Bouncer> s_listBouncer = new List<Bouncer>();
	
	[SerializeField] float speed_ = 1;
	[SerializeField] Vector3 boundary_LT; public Vector3 Boundary_LT{get{return boundary_LT;}}
	[SerializeField] Vector3 boundary_RB; public Vector3 Boundary_RB{get{return boundary_RB;}}
	
	[SerializeField] Vector3 direction_ = Vector3.one;
	[SerializeField] float angleSpeed = 1;
	Vector3 angle;
	Color m_Color; public Color color{get{return m_Color;}}
	
	void Awake()
	{
		s_listBouncer.Add(this);
		
		m_Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		GetComponent<Renderer>().material.color = m_Color;
		transform.position = new Vector3(
			Random.Range(boundary_RB.x, boundary_LT.x),
			Random.Range(boundary_RB.y, boundary_LT.y),
			Random.Range(boundary_RB.z, boundary_LT.z));
	}
	
	void Start () {
		direction_ = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 prevDirection = direction_;
		transform.position += direction_ * speed_ * Time.deltaTime;
		transform.Rotate(angle * angleSpeed * Time.deltaTime);
		
		if(Mathf.Max(boundary_LT.x, boundary_RB.x) < transform.position.x)
			direction_.x = -Mathf.Abs(direction_.x);
		if(Mathf.Max(boundary_LT.y, boundary_RB.y) < transform.position.y)
			direction_.y = -Mathf.Abs(direction_.y);
		if(Mathf.Max(boundary_LT.z, boundary_RB.z) < transform.position.z)
			direction_.z = -Mathf.Abs(direction_.z);
		
		if(Mathf.Min(boundary_LT.x, boundary_RB.x) > transform.position.x)
			direction_.x = Mathf.Abs(direction_.x);
		if(Mathf.Min(boundary_LT.y, boundary_RB.y) > transform.position.y)
			direction_.y = Mathf.Abs(direction_.y);
		if(Mathf.Min(boundary_LT.z, boundary_RB.z) > transform.position.z)
			direction_.z = Mathf.Abs(direction_.z);
		
		if(prevDirection != direction_)
		{
			angle = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
		}
	}
	
	void OnDrawGizmos()
	{
		Vector3 center = (boundary_LT + boundary_RB) * 0.5f;
		Vector3 size = boundary_LT - boundary_RB;
		
		Gizmos.DrawWireCube(center, size);
	}
	
	public static Bouncer GetRandomBouncer()
	{
		Bouncer bouncer = null;
		
		if(s_listBouncer.Count != 0)
		{
			bouncer = s_listBouncer[Random.Range(0, s_listBouncer.Count)];
		}
		
		if(bouncer == null)
			Debug.LogError("bouncer return value is null");
		
		return bouncer;
	}
	
	public static void RemoveBouncer(Bouncer _bouncer)
	{
		s_listBouncer.Remove(_bouncer);
		Destroy(_bouncer.gameObject);
	}
	
	public static void ClearBouncerList()
	{
		s_listBouncer.Clear();
	}
}
