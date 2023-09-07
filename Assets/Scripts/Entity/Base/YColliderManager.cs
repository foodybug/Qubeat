//#define Using_Section
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class YColliderManager : MonoBehaviour {
	
	#region - singleton -
	static YColliderManager instance; public static YColliderManager Instance {get{return instance;}}
	#endregion
	#region - member -
	[SerializeField] float frame_ = 15f;
	float m_FrameRate; public float FrameRate{get{return m_FrameRate;}}
	[SerializeField] Vector2 m_SectionUnit = new Vector2(20, 20);
	
	//collider by instance id
	Dictionary<int, YCollider> m_dicCollider = new Dictionary<int, YCollider>();
	public Dictionary<int, YCollider> dicCollider{get{return m_dicCollider;}}

	Dictionary<int, MultiSortedDictionary<int, YCollider>> m_dmdicCollider =
		new Dictionary<int, MultiSortedDictionary<int, YCollider>>();
	
//	IntRect m_CalculatedSize;
	#endregion
	#region - init & release & update -
	void Awake()
	{
		instance = this;
		
		SectionInit();
	}
	
	void SectionInit()
	{
		int width = (int)(YStageManager.Instance.StageSize.x / m_SectionUnit.x / 2 + 1);
//		int height = (int)(YStageManager.Instance.StageSize.y / m_SectionUnit.y / 2 + 1);
		
//		m_CalculatedSize = new IntRect(width, height);
		
		for(int i=-width; i<=width; ++i)
		{
			m_dmdicCollider.Add(i, new MultiSortedDictionary<int, YCollider>());
		}
	}
	
	void Start()
	{
//		SectionInit();
		
		m_FrameRate = 1 / frame_;
		
//		InvokeRepeating("CheckCollision", 0, rate);
	}
	
	void OnDestroy()
	{
//		ReleaseStage();
	}
	
	void Update () {
//		RefreshSection();
	}	
//	List<YCollider> m_listSectionChange = new List<YCollider>();
//	List<YCollider> m_listSectionDeletion = new List<YCollider>();
//	void RefreshSection()
//	{
//		m_listSectionChange.Clear();
//		m_listSectionDeletion.Clear();
//		
//		float unitX = YStageManager.Instance.UnitSize.x;
//		float unitY = YStageManager.Instance.UnitSize.y;
//		
//		Section2D section = new Section2D(0, 0);
//		YBaseEntity entity;
//		
//		foreach(KeyValuePair<int, YBaseEntity> pair in m_dicEntity)
//		{
//			entity = pair.Value;
//			
//			section.x = Mathf.RoundToInt(entity.transform.position.x / unitX);
//			section.y = Mathf.RoundToInt(entity.transform.position.z / unitY);
//			
//			if(entity.Section.SameValue(section) == false)
//			{
//				if(m_ddmdicEntity_Region.ContainsKey(section.x) == true && 
//					m_ddmdicEntity_Region[section.x].ContainsKey(section.y) == true)
//				{
////					Debug.Log("prev section:["+  entity.Section.x + "," + entity.Section.y  +"], " +
////						"cur section:[ ["+ section.x + "," + section.y +"]");
//					entity.SetSection(section);
//					
//					m_listSectionChange.Add(entity);
//					m_ddmdicEntity_Region[section.x][section.y].Add(entity.Region, entity);
//				}
//				else
//				{
//					Debug.LogWarning("Object is in wrong place. remove this.");
//					m_listSectionDeletion.Add(entity);
//				}
//			}
//			else
//			{
//				
//			}
//		}
//		
//		foreach(YBaseEntity entity1 in m_listSectionChange)
//		{
//			m_ddmdicEntity_Region[entity1.Section.x][entity1.Section.y].Remove(entity1.Region, entity1);
//		}
//		
//		foreach(YBaseEntity entity1 in m_listSectionDeletion)
//		{
//			m_ddmdicEntity_Region[entity1.Section.x][entity1.Section.y].Remove(entity1.Region, entity1);
//		}
//	}
	#endregion
	#region - public method -
	public void AddCollider(YCollider _col)
	{
		_col.SetSection(GetCurSection(_col.transform.position));
		
		m_dicCollider.Add(_col.gameObject.GetInstanceID(), _col);
		m_dmdicCollider[_col.Section.x].Add(_col.Section.y, _col);
	}
	
	public void RefreshCollider(YCollider _col)
	{
//		m_dicCollider.Remove(_col.gameObject.GetInstanceID());
//		if(m_dmdicCollider.ContainsKey(_col.Section.x) == true &&
//			m_dmdicCollider[_col.Section.x].ContainsKey(_col.Section.y) == true)
		m_dmdicCollider[_col.Section.x].Remove(_col.Section.y, _col);
		
		_col.SetSection(GetCurSection(_col.transform.position));
		
//		m_dicCollider.Add(_col.gameObject.GetInstanceID(), _col);
//		if(m_dmdicCollider.ContainsKey(_col.Section.x) == true &&
//			m_dmdicCollider[_col.Section.x].ContainsKey(_col.Section.y) == true)
		m_dmdicCollider[_col.Section.x].Add(_col.Section.y, _col);
	}
	
	public void RemoveCollider(YCollider _col)
	{
		m_dicCollider.Remove(_col.gameObject.GetInstanceID());
		
		if(m_dmdicCollider.ContainsKey(_col.Section.x) == true &&
			m_dmdicCollider[_col.Section.x].ContainsKey(_col.Section.y) == true)
		m_dmdicCollider[_col.Section.x].Remove(_col.Section.y, _col);
	}
	
	IntRect GetCurSection(Vector3 _pos)
	{
		IntRect rect;
		Vector2 size = YStageManager.Instance.StageSize;
		
		rect.x = Mathf.RoundToInt(_pos.x / size.x);
		rect.y = Mathf.RoundToInt(_pos.z / size.y);
		
		return rect;
	}
	#endregion
	#region - search function -
//	public IntRect GetCurSection(Vector3 _pos)
//	{
//		IntRect rect;
//		Vector2 size = YStageManager.Instance.StageSize;
//		
//		rect.x = Mathf.RoundToInt(_pos.x / size.x);
//		rect.y = Mathf.RoundToInt(_pos.z / size.y);
//		
//		return rect;
//	}
	
	float __total;
	float __dist;
	Msg_CollisionOccurred __collisionOccured = new Msg_CollisionOccurred();
	public void CheckCollision(YCollider _col)
	{
		for(int i = _col.Section.x - 1; i < _col.Section.x + 2; ++i)
		{
			if(m_dmdicCollider.ContainsKey(i) == true)
			{
				for(int j = _col.Section.y - 1; j < _col.Section.y + 2; ++j)
				{
					if(m_dmdicCollider[i].ContainsKey(j) == true)
					{
						foreach(YCollider col in m_dmdicCollider[i][j])
						{
							if(col != _col && col.CheckValidMask(_col) == true)
							{
//								__total = _col.Radius * _col.Radius + col.Radius * col.Radius;
								__total = (_col.Radius + col.Radius) * (_col.Radius + col.Radius);
								__dist = Vector3.SqrMagnitude(_col.transform.position - col.transform.position);
								
								if(__total > __dist)
								{
									col.HandleMessage(__collisionOccured.SetCollision(_col));
									_col.HandleMessage(__collisionOccured.SetCollision(col));
									
//									col.HandleMessage(new Msg_CollisionOccurred(_col));
//									_col.HandleMessage(new Msg_CollisionOccurred(col));
								}
							}
						}
					}
				}
			}
		}
	}
	
//	public YCollider[] GetClosestSmallerAndBigger(Vector3 _pos)
//	{
//		Player player = YEntityManager.Instance.PlayerEntity;
//		
//		IntRect section = GetCurSection(_pos);
//		YCollider[] colliders = new YCollider[2];
//		
//		float distSmaller = float.MaxValue;
//		float distBigger = float.MaxValue;
//		
//		for(int i = section.x - 1; i < section.x + 2; ++i)
//		{
//			if(m_dmdicCollider.ContainsKey(i) == true)
//			{
//				for(int j = section.y - 1; j < section.y + 2; ++j)
//				{
//					if(m_dmdicCollider[i].ContainsKey(j) == true)
//					{
//						foreach(YCollider col in m_dmdicCollider[i][j])
//						{
//							YBaseEntity entity = col.AttachedEntity;
//							
//							if(entity.GetType() != typeof(Player) && entity.GetType() != typeof(Boss) &&
//								entity.Living == true)
//							{
//								float dist = Vector3.SqrMagnitude(col.transform.position - player.transform.position);
//								if(entity.Level <= player.Level)
//								{
//									if(dist < distSmaller)
//									{
//										distSmaller = dist;
//										colliders[0] = col;
//									}
//								}
//								else
//								{
//									if(dist < distBigger)
//									{
//										distBigger = dist;
//										colliders[1] = col;
//									}
//								}
//							}
//						}
//					}
//				}
//			}
//		}
//		
//		return colliders;
//	}
	
	public CloseColliders GetCloseColliders(Vector3 _pos)
	{
		Player player = YEntityManager.Instance.PlayerEntity;
		
		IntRect section = GetCurSection(_pos);
		
		CloseColliders colliders = new CloseColliders();
		
		for(int i = section.x - 1; i < section.x + 2; ++i)
		{
			if(m_dmdicCollider.ContainsKey(i) == true)
			{
				for(int j = section.y - 1; j < section.y + 2; ++j)
				{
					if(m_dmdicCollider[i].ContainsKey(j) == true)
					{
						foreach(YCollider col in m_dmdicCollider[i][j])
						{
							YBaseEntity entity = col.AttachedEntity;
							
							if(entity.GetType() != typeof(Player) && entity.GetType() != typeof(Boss) &&
								entity.Living == true)
							{
								float dist = Vector3.SqrMagnitude(col.transform.position - player.transform.position);
								if(entity.Level <= player.Level)
								{
									colliders.slistSmaller.Add(dist, col);
								}
								else
								{
									colliders.slistBigger.Add(dist, col);
								}
							}
						}
					}
				}
			}
		}
		
		return colliders;
	}
	#endregion
	
//	void OnGUI()
//	{
//		if(m_PlayerEntity != null && m_PlayerEntity.Living == true)
//		{
//			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(m_PlayerEntity.transform.position);
//			
//			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 80, 30), "Id:" + m_PlayerEntity.Id);
//			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y + 15, 80, 30), "HP:" + m_PlayerEntity.Hp);
//		}
//	}
}