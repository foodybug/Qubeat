using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour {
	
	#region - singleton -
	static Title instance; public static Title Instance{get{return instance;}}
	#endregion

	[SerializeField] float waitTime_InputEnable_ = 2.0f;
    [SerializeField] int maxBouncer_ = 100;

	bool m_ChangeEnable = false; public bool ChangeEnable { get { return m_ChangeEnable; } }

	void Awake()
	{
		#region - singleton -
		instance = this;
		#endregion
		
		Bouncer.ClearBouncerList();
		
		for (int i = 0; i < maxBouncer_; ++i)
		{
			Instantiate(Resources.Load("Effect/Bouncer_Title"));
		}
		
		Instantiate(Resources.Load("Effect/Actor_Title"));
	}
	
	IEnumerator Start()
	{
		yield return new WaitForSeconds(waitTime_InputEnable_);
		
		m_ChangeEnable = true;
	}

    public void OnGameStart()
    {
        Debug.Log("OnGameStart: before");
        if (!ChangeEnable)
        {
            return;
        }
        Debug.Log("OnGameStart: after");

        //        MainFlow.Instance.SetInput(eInputType.Click);

        StartCoroutine(ChangeToStageSelect());
    }

    public void OnExit()
    {
        if (!ChangeEnable)
        {
            return;
        }

        Debug.Log("quit");

        Application.Quit();
    }

    public void OnDeveloper()
    {
        if (!ChangeEnable)
        {
            return;
        }

        Debug.Log("developer page");

        Application.OpenURL("http://foodybug.egloos.com/4107702");
    }

    IEnumerator ChangeToStageSelect()
    {
        yield return new WaitForSeconds(0.5f);

        MainFlow.Instance.SceneConversion("Briefing");
    }
}