using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : YBaseEntity
{
	#region - property -
//	public override eEntityType EntityType{get{return eEntityType.Boss;}}
	#endregion
	#region - member -
	[SerializeField] string m_CurState;
	YStateMachine<Boss> m_StateMachine;

	Renderer[] m_Renderers;
	#endregion
	
	#region - init & release & update -
	void Awake()
	{
		//state
		m_StateMachine = new YStateMachine<Boss>(this);
		m_StateMachine.RegisterState(new BossState_Idle(m_StateMachine));
		m_StateMachine.RegisterState(new BossState_Death(m_StateMachine));
		
		RegisterReceiver(typeof(Msg_CollisionOccurred), OnRcvCollision);
		RegisterReceiver(typeof(Msg_PlayerLevelUp), OnPlayerLevelUp);

		m_Renderers = GetComponentsInChildren<Renderer>();
		if(m_Renderers == null)
			Debug.Log("Boss:: Start: no renderers is in child");
	}	
	
	public override void Init()
	{
		base.Init();
	}

	public override void SetCreationData(YCreationData _data)
	{
		CreationData_Boss creation = _data as CreationData_Boss;
		
		name = GetType() + "(" + gameObject.GetInstanceID() + ")";
		
		m_Level = creation.lv_;
		
		transform.position = creation.pos_;
		transform.localScale *= m_Level * 0.5f;
		
		SetColor();
		
		gameObject.layer = LayerMask.NameToLayer("Boss");
		
		m_Id = gameObject.GetInstanceID();
	}
	
	IEnumerator Start ()
	{
		float standby = 2f;
		yield return new WaitForSeconds(standby);
		
		SetState(typeof(BossState_Idle));
		
		SetEntityData();
		
		StartCoroutine(ColorChanging());
	}
	
	void SetEntityData()
	{
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		m_Hp = status.roamerStatus_.hp_;
		
		HandleMessage(new Msg_Movement_SpeedModify(status.roamerStatus_.moveSpeed_));
	}
	
	IEnumerator ColorChanging()
	{
		float lerp = 0;
		
		Color lowLv_Lower = new Color(0.1f, 0.1f, 0.1f);
		Color lowLv_Upper = new Color(0.9f, 0.9f, 0.9f);
		
		int colorSeed = 0;
		Color[] colorPool = new Color[3]{Color.red, Color.yellow, Color.green};
		
		float elapsedTime = 0;
		
		while(true)
		{
			yield return null;
			
			elapsedTime += Time.deltaTime;
			if(elapsedTime > 1)
			{
				elapsedTime = 0;
				++colorSeed;
			}
			
			lerp += Time.deltaTime * 1f;
			lerp %= 1f;
			
			if(m_Level > YEntityManager.Instance.PlayerEntity.realLevel)
				SetColorOnRenderers(Color.Lerp(lowLv_Lower, lowLv_Upper, lerp));
			else
				SetColorOnRenderers(Color.Lerp(colorPool[colorSeed % 3], colorPool[(colorSeed + 1) % 3], lerp));
		}
	}
	
	void Update ()
	{
		m_StateMachine.Update();
	}
	
	void OnBecameInvisible()
	{
		#region - tutorial -
		if(TutorialManager.Instance != null)
			TutorialManager.Instance.BossDisappear_TutorialManager();
		#endregion
	}
	
	void OnBecameVisible()
	{
		#region - tutorial -
		if(TutorialManager.Instance != null)
			TutorialManager.Instance.BossAppear_TutorialManager(this);
		#endregion
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
//		Debug.Log("Character/Bee::SetState: id=" + Id + " state changed to " + _state);
	}
	
	public void MessageToState(YMessage _msg)
	{
		m_StateMachine.MessageProcess(_msg);
	}
	#endregion
	#region - msg -
	void OnRcvCollision(YMessage _msg)
	{
		Msg_CollisionOccurred colFrom = _msg as Msg_CollisionOccurred;
		
		if(YEntityManager.Instance.PlayerEntity.realLevel >= m_Level && colFrom.levelUp_ == false)
		{
			if(colFrom.col_.AttachedEntity.GetType() == typeof(Player))
				SetState(typeof(BossState_Death));
				
		}
		else
		{
//			if(colFrom.col_.AttachedEntity.GetType() == typeof(Player))
//			{
//				colFrom.col_.AttachedEntity.HandleMessage(new Msg_PlayerDeath());
//			}
		}
	}
	
	void OnPlayerLevelUp(YMessage _msg)
	{
		--m_Level;
		if(m_Level == YEntityManager.Instance.PlayerEntity.realLevel)
		{
			SetColor();
			SetState(typeof(BossState_Idle));
		}
	}
	
	void SetColor()
	{
		if(YEntityManager.Instance.PlayerEntity == null)
			return;
		
		if(m_Level <= YEntityManager.Instance.PlayerEntity.realLevel)
		{
			SetColorOnRenderers(new Color(0.3f, 0.3f, 0.3f));
		}
		else
		{
			SetColorOnRenderers(new Color(0.1f, 0.1f, 0.1f));
		}
	}
	#endregion
	//	#region - fsm operation -
	#region - idle -
	IEnumerator Idle_Roaming_CR(YMessage _msg = null)
	{
		float refreshRate = Random.Range(3f, 4f);
		float range = 10f;
		
		while(true)
		{
			yield return new WaitForSeconds(refreshRate);
			
			Vector3 pos = YStageManager.Instance.GetRandomRange2DPosInStage(transform.position, range);
			HandleMessage(new Msg_Movement_Start(pos));
		}
	}

	IEnumerator Idle_Spinning_CR(YMessage _msg = null)
	{
		Vector3 rot = Random.insideUnitCircle * 10f;

        while (true)
        {
            yield return null;

            transform.eulerAngles += rot * Time.deltaTime;
        }
    }
	#endregion
	#region - escape -
	IEnumerator Escape_EscapePlayer_CR(YMessage _msg = null)
	{
		yield return null;
		
		float refreshRate = 2f;
		float range = 10f;
		
		while(true)
		{
			if(YEntityManager.Instance.PlayerEntity == null)
			{
				yield return null;
				continue;
			}
			
			Vector3 playerPos = YEntityManager.Instance.PlayerEntity.transform.position;
			Vector3 dir = transform.position - playerPos;
			Vector3 prevPos = transform.position + dir.normalized * 15f;
			Vector3 dest = YStageManager.Instance.GetRandomRange2DPosInStage(prevPos, range);
			HandleMessage(new Msg_Movement_Start(dest));
			
			yield return new WaitForSeconds(refreshRate);
		}
	}

	IEnumerator Escape_CheckPlayer_CR(YMessage _msg = null)
	{
		yield return null;
		
		float refreshRate = 1f;
		float dist = 10f;
		
		while(true)
		{
			if(YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == false)
			{
				SetState(typeof(BossState_Idle));
				break;
			}
			
			yield return new WaitForSeconds(refreshRate);
		}
	}
	#endregion

	#region - private -
	IEnumerator Death_CR(YMessage _msg = null)
	{
		EnableRenderers(false);
		m_Living = false;
		
		HandleMessage(new Msg_Movement_Stop());
		YCameraManager.Instance.EntityDeath(this);

		ExplosionManager.Instance.SetBossExplosion(
			transform.position, transform.position, m_CurColor, (float)m_Level);
		
		if(MainFlow.Instance != null)
		{
			MainFlow.Instance.Sound_BossDeath();
			MainFlow.Instance.StageCleared();
		}
		
		#region - turorial -
		if(TutorialManager.Instance != null)
			SendMessage("EntityDeath_TutorialDisplay", SendMessageOptions.DontRequireReceiver);
		#endregion

		YEntityManager.Instance.PlayerEntity.HandleMessage(new Msg_BossDefeated());
		//StartCoroutine(TimeProcess_Defeat());

        if (InGame.Instance != null)
        {
            InGame.Instance.StageClear();
        }

		yield return new WaitForSeconds(1);

		//Instantiate(Resources.Load("UI/StageClear_Text"));
		
		yield return new WaitForSeconds(2);
		
//		Destroy(obj);
		
		YEntityManager.Instance.RemoveEntity(Id, false);
		Destroy(gameObject);
	}
	
	//IEnumerator TimeProcess_Defeat()
	//{
	//	Time.timeScale = 0.1f;
		
	//	while(true)
	//	{
	//		yield return new WaitForSeconds(0.3f);
			
	//		Time.timeScale += 0.2f;
			
	//		if(Time.timeScale > 1)
	//		{
	//			Time.timeScale = 1;
	//			break;
	//		}
	//	}
	//}

	void EnableRenderers(bool _enable)
	{
		foreach(Renderer node in m_Renderers)
		{
			node.enabled = _enable;
		}
	}

	Color m_CurColor;
	void SetColorOnRenderers(Color _color)
	{
		m_Color = m_CurColor = _color;

		foreach(Renderer node in m_Renderers)
		{
			node.material.color = _color;
		}
	}
	#endregion

	#region - obsolete -
	void Obsolete()
	{
		m_CurState += "";
	}
//	#endregion
//	[SerializeField] bool showUI_ = true;
//	void OnGUI()
//	{
//		if(showUI_ == true)
//		{
//			Vector3 pos = Camera.mainCamera.WorldToScreenPoint(transform.position);
//			Rect rect = new Rect(pos.x - 60, Screen.height - pos.y, 200, 20);
//			GUI.Label(rect, "[state:" + m_CurState + "]");
//		}
//	}
	#endregion
}

