using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class StageBtn : MonoBehaviour {
	
	enum eState {Cleared, Enable, Disable}
	eState m_State = eState.Disable;
	
	int m_Index_; public int index{get{return m_Index_;}}
	
	public static bool s_Moving = false;
	
	void Awake()
	{
		m_Index_ = int.Parse(name.Split(new char[]{'_'})[0]);
	}
	
	void Start () {
		SendMessage("Init_Mover_Twinkle");
	}
	
	void Update () {
	
	}
	
//	void OnMouseButtonDown()
//	{
//		if(Application.isPlaying == false)
//		{
//			if(Input.GetMouseButton(0) == true)
//			{
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				RaycastHit hit;
//				if(Physics.Raycast(ray, out hit) == true)
//				{
//					GameObject obj = new GameObject((m_listStageBtn.Count + 1).ToString());
//					obj.AddComponent<StageBtn>();
//					obj.AddComponent<Mover_Twinkle>();
//					
//					transform.position = hit.point;
//				}
//			}
//		}
//	}
	
	IEnumerator OnMouseDown()
    {
		if(Stage_Select.EditMode == true)
		{
			s_Moving = true;
			
	        Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
	        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, scrSpace.z));
	
	        while (Input.GetMouseButton(0))
	        {
	            Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, scrSpace.z);
	
	            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
	
	            transform.position = curPosition;
						
	            yield return null;
	        }
			
//			m_InitPos = transform.position;
			SendMessage("Init_Mover_Twinkle");
			
			s_Moving = false;
		}
    }
	
	void OnMouseUpAsButton()
	{
		if(Stage_Select.EditMode == false && m_State != eState.Disable)
		{
			Invoke("_StageChange", 1f);

			GameObject.Find("Stage_Select").SendMessage("Stage_Select_StageSelected", index);
			ParticleSystem particle = (Instantiate(Resources.Load("Effect/Explode_Stage")) as GameObject).GetComponent<ParticleSystem>();
			particle.transform.position = transform.position;
			particle.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;

			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;

			Destroy(gameObject, 2f);

			SoundManager.Instance.PlayEffect("BossDeath");

//			StartCoroutine(MouseUpAsButton_CR());
		}
	}

	void _StageChange()
	{
		MainFlow.Instance.SetCurStageIdx(index);
		MainFlow.Instance.SceneConversion(index.ToString());
	}
	
	IEnumerator MouseUpAsButton_CR()
	{
		#region - spin control -
		SendMessage("ChangeAngleSpeed_Mover_Twinkle", 0f); 
		#endregion
		#region - size control -
		float speed = 0.7f;
		
		float minScale = 0.8f * transform.localScale.x;
		float maxScale = 1.2f * transform.localScale.x;
		
		while(true)
		{
			while(true)
			{
				yield return null;
				
				if( transform.localScale.x < minScale)
					break;
				
				transform.localScale -= Vector3.one * Time.deltaTime * speed;
			}
			
			while(true)
			{
				yield return null;
				
				if( transform.localScale.x > maxScale)
					break;
				
				transform.localScale += Vector3.one * Time.deltaTime * speed;
			}
		}
		#endregion
	}
	
	public void SetClearedStage()
	{
		m_State = eState.Cleared;
		
		transform.localScale *= 0.8f;
		
		float total = 2f;
		float r = Random.Range(0f, 1f);
		float g = Random.Range(0f, 1f);
		float b = total - r - g;
		
		GetComponent<Renderer>().material.color = new Color(r, g, b);
	}
	
	public void SetEnableStage()
	{
		m_State = eState.Enable;
		
		GetComponent<Renderer>().material.color = Color.white;
		
		SendMessage("ChangeAngleSpeed_Mover_Twinkle", 180f);
		StartCoroutine(ColorChangeInEnable());
	}
	
	IEnumerator ColorChangeInEnable()
	{
		float lerp = 0;
		
		Color[] colorPool = new Color[]{Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white};
		
//		int curColorIdx = 0;
//		Color curColor = colorPool[curColorIdx];
//		int nextColorIdx = Random.Range(0, colorPool.Length);
//		Color destColor = colorPool[nextColorIdx];
		
		Color curColor = colorPool[Random.Range(0, colorPool.Length)];
		Color destColor = colorPool[Random.Range(0, colorPool.Length)];
		
		while(true)
		{
			yield return null;
			
			lerp += Time.deltaTime * 1.3f;
			if(lerp > 0.9f)
			{
				lerp = 0.1f;
				
				curColor = destColor;
				destColor = colorPool[Random.Range(0, colorPool.Length)];
				
//				int tempCurColorIdx = curColorIdx;
//				curColorIdx = nextColorIdx;
//				curColor = destColor;
//				nextColorIdx = (curColorIdx + Random.Range(0, colorPool.Length - 2)) % colorPool.Length;
//				destColor = colorPool[nextColorIdx];
			}
			
			GetComponent<Renderer>().material.color = Color.Lerp(curColor, destColor, lerp);
		}
	}
	
	public void SetDisableStage()
	{
		m_State = eState.Disable;
		
		transform.localScale *= 0.6f;
		
		float colorValue = 0.2f;
		GetComponent<Renderer>().material.color = new Color(colorValue, colorValue, colorValue);
		
		NumberDisplay numberDisplay = GetComponent<NumberDisplay>();
		if(numberDisplay != null)
			Destroy(numberDisplay);
	}
	
//	void OnGUI()
//	{
//		Vector3 pos = Camera.mainCamera.WorldToScreenPoint(transform.position);
//		float width = 20f;
//		float height = 30f;
//		GUI.Label(new Rect(pos.x - width * 0.5f, Screen.height - (pos.y + height), width, height), name);
//	}
}
