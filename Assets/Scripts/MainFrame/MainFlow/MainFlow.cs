//#define _EDITOR

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum eInputType {Click, Virtual_Pad}
public enum eMainFlowState {Developer_Mark, Title, Stage_Select, In_Game}

public class MainFlow : MonoBehaviour {
	
	#region - singleton -
	static MainFlow instance; public static MainFlow Instance{get{return instance;}}
	#endregion
	#region - member -
	SoundManager soundManager;

	[SerializeField] Text musicName_;
	
	eMainFlowState s_MainFlowState = eMainFlowState.Developer_Mark; public eMainFlowState MainFlowState{get{return s_MainFlowState;}}
	int m_SelectedStageIdx = 1; public int SelectedStageIdx{get{return m_SelectedStageIdx;}}
	
	EntityStatus m_Status;
	
	bool m_SceneChanging = false; public bool SceneChanging{get{return m_SceneChanging;}}
	
	[SerializeField] eInputType inputType_ = eInputType.Click;
	static eInputType s_InputType = eInputType.Virtual_Pad; public static eInputType InputType{get{return s_InputType;}}
	
	[SerializeField] int m_EditStageIdx = 0;
	[SerializeField] bool m_Reset = false;
	[SerializeField] bool m_Invincible = false;

	//[SerializeField] int m_BlurIteration = 10;
	//[SerializeField] float m_GlowIntensity = 0.25f;
	#endregion
	#region - init & update -
	void Awake()
	{
		if( m_EditStageIdx != 0)
			PlayerPrefs.SetInt("ClearedStageIdx", m_EditStageIdx);
		
		if( m_Reset == true)
		{
			PlayerPrefs.SetInt("ClearedStageIdx", 0);
			m_Reset = false;
		}
		
		instance = this;
		s_InputType = inputType_;
		
		DontDestroyOnLoad(this);
		DontDestroyOnLoad(musicName_.gameObject.transform.parent);
		
		m_Status = Resources.Load("Asset/EntityStatus") as EntityStatus;
	
		//if(Application.platform != RuntimePlatform.WindowsEditor)
		//	Debug.LogWarning("Invincible mode is activated");

//		if(Application.platform == RuntimePlatform.Android)
			MainFlow.Instance.SetInput(eInputType.Virtual_Pad);
	}
	
	IEnumerator Start () {
		
		yield return null;
		
		Application.LoadLevel("Developer_Mark");
		
		yield return new WaitForSeconds(1);

		soundManager = SoundManager.Instance;
		soundManager.PlayBgm("Main Theme");
		
		StartCoroutine(MusicSpeedControl());
	}
	
	IEnumerator MusicSpeedControl()
	{
		while(true)
		{
			if(YEntityManager.Instance == null ||
				YEntityManager.Instance.PlayerEntity == null)
			{
				yield return null;

				soundManager.SetPitch(1f);
				
				continue;
			}
			
			yield return new WaitForSeconds(0.1f);
			
			Player player = YEntityManager.Instance.PlayerEntity;
			if(player.Living == true)
			{
				float intensity = player.Exp * player.Exp * player.Exp * 0.01f * 0.01f * 0.01f * 1.5f + 1f;
				soundManager.SetPitch(intensity);
			}
		}
	}
	
	void Update () {
		
//		KeyProcess();
	}
	#endregion
	#region - stage select, clear & death -
	public void SetCurStageIdx(int _idx)
	{
		m_SelectedStageIdx = _idx;
	}
	
	public void StageCleared()
	{
		soundManager.SetPitch(1f);

		if(YEntityManager.Instance.PlayerEntity != null)
			YEntityManager.Instance.PlayerEntity.HandleMessage(new Msg_PlayerInvincible());
		
		if(PlayerPrefs.GetInt("ClearedStageIdx", 0) < m_SelectedStageIdx)
			PlayerPrefs.SetInt("ClearedStageIdx", m_SelectedStageIdx);
//		m_Status.clearedStageIdx_ = m_SelectedStageIdx;	
		
		StartCoroutine(StageClearProcess());
	}
	
	IEnumerator StageClearProcess()
	{
		yield return new WaitForSeconds(4);
		
		if(m_SelectedStageIdx >= m_Status.LastStageIndex)
			SceneConversion("Ending");
		else
			SceneConversion("Stage_Select_New");
	}
	
    //public void PlayerDeath()
    //{
    //    soundManager.SetPitch(1f);

    //    StartCoroutine(PostDeathProcess());
    //}
	
    //IEnumerator PostDeathProcess()
    //{
    //    yield return new WaitForSeconds(2);
		
    //    GameObject objText = Instantiate(Resources.Load("UI/GameOver_Text")) as GameObject;
    //    objText.GetComponent<Text>().material.color = new Color(0.8f, 0.8f, 0.8f);
    //    GameObject objText2 = Instantiate(Resources.Load("UI/ToStageSelect_Text")) as GameObject;
    //    objText2.AddComponent<ToStageSelect_Click>();
    //    objText2.GetComponent<Text>().material.color = new Color(0.8f, 0.8f, 0.8f);
    //    GameObject objText3 = Instantiate(Resources.Load("UI/Retry_Text")) as GameObject;
    //    objText3.AddComponent<Retry_Click>();
    //    objText3.GetComponent<Text>().material.color = new Color(0.8f, 0.8f, 0.8f);
    //}
	
    //public void Click_ToStageSelect()
    //{
    //    StartCoroutine(ToStageSelect_CR());
    //}
	
    //IEnumerator ToStageSelect_CR()
    //{
    //    yield return new WaitForSeconds(0.5f);
		
    //    SceneConversion("Stage_Select_New");
    //}
	
    //public void Click_Retry()
    //{
    //    StartCoroutine(Retry_CR());
    //}
	
    //IEnumerator Retry_CR()
    //{
    //    yield return new WaitForSeconds(0.5f);
		
    //    SceneConversion(m_SelectedStageIdx.ToString());
    //}
	#endregion
	#region - scene changing -
	public class SceneConversionInfo
	{
		public string name_;
		public float wait_;
		
		public SceneConversionInfo( string _name, float _wait)
		{
			name_ = _name;
			wait_ = _wait;
		}
	}
	
	string m_ReservedStage;
    string m_PrevStage;
//	public void SceneConversion(string _name, float _wait)
//	{
//		StartCoroutine( SceneConversion_CR( new SceneConversionInfo( _name, _wait)));
//	}
//	
//	IEnumerator SceneConversion_CR(SceneConversionInfo _info)
//	{
//		yield return new WaitForSeconds( _info.wait_);
//		
//		SceneConversion( _info.name_);
//	}
	
	public void SceneConversion(string _name)
	{
		if(m_SceneChanging == true)
			return;
		
		m_SceneChanging = true;
		
		m_ReservedStage = _name;
		
		Instantiate(Resources.Load("Effect/ConvertScreen"), new Vector3(9999, 9999, 9999), Quaternion.identity);

		Time.timeScale = 1f;
	}
	
	public void CurtainCoveredFull()
	{
		int stageIdx = 1;
		if(int.TryParse(m_ReservedStage, out stageIdx) == true)
		{
			Application.LoadLevel("InGame");
		}
		else
		{
			Application.LoadLevel(m_ReservedStage);
		}
		
		int number = 1;
		if(int.TryParse(m_ReservedStage, out number) == true)
			soundManager.PlayStageByIndex(number);
		else if(soundManager.IsPlaying("Main Theme") == false)
			soundManager.PlayBgm("Main Theme");

        if (m_ReservedStage == "Stage_Select_New" && m_PrevStage != "Title" && AdvertisementManager.Instance != null)
        {
            if (Random.Range(0, 3) == 0)
                AdvertisementManager.Instance.Show_Interstitial();
        }

        m_PrevStage = m_ReservedStage;

		#region - glow -
//		if(Camera.main == null)
//			Debug.LogError("MainFlow:: CurtainCoveredFull: main camera is not found");
//
//		MKGlowSystem.MKGlow glow = Camera.main.GetComponent<MKGlowSystem.MKGlow>();
//		if(glow == null)
//			Debug.LogError("MainFlow:: CurtainCoveredFull: mkglow component is not found");
//
//		glow.BlurIterations = m_BlurIteration;
//		glow.GlowIntensity = m_GlowIntensity;
		#endregion

#if _EDITOR
		UnityEditor.EditorUtility.SetDirty(m_Status);
#endif
	}
	
	public void SceneChangingFinished()
	{
		m_SceneChanging = false;

		if(m_Invincible == true && YEntityManager.Instance != null && YEntityManager.Instance.PlayerEntity != null)
		   YEntityManager.Instance.PlayerEntity.HandleMessage(new Msg_PlayerInvincible());
	}
	#endregion
	#region - sound -
	public void Sound_NormalDeath()
	{
		soundManager.PlayEffect("NormalDeath");
	}
	public void Sound_BossDeath()
	{
		soundManager.PlayEffect("BossDeath");
	}
	public void Sound_PlayerDeath()
	{
		soundManager.PlayEffect("PlayerDeath");
	}
	#endregion
	#region - input setting -
	public void SetInput(eInputType _type)
	{
		s_InputType = _type;
	}
	#endregion
	#region - key operation -
	void KeyProcess()
	{
		if( Application.platform == RuntimePlatform.Android)
		{
			if( Input.GetKey( KeyCode.Escape))
			{
				
			}
		}
	}
	#endregion
}
