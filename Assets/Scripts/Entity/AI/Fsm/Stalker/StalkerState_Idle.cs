using UnityEngine;
using System.Collections;

public class StalkerState_Idle : YBaseState<Stalker>
{
	public StalkerState_Idle(YStateMachine<Stalker> _sm) : base (_sm)
	{
//		m_dicStateMessageDelegate.Add(typeof(Msg_CollisionOccurred), OnRcvCollision);
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", true);
		Owner.CoroutineProc("Idle_CheckPlayer", true);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.CoroutineProc("Idle_Roaming", false);
		Owner.CoroutineProc("Idle_CheckPlayer", false);
	}
	
	void OnRcvCollision(YMessage _msg)
	{
		Msg_CollisionOccurred colFrom = _msg as Msg_CollisionOccurred;
		
		if(YEntityManager.Instance.PlayerEntity.realLevel >= Owner.Level)
		{	
			if(colFrom.col_.AttachedEntity.GetType() == typeof(Player))
				Owner.SetState(typeof(StalkerState_Death));
		}
		else
		{
		}
	}
}

