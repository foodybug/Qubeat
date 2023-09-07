using UnityEngine;
using System.Collections;

public class BulletState_Idle : YBaseState<Bullet>
{
	public BulletState_Idle(YStateMachine<Bullet> _sm) : base (_sm)
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
				Owner.SetState(typeof(BulletState_Death));
		}
		else
		{
		}
	}
}

