using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : YBaseEntity
{
	public readonly int realLevel = 2;
	//public int levelUpCount { get; private set;} = 1;
	//public float adjustLvCycleCompleted { get; private set; } = 1;
	//int allStageClearCount = 0;
	float adjustSpawnRatioUnit = 1.2f;

	//const float sizeRevision = 0.25f;

	const float normalizedSize_Min = 0.5f;
	const float normalizedSize_Max = 0.9f;// 0.74f;

	public enum eOperation {Death}
	
	#region - property -
    [SerializeField] bool useQL = false;
	#endregion
	#region - member -
	[SerializeField] string m_CurState;
	YStateMachine<Player> m_StateMachine;
    Renderer m_Renderer;
	
	float m_Exp = 0; public float Exp{get{return m_Exp;}}
	
	bool m_Invincible = false; public bool Invincible{get{return m_Invincible;}}

	public static readonly float s_MaxExp = 100f;
    public static readonly float s_ExpConsumeRatio = 0.3f;
    public static readonly float s_GameSpeedRatioByExp = 0.15f * 0.01f;
    #endregion
    #region - init & release & update -
    void Awake()
	{
        m_Renderer = GetComponent<Renderer>();

		//state
		m_StateMachine = new YStateMachine<Player>(this);
		m_StateMachine.RegisterState(new PlayerState_Idle(m_StateMachine));
		m_StateMachine.RegisterState(new PlayerState_Death(m_StateMachine));
		
		RegisterReceiver(typeof(Msg_Input_Move), OnInputMove);
		RegisterReceiver(typeof(Msg_CollisionOccurred), OnRcvCollision);
		RegisterReceiver(typeof(Msg_PlayerDeath), OnPlayerDeath);
		RegisterReceiver(typeof(Msg_PlayerInvincible), OnPlayerInvincible);
        RegisterReceiver(typeof(Msg_Movement_Start), OnMovementStart);
		RegisterReceiver(typeof(Msg_BossDefeated), OnBossDefeated);
	}
	
	public override void Init()
	{
		base.Init();
	}
	
	public override void SetCreationData(YCreationData _data)
	{
		CreationData_Player creation = _data as CreationData_Player;
		
		name = GetType() + "(" + gameObject.GetInstanceID() + ")";

		m_Level = realLevel;

        m_Renderer.material.color = Color.white;
		transform.position = creation.pos_;
		transform.localScale = Vector3.one * normalizedSize_Min;
		gameObject.layer = LayerMask.NameToLayer("Player");
		
		m_Id = gameObject.GetInstanceID();
	}

	IEnumerator Start ()
	{
		#region - trail -
//		GameObject trail = Instantiate(Resources.Load("Player/Effect/Trail/Trail")) as GameObject;
//		foreach(Trail node in trail.GetComponentsInChildren<Trail>())
//		{
//			node.Init(transform);
//		}
		#endregion

		//Instantiate(Resources.Load("UI/Ready_Text"));
		
		float standby = 1.5f;	
		yield return new WaitForSeconds(standby);
		
		SetState(typeof(PlayerState_Idle));

		
		SetEntityData();
		
		SimplePad.PlayerInitiated(this);
		
		#region - tutorial -
		if(TutorialManager.Instance != null)
			TutorialManager.Instance.PlayerStart();
		#endregion
	}
	
	void SetEntityData()
	{
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		m_Hp = status.playerStatus_.hp_;
		
		HandleMessage(new Msg_Movement_SpeedModify(status.playerStatus_.moveSpeed_));
	}
	
	void Update ()
	{
		m_StateMachine.Update();

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Space) == true)
			LevelUpProc(true);
#endif
	}
	#endregion
	
	#region - state -
	public void SetState(System.Type _state)
	{
		SetState(_state, null);
	}
	
	public void SetState(System.Type _state, YMessage _msg)
	{
		m_StateMachine.ChangeState(_state, _msg);
		m_CurState = _state.ToString();
//		Debug.Log("Character/Player::SetState: id=" + Id + " state changed to " + _state);
	}
	
	public void MessageToState(YMessage _msg)
	{
		m_StateMachine.MessageProcess(_msg);
	}
	#endregion
	#region - msg -
	void OnInputMove(YMessage _msg)
	{
		m_StateMachine.MessageProcess(_msg);	
	}
	void OnRcvCollision(YMessage _msg)
	{
		m_StateMachine.MessageProcess(_msg);
	}
	void OnPlayerDeath(YMessage _msg)
	{
        StartCoroutine(Death());
	}
	void OnPlayerInvincible(YMessage _msg)
	{
		m_Invincible = true;
	}

    void OnMovementStart(YMessage _msg)
    {
        Msg_Movement_Start move = _msg as Msg_Movement_Start;
        Vector3 dir = (move.dest_ - transform.position).normalized * Mathf.Pow(m_Exp * 0.01f, 10f) * 11f;
        Particle_BG.Instance.FlowProc(dir);
    }

	void OnBossDefeated(YMessage _msg)
	{
		// 보스 파괴에 따른 슬로우 필요
		BossDefeated();
	}
	#endregion
	#region - fsm operation -
	public void GetExp(int _lv, Msg_CollisionOccurred col = null)
	{
		ExpProc(_lv, col);
		LevelUpProc();
	}

	void ExpProc(int _lv, Msg_CollisionOccurred col = null)
	{
		//if (m_Level >= MAX_LEVEL)
		//	return;

		//float exp = 10 / (((m_Level - _lv) * (m_Level - _lv)) + 1);
		float exp = 10 / (((realLevel - _lv) * (realLevel - _lv)) + 1);
		if (exp > 0)
		{
			YCameraManager.Instance.PlayerGetSignificantExp(this);
			if (col != null)
			{
				if (col.col_.AttachedEntity.GetType() != typeof(Boss))
					m_Exp += exp;

				m_Color = col.col_.AttachedEntity.color;
				m_Renderer.material.color = m_Color;

				SimplePad.PlayerGetExp(m_Color);
			}
			else
				m_Exp += exp * s_ExpConsumeRatio;

			if (Time.timeScale >= 1f)
				Time.timeScale = 1 + m_Exp * s_GameSpeedRatioByExp;

			//Debug.Log("current exp = " + m_Exp);

			#region - turorial -
			if (TutorialManager.Instance != null)
				TutorialManager.Instance.PlayerExp_TutorialManager(this);
			#endregion
		}
	}

	void LevelUpProc(bool forced = false)
	{
		if (m_Exp < s_MaxExp && forced == false)
			return;

		//++m_Level;

		SceneSetter.Instance.LevelUp();
		int levelUpCount = SceneSetter.Instance.curStageIdx;
		int allStageClearCount = SceneSetter.Instance.allStageClearCount;

		Debug.Log("Level up. cur level count = " + levelUpCount);

		m_Exp = 0;
		transform.localScale = Vector3.one * normalizedSize_Min;
		HandleMessage(new Msg_CollisionSize(0.22f));
		HandleMessage(new Msg_Spin_AngleModify());
		YEntityManager.Instance.DispatchMessageToAll(new Msg_PlayerLevelUp());
		YEntityManager.Instance.GenerateEvolEntities(++levelUpCount, allStageClearCount);
		YCameraManager.Instance.PlayerLevelUp();
		SimplePad.PlayerLevelUp();
		Particle_BG.Instance.Reset();

		StopCoroutine("TimeProcess_LevelUp");
		StartCoroutine("TimeProcess_LevelUp");
		StartCoroutine(ConsumeProcess_CR());

		GameObject obj = Instantiate(Resources.Load("Effect/Explode_LevelUp")) as GameObject;
		//Instantiate(Resources.Load("UI/LevelUp_Text"));

		if (InGame.Instance != null)
			InGame.Instance.ShowLevelUp();

		Destroy(obj, 10f);

		#region - turorial -
		if (TutorialManager.Instance != null)
			TutorialManager.Instance.PlayerLevelUp_TutorialManager(this);
		#endregion
	}
	
	IEnumerator TimeProcess_LevelUp()
	{
		yield return null;
		
		Time.timeScale = 0.1f;

		transform.localScale = Vector3.one * normalizedSize_Max;
		float normal = 0f;

		while (true)
		{
			Time.timeScale += 0.05f;

			normal += Time.deltaTime;
			float revision = Mathf.Lerp(normalizedSize_Max, normalizedSize_Min, Time.timeScale);

			transform.localScale = Vector3.one * revision;

			Debug.Log("transform.localScale = " + ", revision = " + revision + ", normal = " + normal);

			if (Time.timeScale > 1f)
			{
				Time.timeScale = 1f;
				break;
			}
			
			yield return new WaitForSeconds(0.15f);
		}
	}

	IEnumerator ConsumeProcess_CR()
	{
		yield return new WaitForSeconds(PlayerOption.Instance.ConsumeWaitTime);

		YCollider playerCol = GetComponent<YCollider>();
		//List<YBaseEntity> listSameLevel = new List<YBaseEntity>();

		Collider[] cols = Physics.OverlapSphere(transform.position, PlayerOption.Instance.ConsumeRadiusOnLevelUp);
		foreach(Collider node in cols)
		{
			yield return new WaitForSeconds(PlayerOption.Instance.ConsumeInterval/* * 0.2f*/);

			YBaseEntity enemy = YEntityManager.Instance.GetEntityByInstanceID(node.gameObject.GetInstanceID());
			if (enemy != null)
			{
				if (enemy.Level < m_Level/* - 1*/)
				{
					GetExp(enemy.Level);
					enemy.HandleMessage(new Msg_CollisionOccurred(playerCol).SetByLevelUp());
				}
				//else if(enemy.Level == m_Level - 1)
				//	listSameLevel.Add(enemy);
			}
		}

		//yield return new WaitForSeconds(PlayerOption.Instance.ConsumeWaitTime * 0.5f);

		//foreach (YBaseEntity node in listSameLevel)
		//{
		//	yield return new WaitForSeconds(PlayerOption.Instance.ConsumeInterval);

		//	GetExp(node.Level);
		//	node.HandleMessage(new Msg_CollisionOccurred(playerCol).SetByLevelUp());
		//}
	}
    
	IEnumerator Death()
	{
		yield return null;
		
		if( AdvertisementManager.Instance != null)
			AdvertisementManager.Instance.Show_Banner();
		
		HandleMessage(new Msg_Movement_Stop());
		YCameraManager.Instance.PlayerDeath();
		SimplePad.DestroyThis();
		//InGame.DestroyThis();
		
		m_Renderer.enabled = false;
		m_Living = false;
		
		GameObject obj = Instantiate(Resources.Load("Effect/Explode")) as GameObject;
		obj.transform.position = transform.position;
		obj.GetComponent<ParticleSystem>().startColor = GetComponent<Renderer>().material.color;
		obj.GetComponent<ParticleSystem>().loop = false;
				
		obj.GetComponent<ParticleSystem>().startSpeed = 0.5f;
		obj.GetComponent<ParticleSystem>().startLifetime = 15f;
		obj.GetComponent<ParticleSystem>().emissionRate = s_MaxExp * m_Level;
		
		Time.timeScale = 1;
		
		Destroy(obj, obj.GetComponent<ParticleSystem>().startLifetime);

		if (MainFlow.Instance != null &&
            InGame.Instance != null &&
            InGame.GamePaused == false)
		{
			//MainFlow.Instance.PlayerDeath();
            InGame.Instance.PlayerDeath();
			MainFlow.Instance.Sound_PlayerDeath();
		}
		
		#region - turorial -
		if(TutorialManager.Instance != null)
		{
			TutorialManager.Instance.PlayerDeath_TutorialManager(this);
		}
		#endregion
		
		yield return new WaitForSeconds(1);
		
		YEntityManager.Instance.RemoveEntity(Id, false);
		Destroy(gameObject);
	}

    void BossDefeated()
    {
        m_Exp = 0;
        transform.localScale = Vector3.one;
        
        HandleMessage(new Msg_Spin_AngleModify());

        YCameraManager.Instance.PlayerLevelUp();
        SimplePad.PlayerLevelUp();
        Particle_BG.Instance.Reset();

        StopCoroutine("ConsumeProcess_Boss_CR");
        StartCoroutine("ConsumeProcess_Boss_CR");

        if (InGame.Instance != null)
            InGame.Instance.ShowLevelUp();
    }

    IEnumerator ConsumeProcess_Boss_CR()
    {
		Time.timeScale = 0.01f;

		yield return new WaitForSeconds(0.01f);

        YCollider playerCol = GetComponent<YCollider>();
		List<Collider> listCols = new List<Collider>(Physics.OverlapSphere(transform.position, PlayerOption.Instance.ConsumeRadiusOnStageUp));
		List<Collider> listDelete = new List<Collider>();

		while(true)
		{
			listDelete.Clear();
			
			//--m_Level;
			AdjustVertices(m_Level, 0.25f, true);
			HandleMessage(new Msg_CollisionSize());
			YEntityManager.Instance.DispatchMessageToAll(new Msg_PlayerLevelChanged());

			foreach (Collider node in listCols)
			{
				yield return null;

				YBaseEntity enemy = YEntityManager.Instance.GetEntityByInstanceID(node.gameObject.GetInstanceID());
				if (enemy != null)
				{
					//if (enemy.Level <= m_Level/* - 1*/)
					//{
					//	GetExp(enemy.Level);
					//	enemy.HandleMessage(new Msg_CollisionOccurred(playerCol).SetAsNoneExplode());
					//	listDelete.Add(node);
					//}
					//else
					//{
						enemy.HandleMessage(new Msg_LevelDown());
					//}
				}
			}

			YCameraManager.Instance.BossDefeated();

			foreach (Collider node in listDelete)
			{
				listCols.Remove(node);
			}
			
			yield return new WaitForSeconds(0.005f);

			Time.timeScale += 0.01f;

			if (m_Level <= 1)
				break;
		}

		while(true)
		{
			Time.timeScale += 0.1f;
			yield return new WaitForSeconds(0.2f);

			if (Time.timeScale > 1)
			{
				Time.timeScale = 1;
				break;
			}
		}
	}
    #endregion

    #region - obsolete -
    void Obsolete()
	{
		m_CurState += "";
	}
//	bool m_ShowUI = true;
//	void OnGUI()
//	{
////		if(m_ShowUI == true && m_Living == true)
////		{
////			Vector3 pos = Camera.mainCamera.WorldToScreenPoint(transform.position);
////			Rect rect = new Rect(pos.x - 60, Screen.height - pos.y, 120, 20);
//////			GUI.Label(rect, "[" + m_CurState.ToString() + "]");
//////			rect.y += 15;
////			GUI.Label(rect, "[Exp:" + m_Exp + "]");
////		}
//		
//		if(GUI.Button(new Rect(100, 20, 200, 100), "invincible") == true)
//		{
//			m_Invincible = true;
//		}
//	}
	#endregion
}