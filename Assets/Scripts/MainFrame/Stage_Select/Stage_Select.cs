//#define _EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class Stage_Select : MonoBehaviour {
	
	[SerializeField] List<StageBtn> m_listStageBtn;
	static EntityStatus m_Status;
//	static int s_LastStageIdx = 0; public static int LastStageIdx{get{return s_LastStageIdx;}}
	static bool s_EditMode = false; public static bool EditMode{get{return s_EditMode;}}
	[SerializeField] bool m_EditMode = false;
//	[SerializeField] float m_LineWidth = 0.05f;
	
//	LineRenderer m_Line;
	Dictionary<int, GameObject> m_dicLine = new Dictionary<int, GameObject>();
	[SerializeField] Vector3 m_LineScale = new Vector3(0.01f, 0.01f, 0.01f);
	
	void Start()
	{
		if(Application.isPlaying == false)
			return;
		
		#region - init stage btns -
		m_listStageBtn.Clear();
		foreach(Transform trn in transform)
		{
			m_listStageBtn.Add(trn.GetComponent<StageBtn>());
		}
		
		m_listStageBtn.Sort(delegate(StageBtn _a, StageBtn _b) {
			if(_a.index < _b.index)
				return -1;
			else if(_a.index == _b.index)
			{
//				Debug.LogError("Stage_Select::Start: same index defined[" + _a.name);
				return 0;
			}
			else
				return 1;
		});
		
		m_Status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		
		bool cameraSet = false;
		int idx = -1;
		foreach(StageBtn stageBtn in m_listStageBtn)
		{
			stageBtn.transform.position = m_Status.stageBtnPos_[++idx];
			stageBtn.SendMessage("Init_Mover_Twinkle");
			
			if(stageBtn.index <= PlayerPrefs.GetInt("ClearedStageIdx", 0))
				stageBtn.SetClearedStage();
			
			if(stageBtn.index == PlayerPrefs.GetInt("ClearedStageIdx", 0) + 1)
			{
				stageBtn.SetEnableStage();
				
				// camera pos setting
				Vector3 pos = stageBtn.transform.position;
				pos.z = Camera.main.transform.position.z;
				Camera.main.transform.position = pos;
				
				cameraSet = true;
			}
			
			if(stageBtn.index > PlayerPrefs.GetInt("ClearedStageIdx", 0) + 1)
				stageBtn.SetDisableStage();
		}
		
		if(cameraSet == false)
		{
			StageBtn stageBtn = m_listStageBtn[m_listStageBtn.Count - 1];
			
			Vector3 pos = stageBtn.transform.position;
			pos.z = Camera.main.transform.position.z;
			Camera.main.transform.position = pos;
		}
		#endregion
		#region - init line among stage btns -
		for(int i=0; i<m_listStageBtn.Count - 1; ++i)
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			obj.transform.localScale = m_LineScale;
			m_dicLine.Add(i, obj);
		}
		
		InvokeRepeating("LineRefresh", 0f, 0.05f);
		#endregion
		
//		Invoke("ShowBanner", 0.5f);
		
//		ShowBanner();
	}
	
	void ShowBanner()
	{
		if( AdvertisementManager.Instance != null)
			AdvertisementManager.Instance.Show_Banner();
	}
	
	void LineRefresh()
	{
		for(int i=0; i<m_listStageBtn.Count - 1; ++i)
		{
			StageBtn btn1 = m_listStageBtn[i];
			StageBtn btn2 = m_listStageBtn[i + 1];
			
			GameObject obj = null;
			if(m_dicLine.ContainsKey(i) == true)
				obj = m_dicLine[i];

			if(btn1 == null || btn2 == null || obj == null)
				continue;
			
			Vector3 scale = obj.transform.localScale;
			scale.z = Vector3.Distance(btn1.transform.position, btn2.transform.position);
			obj.transform.localScale = scale;
			
			obj.transform.position = (btn1.transform.position + btn2.transform.position) * 0.5f;
			
			obj.transform.LookAt(btn1.transform);
		}
	}
	
	void Update()
	{
		s_EditMode = m_EditMode;

        if(Input.GetKeyUp(KeyCode.Escape))
            MainFlow.Instance.SceneConversion("Title");
    }
	
	void OnDisable()
	{
		if(m_EditMode == true)
		{
			int idx = -1;
			m_Status.stageBtnPos_ = new Vector3[m_listStageBtn.Count];
			foreach(StageBtn btn in m_listStageBtn)
			{
//				btn.gameObject.SendMessage("Mover_Twinkle_Release");
				m_Status.stageBtnPos_[++idx] = btn.transform.position;
				
#if _EDITOR
				UnityEditor.EditorUtility.SetDirty(m_Status);
#endif
			}
		}
	}

	void Stage_Select_StageSelected(int _idx)
	{
		List<int> removeList = new List<int>();

		StageBtn btn = m_listStageBtn[_idx - 1];

		for(int i=_idx - 2; i<_idx; ++i)
		{
			if(m_dicLine.ContainsKey(i) == true)
			{
				GameObject obj = m_dicLine[i];
				SimpleMover mover = obj.AddComponent<SimpleMover>();
				mover.Init((mover.transform.position - btn.transform.position).normalized * Random.Range(1f, 2f));
				SimpleSpinner spinner = obj.AddComponent<SimpleSpinner>();
				spinner.Init(Random.rotation.eulerAngles);

				removeList.Add(i);
			}
		}

		foreach(int node in removeList)
		{
			m_dicLine.Remove(node);
		}
	}
	
//	void OnGUI()
//	{
//		int width = 100;
//		int height = 40;
//		
//		if(GUI.Button(new Rect(0, Screen.height - width, width, height), "save position") == true)
//		{
//			foreach(StageBtn btn in m_listStageBtn)
//			{
//				UnityEditor.EditorUtility.SetDirty(btn);
//			}
//		}
//	}
	
//	public static void StageSelected(int _idx)
//	{
//		if(m_Status.clearedStage[_idx - 1] == true || _idx == s_EnableStageIdx + 1)
//		{
//			MainFlow.Instance.SetCurStageIdx(_idx);
//			MainFlow.Instance.SceneConversion(_idx.ToString());
//		}
//	}
}
