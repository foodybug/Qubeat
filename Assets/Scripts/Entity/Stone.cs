using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stone : YBaseEntity
{
	#region - property -
//	public override eEntityType EntityType{get{return eEntityType.Stone;}}
	#endregion
	#region - member -
	YCollider col_;
	YSpinner spinner_;
	YMover mover_;

	[SerializeField] string m_CurState;
	YStateMachine<Stone> m_StateMachine;
	#endregion
	
	#region - init & release & update -
	void Awake()
	{
		//state
		m_StateMachine = new YStateMachine<Stone>(this);
		m_StateMachine.RegisterState(new StoneState_Idle(m_StateMachine));
		m_StateMachine.RegisterState(new StoneState_Death(m_StateMachine));
		m_StateMachine.RegisterState(new StoneState_Sleep(m_StateMachine));
		
		RegisterReceiver(typeof(Msg_CollisionOccurred), OnRcvCollision);
		RegisterReceiver(typeof(Msg_PlayerLevelUp), OnPlayerLevelUp);
		RegisterReceiver(typeof(Msg_LevelDown), OnLevelDown);
		RegisterReceiver(typeof(Msg_PlayerLevelChanged), OnPlayerLevelChanged);

		ComponentContainer contain = GetComponent<ComponentContainer>();
		col_ = contain.col_;
		mover_ = contain.mover_;
		spinner_ = contain.spinner_;
	}	
	
	public override void Init()
	{
		base.Init();

		col_.Init();
		mover_.Init();
		spinner_.Init(); 
	}
	
//	void SetEntityData()
//	{
//		m_EntityStatus = Resources.Load("Asset/EntityStatus") as EntityStatus;
//		
//		m_Status = m_EntityStatus.birdStatus_;
//		
//		m_Hp = m_Status.hp_;
//		m_Attack = m_Status.attack_;
//		
//		m_MoveSpeed = m_Status.moveSpeed_;
//		m_Mover.SetMoveSpeed(m_MoveSpeed);
//	}
	
	public override void SetCreationData(YCreationData _data)
	{
		CreationData_Stone creation = _data as CreationData_Stone;
		
		name = GetType() + "(" + gameObject.GetInstanceID() + ")";
		
		m_Level = creation.lv_;
		
		transform.position = creation.pos_;
		
		AdjustVertices(m_Level, 0.25f);
		
		SetColor();
		
		gameObject.layer = LayerMask.NameToLayer("Stone");
		
		m_Id = gameObject.GetInstanceID();
	}

	public override void Init_AfterCreation()
	{
		col_.SetCollisionData(m_Level, new System.Type[] { typeof(Player) }, true);
		spinner_.SetRevision(0.2f);
	}
	
	IEnumerator Start ()
	{
		float standby = 2f;
		yield return new WaitForSeconds(standby);
		
		SetState(typeof(StoneState_Idle));
		
		SetEntityData();
		
//		InvokeRepeating("CheckPlayerLevelUp", 0, 0.5f);
	}
	
	void SetEntityData()
	{
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		m_Hp = status.stoneStatus_.hp_;
		
		HandleMessage(new Msg_Movement_SpeedModify(status.stoneStatus_.moveSpeed_));
	}
	
//	void CheckPlayerLevelUp()
//	{
//		if(m_Level <= YEntityManager.Instance.PlayerEntity.realLevel)
//		{
//			SetColor();
//		}
//	}
	
	void Update ()
	{
		m_StateMachine.Update();
	}
	
	void OnBecameInvisible()
	{
		SetState(typeof(StoneState_Sleep));
//		Debug.Log(gameObject.GetInstanceID() + " became invisible.");
	}
	
	void OnBecameVisible()
	{
		SetState(typeof(StoneState_Idle));
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
		
		if(YEntityManager.Instance.PlayerEntity.realLevel >= m_Level)
		{
			if(colFrom.col_.AttachedEntity.GetType() == typeof(Player))
				SetState(typeof(StoneState_Death));
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
		if (m_Level < 2)
			return;

		--m_Level;
		AdjustVertices(m_Level, 0.25f, true);

		if (m_Level == YEntityManager.Instance.PlayerEntity.realLevel)
		{
			SetColor();
			SetState(typeof(StoneState_Idle));
		}
	}

	void OnLevelDown(YMessage _msg)
	{
		if (m_Level <= 1)
		{
			//Debug.LogWarning("Stone:: OnLevelDown: invalid level = " + m_Level);
			SetState(typeof(StoneState_Death), _msg);
		}
		else
		{
			--m_Level;

			AdjustVertices(m_Level, 0.25f, true);
			HandleMessage(new Msg_CollisionSize());

			SetColor(true);
			SetState(typeof(StoneState_Idle));
		}
	}

	void OnPlayerLevelChanged(YMessage _msg)
	{
		SetColor();
		SetState(typeof(StoneState_Idle));
	}

	void SetColor(bool keep = false)
	{
		if(YEntityManager.Instance.PlayerEntity == null)
			return;
		
//		float upper = 1f;
//		float lower = 0.85f;
//		float maximum = 1f;
//		
//		Color color = Color.black;
		
		if(m_Level <= YEntityManager.Instance.PlayerEntity.realLevel)
		{
//			upper = 0.6f;
//			lower = 0f;
//			maximum = 1f;
//			
//			color = new Color(Random.Range(lower, upper),Random.Range(lower, upper),Random.Range(lower, upper));
//		
//			int seed = Random.Range(0, 3);
//			switch(seed)
//			{
//			case 0: color.r = maximum; break;
//			case 1: color.g = maximum; break;
//			case 2: color.b = maximum; break;
//			}
			
			if(keep == false)
			SetRandomColor();
		}
		else
		{
//			upper = 0.5f;
//			lower = 0.2f;
//			maximum = 0.3f;
//			
//			float intensity = Random.Range(lower, upper);
//			
//			color = new Color(intensity, intensity, intensity);
			
			SetBlackColor();
		}
		
//		GetComponent<Renderer>().material.color = color;
	}
	#endregion
//	#region - fsm operation -
//	#region - idle -
//	IEnumerator Idle_Roaming()
//	{
//		float refreshRate = Random.Range(3f, 4f);
//		float range = 10f;
//		
//		while(true)
//		{
//			Vector3 pos = YStageManager.Instance.GetRandomRange2DPosInStage(transform.position, range);
//			HandleMessage(new Msg_Movement_Start(pos));
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	
//	IEnumerator Idle_CheckPlayer()
//	{
//		float refreshRate = 0.5f;
//		float dist = 5f;
//		
//		while(true)
//		{
//			if(YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == true)
//			{
//				if(YEntityManager.Instance.PlayerEntity.realLevel < Level)
//					SetState(typeof(StoneState_Chase));
//				else
//					SetState(typeof(StoneState_Escape));
//				
//				break;
//			}
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	#endregion
//	#region - chase -
//	IEnumerator Chase_ChasePlayer()
//	{
//		float refreshRate = 2f;
//		float range = 3f;
//		
//		while(true)
//		{
//			if(YEntityManager.Instance.PlayerEntity == null)
//			{
//				yield return null;
//				continue;
//			}
//			
//			Vector3 playerPos = YEntityManager.Instance.PlayerEntity.transform.position;
//			
//			Vector3 dest = YStageManager.Instance.GetRandomRange2DPosInStage(playerPos, range);
//			
//			HandleMessage(new Msg_Movement_Start(dest));
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	
//	IEnumerator Chase_CheckPlayer()
//	{
//		float refreshRate = 1f;
//		float dist = 10f;
//		
//		while(true)
//		{
//			if(YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == false)
//			{
//				SetState(typeof(StoneState_Idle));
//				break;
//			}
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	#endregion
//	#region - escape -
//	IEnumerator Escape_EscapePlayer()
//	{
//		float refreshRate = 2f;
//		float range = 10f;
//		
//		while(true)
//		{
//			if(YEntityManager.Instance.PlayerEntity == null)
//			{
//				yield return null;
//				continue;
//			}
//			
//			Vector3 playerPos = YEntityManager.Instance.PlayerEntity.transform.position;
//			Vector3 dir = transform.position - playerPos;
//			Vector3 prevPos = transform.position + dir.normalized * 15f;
//			Vector3 dest = YStageManager.Instance.GetRandomRange2DPosInStage(prevPos, range);
//			HandleMessage(new Msg_Movement_Start(dest));
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	
//	IEnumerator Escape_CheckPlayer()
//	{
//		float refreshRate = 1f;
//		float dist = 10f;
//		
//		while(true)
//		{
//			if(YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == false)
//			{
//				SetState(typeof(StoneState_Idle));
//				break;
//			}
//			
//			yield return new WaitForSeconds(refreshRate);
//		}
//	}
//	#endregion
	IEnumerator Death_CR(YMessage _msg = null)
	{
		GetComponent<Renderer>().enabled = false;
		m_Living = false;
		
		HandleMessage(new Msg_Movement_Stop());
		YCameraManager.Instance.EntityDeath(this);

		if (_msg.GetType() == typeof(Msg_Blank))
			ExplosionManager.Instance.SetExplosion(transform, YEntityManager.Instance.PlayerEntity.transform, m_Color, (float)m_Level);
		//else
		//	ExplosionManager.Instance.SetExplosion(transform, YEntityManager.Instance.PlayerEntity.transform, m_Color, (float)m_Level);

		if (MainFlow.Instance != null)
			MainFlow.Instance.Sound_NormalDeath();
		
		yield return new WaitForSeconds(5);

		YEntityManager.Instance.RemoveEntity(Id, false);
		ReleaseReceiver();
	}
	
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

