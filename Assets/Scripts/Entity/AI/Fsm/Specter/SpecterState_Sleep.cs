using UnityEngine;
using System.Collections;

public class SpecterState_Sleep : YBaseState<Specter>
{
	public SpecterState_Sleep(YStateMachine<Specter> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.HandleMessage(new Msg_Spin_Stop());
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
		Owner.HandleMessage(new Msg_Spin_Start());
	}
}
