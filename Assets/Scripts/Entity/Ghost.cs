using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : YBaseEntity
{
	#region - property -
//	public override eEntityType EntityType{get{return eEntityType.Ghost;}}
	#endregion
	#region - member -
	[SerializeField] string m_CurState;
	YStateMachine<Ghost> m_StateMachine;
	
	float m_Speed;
	#endregion
	
	#region - init & release & update -
	void Awake()
	{
		//state
		m_StateMachine = new YStateMachine<Ghost>(this);
		m_StateMachine.RegisterState(new GhostState_Dash(m_StateMachine));
		m_StateMachine.RegisterState(new GhostState_Idle(m_StateMachine));
		m_StateMachine.RegisterState(new GhostState_Death(m_StateMachine));
		
		RegisterReceiver(typeof(Msg_CollisionOccurred), OnRcvCollision);
		RegisterReceiver(typeof(Msg_PlayerLevelUp), OnPlayerLevelUp);
	}	
	
	public override void Init()
	{
		base.Init();
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
		CreationData_Ghost creation = _data as CreationData_Ghost;
		
		name = GetType() + "(" + gameObject.GetInstanceID() + ")";
		
		m_Level = 11;
		m_Speed = creation.speed_;
		
		transform.position = creation.pos_;
		transform.localScale *= m_Level * 0.2f;
		
		gameObject.layer = LayerMask.NameToLayer("Ghost");
		
		m_Id = gameObject.GetInstanceID();
	}
	
	void Start ()
	{
		if(YEntityManager.Instance.PlayerEntity != null)
		{
			SetEntityData();
			
			StartCoroutine(ColorChanging());
			
			#region - laser -
			GameObject laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
			laser.AddComponent<Laser>();
			Destroy(laser.GetComponent<Collider>());
			laser.transform.position = transform.position;
//			laser.renderer.material.color = renderer.material.color;
			#endregion
			
			SetState(typeof(GhostState_Dash), new Msg_GhostMemory(YEntityManager.Instance.PlayerEntity.transform.position, m_Speed));
		}
		else
		{
			SetState(typeof(GhostState_Idle), new Msg_GhostMemory(YEntityManager.Instance.PlayerEntity.transform.position, m_Speed));
		}
		
		StartCoroutine(ColorChanging());
	}
	
	void SetEntityData()
	{
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		m_Hp = status.roamerStatus_.hp_;
		
//		HandleMessage(new Msg_Movement_SpeedModify(m_Speed));
	}
	
	IEnumerator ColorChanging()
	{
		float lerp = 0;
		
//		Color lowLv_Lower = new Color(0.4f, 0.4f, 0.4f);
//		Color lowLv_Upper = new Color(0.6f, 0.6f, 0.6f);
		Color highLv_Lower = new Color(0.4f, 0.4f, 0.4f);
		Color highLv_Upper = new Color(0.95f, 0.95f, 0.95f);
		
		bool uping = true;
		
		while(true)
		{
			yield return null;
			
			if(lerp > 1)
				uping = false;
			if(lerp < 0)
				uping = true;
			
			float delta = Time.deltaTime * 1f;
			if(uping == false)
				delta *= -1f;
			
			lerp += delta;
			
//			if(m_Level <= YEntityManager.Instance.PlayerEntity.realLevel)
//				renderer.material.color = Color.Lerp(lowLv_Lower, lowLv_Upper, lerp);
//			else
				GetComponent<Renderer>().material.color = Color.Lerp(highLv_Lower, highLv_Upper, lerp);
		}
	}
	
	void Update ()
	{
		m_StateMachine.Update();
	}
	
//	void OnBecameInvisible()
//	{
//		SetState(typeof(GhostState_Sleep));
////		Debug.Log(gameObject.GetInstanceID() + " became invisible.");
//	}
//	
//	void OnBecameVisible()
//	{
//		SetState(typeof(GhostState_Idle));
//	}
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
//		Msg_CollisionOccurred colFrom = _msg as Msg_CollisionOccurred;
//		
//		if(YEntityManager.Instance.PlayerEntity.realLevel >= m_Level)
//		{
//			if(colFrom.col_.AttachedEntity.GetType() == typeof(Player))
//				SetState(typeof(GhostState_Death));
//		}
//		else
//		{
//		}
	}
	
	void OnPlayerLevelUp(YMessage _msg)
	{
//		if(m_Level == YEntityManager.Instance.PlayerEntity.realLevel)
//		{
//			SetColor();
//			SetState(typeof(GhostState_Idle));
//		}
	}
	#endregion
//	#region - fsm operation -
	#region - dash -
	IEnumerator Dash_Moving_CR(YMessage _msg = null)
	{
		yield return new WaitForSeconds(2f);
		
		Msg_GhostMemory memo = _msg as Msg_GhostMemory;
		float speed = 50f;
		HandleMessage(new Msg_Movement_SpeedModify(speed));
		
		Vector3 dir = memo.pos_ - transform.position;
		dir.Normalize();
		dir *= 999f;
		
		HandleMessage(new Msg_Movement_Start(dir + transform.position));
		
		while(true)
		{
			yield return null;
			
			if(YStageManager.Instance.CheckPosInPlane(transform.position) == false)
			{
				SetState(typeof(GhostState_Idle), _msg);
				break;
			}
		}
	}
	#endregion
	#region - idle -
	IEnumerator Idle_Roaming_CR(YMessage _msg = null)
	{
		Msg_GhostMemory memo = _msg as Msg_GhostMemory;
		HandleMessage(new Msg_Movement_SpeedModify(memo.speed_));
		HandleMessage(new Msg_Movement_Stop());
		
		float refreshRate = 2f;
		float range = 3f;
		
		while(true)
		{
			yield return new WaitForSeconds(refreshRate);
			
			Player player = YEntityManager.Instance.PlayerEntity;
			
			if(player != null && player.Living == true)
			{
				Vector3 pos = YStageManager.Instance.GetRandomRange2DPosInStage(player.transform.position, range);
				HandleMessage(new Msg_Movement_Start(pos));
			}
			else
			{
				Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage();
				HandleMessage(new Msg_Movement_Start(pos));
			}
		}
	}
	#endregion
	IEnumerator Death_CR(YMessage _msg = null)
	{
		GetComponent<Renderer>().enabled = false;
		m_Living = false;
		
		HandleMessage(new Msg_Movement_Stop());
		YCameraManager.Instance.EntityDeath(this);
		
		GameObject obj = Instantiate(Resources.Load("Effect/Explode")) as GameObject;
		obj.transform.position = transform.position;
		obj.GetComponent<ParticleSystem>().startColor = GetComponent<Renderer>().material.color;
		obj.GetComponent<ParticleSystem>().loop = false;
		obj.GetComponent<ParticleSystem>().startSpeed = 10f + m_Level * 0.9f;
		obj.GetComponent<ParticleSystem>().emissionRate = 300 * m_Level;
		
		yield return new WaitForSeconds(3);
		
		Destroy(obj);
		
		YEntityManager.Instance.RemoveEntity(Id, false);
		Destroy(gameObject);
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

