using UnityEngine;
using System.Collections;

public class SpecterState_Death : YBaseState<Specter>
{
	public SpecterState_Death(YStateMachine<Specter> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.HandleMessage(new Msg_CollisionActive(false));
		Owner.CoroutineProc("Death", true, _msg);
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
	}
}
