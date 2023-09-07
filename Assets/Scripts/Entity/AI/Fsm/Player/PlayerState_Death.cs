using UnityEngine;
using System.Collections;

public class PlayerState_Death : YBaseState<Player>
{
	public PlayerState_Death(YStateMachine<Player> _sm) : base (_sm)
	{
	}
	
	public override void Enter(YMessage _msg)
	{
		Owner.HandleMessage(new Msg_CollisionActive(false));
		Owner.HandleMessage(new Msg_PlayerDeath());
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
	}
}

