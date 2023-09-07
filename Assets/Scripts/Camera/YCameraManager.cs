using UnityEngine;
using System.Collections;

public class YCameraManager : MonoBehaviour {
	
	public enum ViewMode {Entity, Top}
	
	#region - singleton -
	static YCameraManager instance;
	public static YCameraManager Instance
	{get{return instance;}}
	#endregion
	
	#region - member -
	ViewMode m_EntityViewMode = ViewMode.Entity;
	
	Transform m_FocusedEntity;
	float m_CameraHeight = 1;
	
	Camera m_MainCamera;
	
	Camera m_BGCamera;
	ParticleSystem m_BGParticle;
	
	float m_MaxIntensity = 0;
	float m_Intensity = 0;
	
	Transform m_Transform;
	#endregion
	#region - init -
	void Awake()
	{
		instance = this;
		
		m_Transform = transform;
		
		m_MainCamera = Camera.main;
		m_MainCamera.transform.localPosition = Vector3.zero;
		m_MainCamera.transform.localRotation = Quaternion.identity;
		
		GameObject obj = Instantiate(Resources.Load("Effect/BG")) as GameObject;
		m_BGCamera = obj.GetComponent<Camera>();
		
//		m_BGParticle = m_BGCamera.GetComponentInChildren<ParticleSystem>();
//		m_BGParticle.emissionRate = 1;
//		m_BGParticle.startSpeed = 5;
	}
	
	void Start ()
	{
		m_Transform.position = YStageManager.Instance.StdCameraPos;
		m_Transform.rotation = Quaternion.LookRotation(Vector3.down);
		
		InvokeRepeating("CheckPlayerLevel", 0, 0.1f);
//		StartCoroutine("ChangingBgColor");
	}
	
	void CheckPlayerLevel()
	{
		Player player = YEntityManager.Instance.PlayerEntity;	
		m_MaxIntensity = player.Exp * 0.007f;
		
		
//		m_BGParticle.startSpeed = player.Exp * 0.1f + 5;
//		m_BGParticle.emissionRate = player.Exp;
	}
	
	IEnumerator ChangingBgColor()
	{
		while(true)
		{
			while(true)
			{
				yield return null;
				
				if(m_Intensity > m_MaxIntensity)
					break;
				
				m_Intensity += Time.deltaTime * (m_MaxIntensity * m_MaxIntensity * m_MaxIntensity * 20);
				m_BGCamera.backgroundColor = new Color(m_Intensity, m_Intensity, m_Intensity);
			}
			
			while(true)
			{
				yield return null;
				
				if(m_Intensity < 0)
					break;
				
				m_Intensity -= Time.deltaTime * (m_MaxIntensity * m_MaxIntensity * m_MaxIntensity * 20);
				m_BGCamera.backgroundColor = new Color(m_Intensity, m_Intensity, m_Intensity);
			}
		}
	}
	#endregion
	#region - update & input -
	void Update () {
	}
	
	Vector3 pos;
	void LateUpdate()
	{
		if(m_FocusedEntity != null && m_EntityViewMode == ViewMode.Entity)
		{
			pos = m_FocusedEntity.position;
			pos.y = m_CameraHeight;
			
			m_Transform.position = Vector3.Lerp(m_Transform.position, pos, Time.deltaTime * 4f);
		}
	}
	#endregion
	#region - process -
	public void SetPosOnEntity(YBaseEntity _entity, float _height)
	{
//		m_ViewMode = ViewMode.Entity;
		m_FocusedEntity = _entity.transform;
		m_CameraHeight = _height;
	}
	
	public void SetPosOnFullShot()
	{
//		m_ViewMode = ViewMode.Top;
		m_FocusedEntity = null;
		transform.position = YStageManager.Instance.StdCameraPos;
		YInputManager.Instance.SetActivation(true);
	}
	
	public void PlayerGetSignificantExp(Player _player)
	{
		float m_Intensity = Mathf.Pow(_player.Exp, 2f) * 0.00005f;
		StartCoroutine(CameraShaking(m_Intensity));
	}
	
	public void BossDefeated()
	{
		StartCoroutine(CameraShaking(0.3f));
	}
	
	public void EntityDeath(YBaseEntity _entity)
	{
//		Debug.Log("shake");
//		StartCoroutine(CameraShaking(_entity));
	}
	
	public void PlayerLevelUp()
	{
		Player player = YEntityManager.Instance.PlayerEntity as Player;
		m_CameraHeight += player.Level / 5;
		
		m_BGCamera.backgroundColor = Color.black;
		
		m_Intensity = 0;
		m_MaxIntensity = 0;
		
//		m_BGParticle.Clear();
	}
	
	public void PlayerDeath()
	{
		m_BGCamera.backgroundColor = Color.black;
		
//		StopCoroutine("ChangingBgColor");
	}
	
	IEnumerator CameraShaking(float _intensity)
	{
		float startTime = Time.time;
		
		while(true)
		{
			yield return null;
			
			m_MainCamera.transform.localPosition = Random.insideUnitSphere * _intensity;
			
			if(Time.time - startTime > 0.15f)
				break;
		}
		
		m_MainCamera.transform.localPosition = Vector3.zero;
	}
	#endregion
	#region - ui -
//	void OnGUI()
//	{
//		MoveWindow();
//	}
//	
//	void MoveWindow()
//	{
//		if(GUI.RepeatButton(m_LeftButtonRect, "Left") == true)
//		{
//			if(transform.position.x > m_vBoundaryMin.x)
//				transform.position += new Vector3(-m_fUnitMovement, 0, 0);
//		}
//		
//		if(GUI.RepeatButton(m_RightButtonRect, "Right") == true)
//		{
//			if(transform.position.x < m_vBoundaryMax.x)
//				transform.position += new Vector3(m_fUnitMovement, 0, 0);
//		}
//		
//		if(GUI.RepeatButton(m_TopButtonRect, "Top") == true)
//		{
//			Vector3 point = GetInterSectingPointWithStandardPlane(transform.position);
//			
//			if(point.z < m_vBoundaryMax.z)
//			{
//				Debug.Log(point + "<" + m_vBoundaryMax.z);
//				Vector3 vector = Quaternion.Inverse(transform.rotation) * Vector3.forward;			
//				transform.Translate(vector * m_fUnitMovement);
//			}
//		}
//		
//		if(GUI.RepeatButton(m_BottomButtonRect, "Bottom") == true)
//		{
//			Vector3 point = GetInterSectingPointWithStandardPlane(transform.position);
//			
//			if(point.z > m_vBoundaryMin.z)
//			{
//				Debug.Log(point + ">" + m_vBoundaryMin.z);
//				Vector3 vector = Quaternion.Inverse(transform.rotation) * -Vector3.forward;			
//				transform.Translate(vector * m_fUnitMovement);
//			}
//		}
//	}
	#endregion
	#region - method -
//	Vector3 GetInterSectingPointWithStandardPlane(Vector3 _pos)
//	{
//		Ray ray = new Ray(_pos + transform.forward * 100000, 
//			                  -transform.forward);
//		float distance;
//		m_StandardPlane.Raycast(ray, out distance);
//		return ray.GetPoint(distance);
//	}
	#endregion
	
//	void OnGUI()
//	{
//		if(m_FocusedEntity != null)
//		{
//			switch(m_EntityViewMode)
//			{
//			case ViewMode.Entity:
//				if(GUI.Button(new Rect(950, 30, 100, 30), "Top View") == true)
//				{
//					m_EntityViewMode = ViewMode.Top;
//					transform.position = YStageManager.Instance.StdCameraPos;
//					YInputManager.Instance.SetActivation(false);
//				}
//				break;
//			case ViewMode.Top:
//				if(GUI.Button(new Rect(950, 30, 100, 30), "Entity View") == true)
//				{
//					m_EntityViewMode = ViewMode.Entity;
//					YInputManager.Instance.SetActivation(true);
//				}
//				break;
//			}
//		}
//	}
}
