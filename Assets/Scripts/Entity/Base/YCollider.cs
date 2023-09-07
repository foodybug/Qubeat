using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IntRect
{
	public int x;
	public int y;
	
	public IntRect(int _x, int _y)
	{
		x = _x;
		y = _y;
	}
}

public class YCollider : YBaseComponent
{
    //	float s_RefreshSectionRate = 0.2f;

    Collider _col; public Collider col { get { return _col; } }

	IntRect m_Section; public IntRect Section{get{return m_Section;}}
	[SerializeField] float m_Radius; public float Radius{get{return m_Radius;}}
	bool m_Passive = false; public bool Passive{get{return m_Passive;}}
	
	YBaseEntity m_AttachedEntity; public YBaseEntity AttachedEntity{get{return m_AttachedEntity;}}
	
	Dictionary<System.Type, System.Type> m_dicTypeMask = new Dictionary<System.Type, System.Type>();
	
	void Awake()
	{
        _col = GetComponent<Collider>();

		RegisterReceiver(typeof(Msg_CollisionActive), OnCollisionActive);
		RegisterReceiver(typeof(Msg_CollisionSize), OnCollisionSize);
	}
	
	public override void Init()
	{
		base.Init();
	}
	
	void OnEnable()
	{
//		YColliderManager.Instance.AddCollider(this);
		
//		InvokeRepeating("RefreshSection", Random.Range(0f, s_RefreshSectionRate), s_RefreshSectionRate);
//		StartCoroutine("CheckCollision");
	}
	
	public void SetCollisionData(int _lv, System.Type[] _types, bool _passive)
	{
		m_Radius = _lv * 0.2f;
		
		foreach(System.Type type in _types)
		{
			m_dicTypeMask.Add(type, type);
		}
		
		m_Passive = _passive;
	}
	
	public void SetBossCollisionData(int _lv, System.Type[] _types, bool _passive)
	{
		m_Radius = _lv * 0.25f;
		
		foreach(System.Type type in _types)
		{
			m_dicTypeMask.Add(type, type);
		}
		
		m_Passive = _passive;
	}
	
	public void SetGhostCollisionData(int _lv, System.Type[] _types, bool _passive)
	{
		m_Radius = _lv * 0.1f;
		
		foreach(System.Type type in _types)
		{
			m_dicTypeMask.Add(type, type);
		}
		
		m_Passive = _passive;
	}

	void Start()
	{
		m_AttachedEntity = GetComponent<YBaseEntity>();
	}
	
	public void SetPassive(bool _value)
	{
		m_Passive = _value;
	}
	
	public void SetRefreshRate(float _rate)
	{
	}
	
	void RefreshSection()
	{
//		YColliderManager.Instance.RefreshCollider(this);
	}
	
	IEnumerator CheckCollision()
	{
		while(true)
		{
			yield return new WaitForSeconds(YColliderManager.Instance.FrameRate);
			
			if(m_Passive == false)
				YColliderManager.Instance.CheckCollision(this);
		}
	}
	
	public bool CheckValidMask(YCollider _col)
	{
		return m_AttachedEntity.Living == true && enabled == true && m_dicTypeMask.ContainsKey(_col.AttachedEntity.GetType());
	}
	
	void Update()
	{
	}
	
	void OnDestroy()
	{
//		YColliderManager.Instance.RemoveCollider(this);
	}
	
	#region - msg -
	void OnCollisionActive(YMessage _msg)
	{
		Msg_CollisionActive active = _msg as Msg_CollisionActive;
		
		if(active.active_ == true)
		{
			if(GetComponent<Collider>() != null)
				GetComponent<Collider>().enabled = true;
			
			this.enabled = true;
		}
		else
		{
			if(GetComponent<Collider>() != null)
				GetComponent<Collider>().enabled = false;
			
			StopAllCoroutines();
			this.enabled = false;
		}
	}
	
	void OnCollisionSize(YMessage _msg)
	{
//		Msg_CollisionSize size = _msg as Msg_CollisionSize;
		
		m_Radius = m_AttachedEntity.Level * 0.2f;
	}
	#endregion
	#region - public method -
	public void SetSection(IntRect _rect)
	{
		m_Section = _rect;
	}
	#endregion
	#region - native msg -
//	void OnCollisionEnter(Collision _col)
//	{
//		Debug.Log("OnCollisionEnter");
//		
//		Msg_CollisionOccurred col = new Msg_CollisionOccurred(this);
//		
////		m_AttachedEntity.HandleMessage(col);
//		
//		int id = _col.gameObject.GetInstanceID();
//		YEntityManager.Instance.DispatchMessage(id, col);
//	}
	
	void OnTriggerEnter(Collider _col)
	{
//		Debug.Log("OnTriggerEnter");
		
		Msg_CollisionOccurred col = new Msg_CollisionOccurred(this);
		
//		m_AttachedEntity.HandleMessage(col);
		
		int id = _col.gameObject.GetInstanceID();
		YEntityManager.Instance.DispatchMessage(id, col);
	}
	
//	void OnControllerColliderHit(ControllerColliderHit _hit)
//	{
//		Debug.Log("OnControllerColliderHit");
//		
//		Msg_CollisionOccurred col = new Msg_CollisionOccurred(this);
//		
////		m_AttachedEntity.HandleMessage(col);
//		
//		int id = _hit.gameObject.GetInstanceID();
//		YEntityManager.Instance.DispatchMessage(id, col);
//	}
	#endregion
	
//	[SerializeField] bool showUI_ = true;
//	void OnGUI()
//	{
//		if(showUI_ == true)
//		{
//			Vector3 pos = Camera.mainCamera.WorldToScreenPoint(transform.position);
//			Rect rect = new Rect(pos.x - 60, Screen.height - pos.y, 120, 20);
//			GUI.Label(rect, "[x:" + m_Section.x + "][y:" + m_Section.y + "]");
//		}
//	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, m_Radius);
	}
}

