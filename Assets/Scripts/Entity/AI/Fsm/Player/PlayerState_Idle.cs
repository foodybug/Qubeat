using UnityEngine;
using System.Collections;

public class PlayerState_Idle : YBaseState<Player>
{
	public PlayerState_Idle(YStateMachine<Player> _sm) : base (_sm)
	{
		m_dicStateMessageDelegate.Add(typeof(Msg_Input_Move), OnInputMove);
		m_dicStateMessageDelegate.Add(typeof(Msg_CollisionOccurred), OnRcvCollision);
	}
	
	public override void Enter(YMessage _msg)
	{
	}
	
	public override void Update()
	{
	}
	
	public override void Exit(YMessage _msg)
	{
	}
	
	void OnInputMove(YMessage _msg)
	{
		Msg_Input_Move input = _msg as Msg_Input_Move;
		Owner.HandleMessage(new Msg_Movement_Start(input.pos_));
	}
	
	void OnRcvCollision(YMessage _msg)
	{
        //if (Owner.Invincible == true)
        //    return;

        Msg_CollisionOccurred col = _msg as Msg_CollisionOccurred;
		
		int colLevel = col.col_.AttachedEntity.Level;
		if(Owner.Level < colLevel && Owner.Invincible == false)
		{
//#if UNITY_EDITOR
//            Debug.Log("col.col_.name = " + col.col_.name);
//            UnityEditor.EditorApplication.isPaused = true;
//#endif

            Owner.SetState(typeof(PlayerState_Death));
		}
		else
		{
			Owner.GetExp(colLevel, col);
			Owner.HandleMessage(new Msg_Spin_AngleModify());
		}
	}
}

