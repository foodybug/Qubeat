using UnityEngine;
using System.Collections;

public class RoamerState_Sleep : YBaseState<Roamer>
{
	public RoamerState_Sleep(YStateMachine<Roamer> _sm) : base (_sm)
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

