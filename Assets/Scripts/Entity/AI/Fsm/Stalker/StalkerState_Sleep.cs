using UnityEngine;
using System.Collections;

public class StalkerState_Sleep : YBaseState<Stalker>
{
	public StalkerState_Sleep(YStateMachine<Stalker> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
//		Owner.gameObject.SetActive(false);
		Owner.HandleMessage(new Msg_Spin_Stop());
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
//		Owner.gameObject.SetActive(true);
		Owner.HandleMessage(new Msg_Spin_Start());
	}
}

