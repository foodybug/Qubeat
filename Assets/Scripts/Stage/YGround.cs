using UnityEngine;
using System.Collections;

public class YGround : MonoBehaviour {

    int layerPlayer = 0;
    int layerGround = 0;

    void Awake()
    {
        layerPlayer = LayerMask.NameToLayer("Player");
        layerGround = LayerMask.NameToLayer("Ground");
    }

	void Start () {
        
        Vector2 size = YStageManager.Instance.StageSize;
		
		gameObject.name = "Ground";
		gameObject.layer = LayerMask.NameToLayer("Ground");
//		transform.localScale = new Vector3(size.x * 2, 0.01f, size.y * 2);
		transform.localScale = new Vector3(size.x * 2.2f, 0.01f, size.y * 2.2f);
		
		GetComponent<Renderer>().enabled = false;
		
		if(MainFlow.InputType == eInputType.Virtual_Pad)
			Instantiate(Resources.Load("Pad/Pad"));
	}
	
	Vector3 __dir; 
	Vector3 __pos;
	Vector3 __screenPos;
	Ray __ray;
	RaycastHit __hit;
	void Update () {
		
		if(MainFlow.Instance != null && MainFlow.Instance.SceneChanging == true)
			return;
		
		if( InGame.GamePaused == true)
			return;
		
		switch(MainFlow.InputType)
		{
		case eInputType.Virtual_Pad:
			if(YEntityManager.Instance.PlayerEntity != null)
			{
				Player player = YEntityManager.Instance.PlayerEntity;
				__dir = new Vector3(SimplePad.ratio.x, 0, SimplePad.ratio.y);
				__pos = __dir.normalized + player.transform.position;
				
				__screenPos = Camera.main.WorldToScreenPoint(__pos);
				__ray = Camera.main.ScreenPointToRay(__screenPos);
				if(Physics.Raycast(__ray, out __hit, float.MaxValue, 1 << layerGround) == true)
					YEntityManager.Instance.MessageToUserEntity(new Msg_Input_Move(__pos));
			}
			break;
		case eInputType.Click:
//			if(Input.GetMouseButton(0) == true)
			if( Input.GetButton("Fire1"))
			{
				__ray = Camera.main.ScreenPointToRay(GetScreenPosition());
				if(Physics.Raycast(__ray, out __hit, float.MaxValue, 1 << layerGround) == true)
				{
					if(__hit.transform.gameObject.layer != layerPlayer)
						YEntityManager.Instance.MessageToUserEntity(new Msg_Input_Move(__hit.point));
				}
			}
			break;
		}
	}
	
	Vector3 GetScreenPosition()
	{
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
            return Input.mousePosition;

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
            return new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

        return Input.mousePosition;
	}
}
