using UnityEngine;
using System.Collections;

public class StoneState_Sleep : YBaseState<Stone>
{
	public StoneState_Sleep(YStateMachine<Stone> _sm) : base (_sm)
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

