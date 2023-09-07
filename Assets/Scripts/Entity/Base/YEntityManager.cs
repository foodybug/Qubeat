//#define Using_Section
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class YEntityManager : MonoBehaviour {
	
	#region - singleton -
	static YEntityManager instance; public static YEntityManager Instance {get{return instance;}}
	#endregion
	#region - member -
	const int ghostSpawnIntervalStage = 10;
	const float ghostDeltaSpeed = 0.2f;
	//object by instance id
	Dictionary<int, YBaseEntity> m_dicEntity = new Dictionary<int, YBaseEntity>();
	public Dictionary<int, YBaseEntity> dicEntity{get{return m_dicEntity;}}

    Dictionary<int, List<YCreationData>> m_dicRegisterEntity;
	
	#region - user control -
	Player m_PlayerEntity;public Player PlayerEntity{get{return m_PlayerEntity;}}
	public void SetPlayerEntity(Player _entity)
	{
		m_PlayerEntity = _entity;
	}
	
//	public void ReleasePlayerEntity()
//	{
//		m_PlayerEntity = null;
//		YCameraManager.Instance.SetPosOnFullShot();
//	}	
	
	public void MessageToUserEntity(YMessage _msg)
	{
		if(m_PlayerEntity != null)
			m_PlayerEntity.HandleMessage(_msg);
	}
	
	Boss m_BossEntity; public Boss BossEntity{get{return m_BossEntity;}}
	public void SetBossEntity(Boss _entity)
	{
		m_BossEntity = _entity;
	}
	#endregion
	#endregion
	#region - init & release -
	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);

		instance = this;

//		SectionInit();
	}
	
#if Using_Section	
	void SectionInit()
	{
		int width = YStageManager.Instance.SectionUnit;
		int height = YStageManager.Instance.SectionUnit;
		
		for(int i=-width; i<=width; ++i)
		{
			m_ddmdicEntity_Region.Add(i, new Dictionary<int, MultiSortedDictionary<eRegion, YBaseEntity>>());
			
			for(int j=-height; j<=height; ++j)
			{
				m_ddmdicEntity_Region[i].Add(j, new MultiSortedDictionary<eRegion, YBaseEntity>());
//				Debug.Log("Section " + i + "," + j + " is set");
			}
		}
	}
#endif
	
	void Start()
	{
	}

	void OnEnable()
	{
		//Debug.Log("YEntityManager:: OnEnable");
	}
	
	void OnDestroy()
	{
//		ReleaseStage();
	}
	#endregion
	#region - game flow -
	void Update () {
//		RefreshSection();
	}
	
#if Using_Section
	List<YBaseEntity> m_listSectionChange = new List<YBaseEntity>();
	List<YBaseEntity> m_listSectionDeletion = new List<YBaseEntity>();
	void RefreshSection()
	{
		m_listSectionChange.Clear();
		m_listSectionDeletion.Clear();
		
		float unitX = YStageManager.Instance.UnitSize.x;
		float unitY = YStageManager.Instance.UnitSize.y;
		
		Section2D section = new Section2D(0, 0);
		YBaseEntity entity;
		
		foreach(KeyValuePair<int, YBaseEntity> pair in m_dicEntity)
		{
			entity = pair.Value;
			
			section.x = Mathf.RoundToInt(entity.transform.position.x / unitX);
			section.y = Mathf.RoundToInt(entity.transform.position.z / unitY);
			
			if(entity.Section.SameValue(section) == false)
			{
				if(m_ddmdicEntity_Region.ContainsKey(section.x) == true && 
					m_ddmdicEntity_Region[section.x].ContainsKey(section.y) == true)
				{
//					Debug.Log("prev section:["+  entity.Section.x + "," + entity.Section.y  +"], " +
//						"cur section:[ ["+ section.x + "," + section.y +"]");
					entity.SetSection(section);
					
					m_listSectionChange.Add(entity);
					m_ddmdicEntity_Region[section.x][section.y].Add(entity.Region, entity);
				}
				else
				{
					Debug.LogWarning("Object is in wrong place. remove this.");
					m_listSectionDeletion.Add(entity);
				}
			}
			else
			{
				
			}
		}
		
//		foreach(KeyValuePair<int, Dictionary<int, MultiSortedDictionary<eRegion, YBaseEntity>>> pair1 in m_ddmdicEntity_Region)
//		{
//			foreach(KeyValuePair<int, MultiSortedDictionary<eRegion, YBaseEntity>> pair2 in pair1.Value)
//			{
//				foreach(KeyValuePair<eRegion, List<YBaseEntity>> pair3 in pair2.Value)
//				{
//					foreach(YBaseEntity entity in pair3.Value)
//					{
//						section.x = Mathf.RoundToInt(entity.transform.position.x / unitX);
//						section.y = Mathf.RoundToInt(entity.transform.position.y / unitY);
//						
//						if(entity.Section.SameValue(section) == false)
//						{
//							if(m_ddmdicEntity_Region.ContainsKey(section.x) == true && 
//								m_ddmdicEntity_Region[section.x].ContainsKey(section.y) == true)
//							{
//								entity.SetSection(section);
////								pair3.Value.Remove(entity);
//								m_listSectionChange.Add(entity);
//								m_ddmdicEntity_Region[section.x][section.y].Add(entity.Region, entity);
//							}
//							else
//							{
//								Debug.LogWarning("Object is in wrong place. remove this.");
////								RemoveEntity(entity.Id);
//								m_listSectionDeletion.Add(entity);
//							}
//						}
//						else
//						{
//							
//						}
//					}
//				}
//			}
//		}
		
		foreach(YBaseEntity entity1 in m_listSectionChange)
		{
			m_ddmdicEntity_Region[entity1.Section.x][entity1.Section.y].Remove(entity1.Region, entity1);
		}
		
		foreach(YBaseEntity entity1 in m_listSectionDeletion)
		{
			m_ddmdicEntity_Region[entity1.Section.x][entity1.Section.y].Remove(entity1.Region, entity1);
		}
	}
	
#endif
	#endregion
	#region - message handling -
//	SortedList<float, YMessage> m_slistMessage = new SortedList<float, YMessage>();
//	List<float> m_listDeletionReserved = new List<float>();
	
	public void DispatchMessage(int _id, YMessage _msg, bool _needReceiver = true){
		if(m_dicEntity.ContainsKey(_id) == true)
		{
			m_dicEntity[_id].HandleMessage(_msg);
		}
		else if(_needReceiver == true)
		{
			Debug.LogWarning("No id is found while dispatching message. id = " + _id);
		}
	}
	
	public void DispatchMessageToAll(YMessage _msg){
		foreach(KeyValuePair<int, YBaseEntity> pair in m_dicEntity){
			if(pair.Value != null)
				pair.Value.HandleMessage(_msg);
		}
	}
	
	public void DispatchMessageToAllAsync(YMessage _msg)
	{
		StartCoroutine(DispatchAllAsync(_msg));
	}
	
	IEnumerator DispatchAllAsync(YMessage _msg)
	{
		foreach(KeyValuePair<int, YBaseEntity> pair in m_dicEntity){
			yield return null;
			pair.Value.HandleMessage(_msg);
		}
	}
	
//	public void RegisterMessage(float _delay, YMessage _msg)
//	{
//		m_slistMessage.Add(Time.time + _delay, _msg);
//	}
	#endregion
	#region - object managing -
	public YBaseEntity CreateEntity(YCreationData _data)
	{
        if (_data == null)
			return null;

        _data.Proc_RegisteredEntity();
        YBaseEntity entity = _data.Create();

		if(entity == null)
		{
			Debug.Log("YEntityManager:: CreateEntity: exceed limit. creation will be ignored");
			return null;
		}

		m_dicEntity.Add(entity.Id, entity);
		
#if Using_Section
		Section2D section = new Section2D(entity.transform.position);
		entity.SetSection(section);
		
		if(m_ddmdicEntity_Region.ContainsKey(section.x) == true && 
								m_ddmdicEntity_Region[section.x].ContainsKey(section.y) == true)
		{
			m_ddmdicEntity_Region[section.x][section.y].Add(entity.Region, entity);
		}
		else
		{
			Debug.Log("Object is set in " + section.x + "," + section.y + " section");
			RemoveEntity(entity.Id);
		}
#endif
		return entity;
	}

    public void GenerateGhost(float _speed, float _delta, float _interval)
    {
        StartCoroutine(_GenerateGhost(_speed, _delta, _interval));
    }

    IEnumerator _GenerateGhost(float _speed, float _delta, float _interval)
    {
        float speed = _speed;
        float delta = _delta;
        float interval = _interval;

        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (YEntityManager.Instance.PlayerEntity == null)
                break;

            Vector3 pos = YStageManager.Instance.GetRandomPosOuterPlane(0);
            CreationData_Ghost creation = new CreationData_Ghost(pos, 10);
            creation.SetSpeed(speed);
            CreateEntity(creation);

            speed += delta;
        }
    }

    public void RegisterEvolEntities(Dictionary<int, List<YCreationData>> dicEntities)
    {
        m_dicRegisterEntity = dicEntities;
    }

    public void RemoveEntity(int _id)
	{
		RemoveEntity(_id, true);
	}

	readonly string defaultName = "CachedGeneral";
	public void RemoveEntity(int _id, bool _destroy)
	{
		if(m_dicEntity.ContainsKey(_id) == true)
		{
			YBaseEntity entity = m_dicEntity[_id];
			
//			m_mdicEntity_Type.Remove(entity.EntityType, entity);
//			m_mdicEntity_Region.Remove(entity.Region, entity);
			m_dicEntity.Remove(_id);
			
#if Using_Section
			if(m_ddmdicEntity_Region.ContainsKey(entity.Section.x) == true && 
								m_ddmdicEntity_Region[entity.Section.x].ContainsKey(entity.Section.y) == true)
			{
//				Debug.Log("Section(" + entity.Id + ") " + entity.Section.x + "," + entity.Section.y + " entity is deleted");
				m_ddmdicEntity_Region[entity.Section.x][entity.Section.y].Remove(entity.Region, entity);
			}
#endif
			
			if(_destroy == true)
				Destroy(entity.gameObject);
			else
			{
				entity.name = defaultName;
				entity.gameObject.SetActive(false);
//				m_queEntity.Enqueue(entity.gameObject);

				Destroy(entity);
			}
		}
		else
		{
			Debug.LogWarning("There is no id:" + _id + " entity");
		}
	}
	#endregion
	#region - search function -
	public bool CheckPlayerInRange(Vector3 _pos, float _range)
	{
		if(m_PlayerEntity == null)
			return false;
		
		float dist = Vector3.Distance(_pos, m_PlayerEntity.transform.position);
		if(dist < _range)
			return true;
		else
			return false;
	}
	
	public YBaseEntity GetEntityByInstanceID(int _id)
	{
		if(m_dicEntity.ContainsKey(_id) == true)
			return m_dicEntity[_id];
		else
		{
			Debug.LogWarning("GetEntityByInstanceID: id[" + _id + "] not exist");
			return null;
		}
	}
    #endregion
    #region - public -
    public void GenerateEvolEntities(int lv, int allStageClearCount)
    {
        StartCoroutine(_GenerateEvolEntities(new GEE(lv, allStageClearCount)));
    }
	struct GEE
    {
		public int lv;
		public int allStageClearCount;
		public GEE(int lv, int allStageClearCount)
        {
			this.lv = lv; this.allStageClearCount = allStageClearCount;
        }
	}
    IEnumerator _GenerateEvolEntities(GEE info)
    {
		if (m_dicRegisterEntity.ContainsKey(info.lv))
		{
			List<YCreationData> list = m_dicRegisterEntity[info.lv];

			//int count = 5;
			//float rate = 0.1f;
			foreach (YCreationData node in list)
			{
				//yield return new WaitForSeconds(rate);
				yield return null;

				node.pos_ = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
				CreateEntity(node);
			}

			Debug.Log("YEntityManager:: _GenerateEvolEntities: lv" + info.lv + " entities count = " + list.Count);

			if(info.lv % ghostSpawnIntervalStage == 0)//°í½ºÆ®
            {
				Vector3 pos = YStageManager.Instance.GetRandomPosOuterPlane(0);
				CreationData_Ghost creation = new CreationData_Ghost(pos, ghostSpawnIntervalStage);
				creation.SetSpeed(((info.lv + info.allStageClearCount) / (float)ghostSpawnIntervalStage) * ghostDeltaSpeed);
				CreateEntity(creation);
			}
		}
		else
			Debug.LogWarning("YEntityManager:: _GenerateEvolEntities: no level data. lv = " + info.lv);
	}

    public GameObject GetObject()
	{
		return GameObject.Instantiate(Resources.Load("Enemy/Enemies/General")) as GameObject;
	}
	
	public GameObject GetCachedObject()
	{
		return GameObject.Instantiate(Resources.Load("Enemy/Enemies/General")) as GameObject;
		
		//		if(m_queEntity.Count > 0)
		//			return m_queEntity.Dequeue();
		//		else
		//		{
		//			Debug.LogWarning("YEntityManager:: GetCachedObject: no more cached object");
		//			return null;
		//		}
	}
	#endregion
	
//	void OnGUI()
//	{
////		int y = 0;
////		foreach(KeyValuePair<eRegion, List<YBaseEntity>> pair in m_mdicEntity_Region)
////		{
////			GUI.Label(new Rect(30, 30 + y * 15, 100, 40), pair.Key + ":" + pair.Value.Count);
////			++y;
////		}
//		
//		if(m_PlayerEntity != null && m_PlayerEntity.Living == true)
//		{
//			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(m_PlayerEntity.transform.position);
//			
//			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 80, 30), "Id:" + m_PlayerEntity.Id);
//			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y + 15, 80, 30), "HP:" + m_PlayerEntity.Hp);
//		}
//	}
}