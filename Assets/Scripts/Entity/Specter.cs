using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Specter : YBaseEntity
{
	#region - property -
	#endregion
	#region - member -
	YCollider col_;
	YSpinner spinner_;
	YMover mover_;

	[SerializeField] string m_CurState;
	YStateMachine<Specter> m_StateMachine;
	
	Vector3 m_Direction;
	#endregion
	
	#region - init & release & update -
	void Awake()
	{
		// state
		m_StateMachine = new YStateMachine<Specter>(this);
		m_StateMachine.RegisterState(new SpecterState_Idle(m_StateMachine));
		m_StateMachine.RegisterState(new SpecterState_Chase(m_StateMachine));
		m_StateMachine.RegisterState(new SpecterState_Escape(m_StateMachine));
		m_StateMachine.RegisterState(new SpecterState_Death(m_StateMachine));
		m_StateMachine.RegisterState(new SpecterState_Sleep(m_StateMachine));
		
		RegisterReceiver(typeof(Msg_CollisionOccurred), OnRcvCollision);
		RegisterReceiver(typeof(Msg_PlayerLevelUp), OnPlayerLevelUp);
		RegisterReceiver(typeof(Msg_LevelDown), OnLevelDown);
		RegisterReceiver(typeof(Msg_PlayerLevelChanged), OnPlayerLevelChanged);

		ComponentContainer contain = GetComponent<ComponentContainer>();
		if (contain != null)
		{
			col_ = contain.col_;
			mover_ = contain.mover_;
			spinner_ = contain.spinner_;
		}
	}	
	
	public override void Init()
	{
		base.Init();

		if (col_ != null) col_.Init();
		if (mover_ != null) mover_.Init();
		if (spinner_ != null) spinner_.Init();
	}
	
	public override void SetCreationData(YCreationData _data)
	{
		CreationData_Specter creation = _data as CreationData_Specter;
		
		name = GetType() + "(" + gameObject.GetInstanceID() + ")";
		
		m_Level = creation.lv_;
		
		transform.position = creation.pos_;
		
		AdjustVertices(m_Level, 0.25f);
		
		SetColor();
		
		int specterLayer = LayerMask.NameToLayer("Specter");
		if (specterLayer != -1)
			gameObject.layer = specterLayer;
		else
			gameObject.layer = LayerMask.NameToLayer("Stalker");
		
		m_Id = gameObject.GetInstanceID();
	}

	public override void Init_AfterCreation()
	{
		if (col_ != null) col_.SetCollisionData(m_Level, new System.Type[] { typeof(Player) }, true);
		if (spinner_ != null) spinner_.SetRevision(0.2f);
	}
	
	IEnumerator Start ()
	{
		float standby = 2f;
		yield return new WaitForSeconds(standby);
		
		SetState(typeof(SpecterState_Idle));
		
		SetEntityData();

		StartCoroutine(Fade_CR());
	}
	
	void SetEntityData()
	{
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		if (status != null && status.specterStatus_ != null)
		{
			m_Hp = status.specterStatus_.hp_;
			// Stalker보다 느린 이동 속도 설정 (0.6x)
			float speed = status.specterStatus_.moveSpeed_ * 0.6f;
			HandleMessage(new Msg_Movement_SpeedModify(speed));
		}
		else
		{
			m_Hp = 100;
			HandleMessage(new Msg_Movement_SpeedModify(0.6f));
		}
	}
	
	void Update ()
	{
		m_StateMachine.Update();
	}
	
	void OnBecameInvisible()
	{
		SetState(typeof(SpecterState_Sleep));
	}
	
	void OnBecameVisible()
	{
		SetState(typeof(SpecterState_Idle));
	}
	#endregion

	#region - fade (투명화 기능) -
	IEnumerator Fade_CR()
	{
		Renderer ren = GetComponent<Renderer>();
		if (ren == null)
			yield break;

		float duration = 2.5f; // 투명해지고 원래대로 돌아오는 반주기 시간

		while (m_Living)
		{
			// 1. 천천히 투명해짐 (alpha: 1.0 -> 0.1)
			float timer = 0f;
			while (timer < duration && m_Living)
			{
				timer += Time.deltaTime;
				float alpha = Mathf.Lerp(1.0f, 0.1f, timer / duration);
				if (ren != null && ren.material != null)
				{
					Color col = ren.material.color;
					col.a = alpha;
					ren.material.color = col;
				}
				yield return null;
			}

			// 2. 천천히 원래 색으로 돌아옴 (alpha: 0.1 -> 1.0)
			timer = 0f;
			while (timer < duration && m_Living)
			{
				timer += Time.deltaTime;
				float alpha = Mathf.Lerp(0.1f, 1.0f, timer / duration);
				if (ren != null && ren.material != null)
				{
					Color col = ren.material.color;
					col.a = alpha;
					ren.material.color = col;
				}
				yield return null;
			}
		}
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
		
		if (YEntityManager.Instance != null && YEntityManager.Instance.PlayerEntity != null)
		{
			if (YEntityManager.Instance.PlayerEntity.realLevel >= m_Level)
			{
				if (colFrom.col_.AttachedEntity.GetType() == typeof(Player))
					SetState(typeof(SpecterState_Death));
			}
		}
	}
	
	void OnPlayerLevelUp(YMessage _msg)
	{
		if (m_Level < 2)
			return;

		--m_Level;
		AdjustVertices(m_Level, 0.25f, true);

		if (YEntityManager.Instance != null && YEntityManager.Instance.PlayerEntity != null && m_Level == YEntityManager.Instance.PlayerEntity.realLevel)
		{
			SetColor();
			SetState(typeof(SpecterState_Idle));
		}
	}

	void OnLevelDown(YMessage _msg)
	{
		if (m_Level == 1)
		{
			SetState(typeof(SpecterState_Death), _msg);
		}
		else
		{
			--m_Level;

			AdjustVertices(m_Level, 0.25f, true);
			HandleMessage(new Msg_CollisionSize());

			SetColor(true);
			SetState(typeof(SpecterState_Idle));
		}
	}

	void OnPlayerLevelChanged(YMessage _msg)
	{
		SetColor();
		SetState(typeof(SpecterState_Idle));
	}

	void SetColor(bool keep = false)
	{
		if (YEntityManager.Instance == null || YEntityManager.Instance.PlayerEntity == null)
			return;
		
		if (m_Level <= YEntityManager.Instance.PlayerEntity.realLevel)
		{
			if (keep == false)
				SetRandomColor();
		}
		else
		{
			SetBlackColor();
		}
	}
	#endregion

	#region - idle -
	public void Idle_Roaming()
	{
		StartCoroutine(Idle_Roaming_CR());
	}

	IEnumerator Idle_Roaming_CR()
	{
		yield return null;
		
		float refreshRate = Random.Range(3f, 4f);
		float range = 10f;
		
		while (true)
		{
			Vector3 pos = YStageManager.Instance.GetRandomRange2DPosInStage(transform.position, range);
			HandleMessage(new Msg_Movement_Start(pos));
			
			m_Direction = pos - transform.position;
			m_Direction.Normalize();
			
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void Idle_CheckPlayer()
	{
		StartCoroutine(Idle_CheckPlayer_CR());
	}

	IEnumerator Idle_CheckPlayer_CR()
	{
		yield return null;
		
		float refreshRate = 0.5f;
		float dist = 5f;
		
		while (true)
		{
			if (YEntityManager.Instance != null && YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == true)
			{
				if (YEntityManager.Instance.PlayerEntity != null && YEntityManager.Instance.PlayerEntity.realLevel < Level)
					SetState(typeof(SpecterState_Chase));
				else
					SetState(typeof(SpecterState_Escape));
				
				break;
			}
			
			yield return new WaitForSeconds(refreshRate);
		}
	}
	#endregion

	#region - chase -
	public void Chase_ChasePlayer()
	{
		StartCoroutine(Chase_ChasePlayer_CR());
	}

	IEnumerator Chase_ChasePlayer_CR()
	{
		float refreshRate = 2f;
		float steerAngle = 20f;
		
		while (true)
		{
			if (YEntityManager.Instance == null || YEntityManager.Instance.PlayerEntity == null)
			{
				yield return null;
				continue;
			}
			
			Vector3 dest;
			
			Vector3 playerPos = YEntityManager.Instance.PlayerEntity.transform.position;
			Vector3 dir = playerPos - transform.position;
			dir.Normalize();
			
			float angle = Vector3.Angle(m_Direction, dir);
			if (angle < steerAngle)
			{
				dest = playerPos;
			}
			else
			{
				m_Direction = Vector3.Lerp(m_Direction, dir, steerAngle / angle);
				dest = m_Direction * 20f;
			}
			
			HandleMessage(new Msg_Movement_Start(dest));
			
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void Chase_CheckPlayer()
	{
		StartCoroutine(Chase_CheckPlayer_CR());
	}

	IEnumerator Chase_CheckPlayer_CR()
	{
		yield return null;
		
		float refreshRate = 1f;
		float dist = 10f;
		
		while (true)
		{
			if (YEntityManager.Instance != null && YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == false)
			{
				SetState(typeof(SpecterState_Idle));
				break;
			}
			
			yield return new WaitForSeconds(refreshRate);
		}
	}
	#endregion

	#region - escape -
	IEnumerator Escape_EscapePlayer_CR()
	{
		float refreshRate = 2f;
		float range = 10f;
		
		while (true)
		{
			if (YEntityManager.Instance == null || YEntityManager.Instance.PlayerEntity == null)
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
	
	IEnumerator Escape_CheckPlayer_CR()
	{
		yield return null;
		
		float refreshRate = 1f;
		float dist = 10f;
		
		while (true)
		{
			if (YEntityManager.Instance != null && YEntityManager.Instance.CheckPlayerInRange(transform.position, dist) == false)
			{
				SetState(typeof(SpecterState_Idle));
				break;
			}
			
			yield return new WaitForSeconds(refreshRate);
		}
	}
	#endregion

	IEnumerator Death_CR(YMessage _msg = null)
	{
		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().enabled = false;
		m_Living = false;
		
		HandleMessage(new Msg_Movement_Stop());
		if (YCameraManager.Instance != null)
			YCameraManager.Instance.EntityDeath(this);

		if (_msg != null && _msg.GetType() == typeof(Msg_Blank) && ExplosionManager.Instance != null && YEntityManager.Instance.PlayerEntity != null)
			ExplosionManager.Instance.SetExplosion(transform, YEntityManager.Instance.PlayerEntity.transform, m_Color, (float)m_Level);

		if (MainFlow.Instance != null)
			MainFlow.Instance.Sound_NormalDeath();
		
		yield return new WaitForSeconds(5);
		
		if (YEntityManager.Instance != null)
			YEntityManager.Instance.RemoveEntity(Id, false);
		ReleaseReceiver();
	}
}
