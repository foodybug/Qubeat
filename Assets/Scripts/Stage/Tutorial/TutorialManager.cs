using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#region - class -
public class CloseColliders
{
	public SortedDictionary<float, YCollider> slistSmaller = new SortedDictionary<float, YCollider>();
	public SortedDictionary<float, YCollider> slistBigger = new SortedDictionary<float, YCollider>();
}
#endregion

public class TutorialManager : MonoBehaviour {
	
	#region - singleton -
	static TutorialManager instance; public static TutorialManager Instance {get{return instance;}}
	#endregion
	
	public enum eDisplayType {NONE, Smaller, Bigger, Boss}
	public enum eState {Not_Enough_Level, Level_Up}
	
	[SerializeField] int m_MaxSmalllerCount = 3; public int MaxSmallerCount{get{return m_MaxSmalllerCount;}}
	[SerializeField] int m_MaxBiggerCount = 2; public int MaxBiggerCount{get{return m_MaxBiggerCount;}}
	
	[SerializeField] string m_strSmaller;
	[SerializeField] string m_strBigger;
	[SerializeField] string m_strBoss;
	
	[SerializeField] string m_strCatchSmaller;
	
	[SerializeField] string m_strPlayerDeath;
	[SerializeField] string m_strBossDefeated;
	
	[SerializeField] string m_strNormalExplain;
	[SerializeField] string m_strBossExplain;
	[SerializeField] string m_strPlayerGetExp;
	[SerializeField] string m_strPlayerLevelUp;
	[SerializeField] string m_strTargetBoss;
	[SerializeField] string m_strBossRaid;
	
	
	[SerializeField] float m_WaitBeginTime = 2f;
//	[SerializeField] float m_RefreshColliderInterval = 1.5f;
	
	CloseColliders m_CloseColliders = null;
	
//	int m_CurSmallerCount = 0;
//	int m_CurBiggerCount = 0;
	
	List<TutorialDisplay> m_listSmaller = new List<TutorialDisplay>();
	List<TutorialDisplay> m_listBigger = new List<TutorialDisplay>();
	
	[SerializeField] Text m_MainText;
	[SerializeField] Vector3 m_MainTextPos = new Vector3(0.5f, 0.2f, 0);
	eState m_State = eState.Not_Enough_Level;
	
	bool m_Block = true;
	bool m_End = false;
	
	void Awake()
	{
		#region - singleton -
		instance = this;
		#endregion
	}
	
	IEnumerator Start()
	{		
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		if(status.tutorialFinished_ == false)
		{
//			Debug.Log("TutorialManager::Start: ");
			
			yield return new WaitForSeconds(m_WaitBeginTime);
			
			m_MainText.transform.position = m_MainTextPos;
			NormalExplain();
			
			ResetDisplay();
			
			Boss boss = YEntityManager.Instance.BossEntity;
			TutorialDisplay display = boss.gameObject.AddComponent<TutorialDisplay>();
			display.Init(m_strBoss, 23, eDisplayType.Boss);
			
			InvokeRepeating("ResetDisplay", 1f, 2f);
		}
		else
			Destroy(gameObject);
	}
	
	#region - basic display -
	void ResetDisplay()
	{
		ReleaseDisplay();
		SetDisplay();
	}
	
	void SetDisplay()
	{
		Player player = YEntityManager.Instance.PlayerEntity;
		if(player == null)
			return;
		
		m_CloseColliders = YColliderManager.Instance.GetCloseColliders(player.transform.position);
		
		int count = 0;
		foreach(KeyValuePair<float, YCollider> pair in m_CloseColliders.slistSmaller)
		{
			TutorialDisplay display = pair.Value.gameObject.AddComponent<TutorialDisplay>();
			m_listSmaller.Add(display);
			if(display != null)
			{
				display.Init(m_strSmaller, 15, eDisplayType.Smaller);
				
				if(++count >= m_MaxSmalllerCount)
					break;
			}
		}
		
		count = 0;
		foreach(KeyValuePair<float, YCollider> pair in m_CloseColliders.slistBigger)
		{
			TutorialDisplay display = pair.Value.gameObject.AddComponent<TutorialDisplay>();
			m_listBigger.Add(display);
			if(display != null)
			{
				display.Init(m_strBigger, 20, eDisplayType.Bigger);
				
				if(++count >= m_MaxBiggerCount)
					break;
			}
		}
	}
	
	void ReleaseDisplay()
	{
		foreach(TutorialDisplay node in m_listSmaller)
		{
			if(node != null)
				Destroy(node);
		}
		
		foreach(TutorialDisplay node in m_listBigger)
		{
			if(node != null)
				Destroy(node);
		}
		
		m_listSmaller.Clear();
		m_listBigger.Clear();
	}
	
	public void SmallerDeath_TutorialManager(TutorialDisplay _display)
	{
		GameObject obj1 = new GameObject("'" + m_strCatchSmaller + "' beacon");
		obj1.transform.position = _display.transform.position;
		TutorialDisplay display = obj1.AddComponent<TutorialDisplay>();
		display.Init(m_strCatchSmaller, 15, eDisplayType.NONE);
		display.SetAnchor(TextAnchor.UpperCenter);
		display.SetDestroyTime(0.5f);
		
		ResetDisplay();
	}
	#endregion
	#region - stage clear & fail -
	public void PlayerDeath_TutorialManager(Player _player)
	{
		StartCoroutine(PlayerDeath(_player));
	}
	IEnumerator PlayerDeath(Player _player)
	{
		m_Block = true;
		m_End = true;
		
		//m_MainText.text = "";
        if (InGame.Instance != null)
        {
            InGame.Instance.HideTutorial();
        }
		
		ReleaseDisplay();
		CancelInvoke();
		
		yield return new WaitForSeconds(2);
		
		//m_MainText.text = m_strPlayerDeath;
        if (InGame.Instance != null)
        {
            InGame.Instance.ShowTutorial(m_strPlayerDeath, 1.0f);
        }
	}
	
	public void BossDeath_TutorialManager(TutorialDisplay _display)
	{
		StopCoroutine("BossDeath");
		StartCoroutine("BossDeath");
	}	
	IEnumerator BossDeath()
	{
		m_Block = true;
		m_End = true;
		
		//m_MainText.text = "";
        if (InGame.Instance != null)
        {
            InGame.Instance.HideTutorial();
        }
		
		ReleaseDisplay();
		CancelInvoke();
		
		yield return new WaitForSeconds(1);
		
		//m_MainText.text = m_strBossDefeated;
        if (InGame.Instance != null)
        {
            InGame.Instance.ShowTutorial(m_strBossDefeated, 1.0f);
        }
	}
	#endregion
	
	public void PlayerStart()
	{
		m_Block = false;
	}
	
	void NormalExplain()
	{
		if(m_Block == true)
			return;
		
		switch(m_State)
		{
		    case eState.Not_Enough_Level:
                //m_MainText.text = m_strNormalExplain;
                if (InGame.Instance != null)
                {
                    InGame.Instance.ShowTutorial(m_strNormalExplain, 1.0f);
                }
			    break;

		    case eState.Level_Up:
			    //m_MainText.text = m_strBossRaid;
                if (InGame.Instance != null)
                {
                    InGame.Instance.ShowTutorial(m_strBossRaid, 1.0f);
                }
			    break;
		}
		
		//m_MainText.fontSize = 30;
	}
	
	public void BossAppear_TutorialManager(Boss _boss)
	{
		if(m_Block == true)
			return;
		
		StopCoroutine("BossAppear");
		StartCoroutine("BossAppear", _boss);
	}
	IEnumerator BossAppear(Boss _boss)
	{
		switch(m_State)
		{
		    case eState.Not_Enough_Level:
			    //m_MainText.text = m_strBossExplain;
                if (InGame.Instance != null)
                {
                    InGame.Instance.ShowTutorial(m_strBossExplain, 1.0f);
                }
			    break;

		    case eState.Level_Up:
			    //m_MainText.text = m_strTargetBoss;
                if (InGame.Instance != null)
                {
                    InGame.Instance.ShowTutorial(m_strTargetBoss, 1.0f);
                }
			    break;
		}
		
		yield return new WaitForSeconds(3);
		
		NormalExplain();
	}
	public void BossDisappear_TutorialManager()
	{
//		NormalExplain();
	}
	
	public void PlayerExp_TutorialManager(Player _player)
	{
		if(m_Block == true)
			return;
		
		switch(m_State)
		{
		case eState.Not_Enough_Level:
			break;
		case eState.Level_Up:
			return;
		}
		
		if(_player.Exp > 40)
		{
			StopCoroutine("PlayerExp");
			StartCoroutine("PlayerExp", _player);
		}
	}	
	IEnumerator PlayerExp(Player _player)
	{
		//m_MainText.text = m_strPlayerGetExp;
        if (InGame.Instance != null)
        {
            InGame.Instance.ShowTutorial(m_strPlayerGetExp, 1.0f);
        }
		
		yield return new WaitForSeconds(3);
		
		NormalExplain();
	}
	
	public void PlayerLevelUp_TutorialManager(Player _player)
	{
		StopCoroutine("PlayerLevelUp");
		StartCoroutine("PlayerLevelUp");
	}	
	IEnumerator PlayerLevelUp()
	{
		m_Block = true;
        Player player = YEntityManager.Instance.PlayerEntity;
        Boss boss = YEntityManager.Instance.BossEntity;
        if(player != null && boss != null && player.Level >= boss.Level)
            m_State = eState.Level_Up;

        if (InGame.Instance != null)
        {
            InGame.Instance.ShowTutorial(m_strPlayerLevelUp, 1.2f);
        }
		
		ResetDisplay();
		
		yield return new WaitForSeconds(4);
		
		if(m_End == false)
		{
			m_Block = false;
			NormalExplain();
		}
	}
	
	
//	IEnumerator TutorialUpdate()
//	{
//		Player player = YEntityManager.Instance.PlayerEntity;
//		
//		while(true)
//		{
//			if(player != null)
//			{
//				m_SmallerCollider = YColliderManager.Instance.GetClosestSmallerAndBigger(player.transform.position)[0];
//				m_Smaller.text = m_strSmaller;
//				m_BiggerCollider = YColliderManager.Instance.GetClosestSmallerAndBigger(player.transform.position)[1];
//				
//				m_BossEntity = YEntityManager.Instance.BossEntity;
//			}
//			
//			yield return new WaitForSeconds(m_RefreshColliderInterval);
//		}
//	}
//	
//	IEnumerator PositionUpdate()
//	{
//		Player player = YEntityManager.Instance.PlayerEntity;
//		
//		while(true)
//		{
//			yield return null;
//			
//			if(m_SmallerCollider != null)
//			{
//				if(m_SmallerCollider.AttachedEntity.Living == true)
//				{
//					Vector3 pos = m_SmallerCollider.transform.position;
//					m_Smaller.transform.position = Camera.mainCamera.WorldToViewportPoint(pos);
//				}
//				else
//					m_Smaller.transform.position = new Vector3(10f, 10f, 10f);
//			}
//			
//			if(m_BiggerCollider != null)
//			{
//				if(m_BiggerCollider.AttachedEntity.Living == true)
//				{
//					Vector3 pos = m_BiggerCollider.transform.position;
//					m_Bigger.transform.position = Camera.mainCamera.WorldToViewportPoint(pos);
//				}
//				else
//					m_BiggerCollider = null;
//			}
//			else
//				m_Bigger.transform.position = new Vector3(10f, 10f, 10f);
//			
//			if(m_BossEntity != null)
//			{
//				if(m_BossEntity.Living == true)
//				{
//					Vector3 pos = m_BossEntity.transform.position;
//					m_Boss.transform.position = Camera.mainCamera.WorldToViewportPoint(pos);
//				}
//				else
//				{
//					m_Boss.fontSize = 30;
//					m_Boss.text = m_strBossDefeated;
//				}
//			}
//			else
//				m_Boss.transform.position = new Vector3(10f, 10f, 10f);
//		}
//	}
//	
//	public void PlayerLevelUp()
//	{
//		Player player = YEntityManager.Instance.PlayerEntity;
//		if(player != null)
//		{
//			m_SmallerCollider = YColliderManager.Instance.GetClosestSmallerAndBigger(player.transform.position)[0];
//			m_BiggerCollider = YColliderManager.Instance.GetClosestSmallerAndBigger(player.transform.position)[1];
//			
//			m_BossEntity = YEntityManager.Instance.BossEntity;
//		}
//	}
}
