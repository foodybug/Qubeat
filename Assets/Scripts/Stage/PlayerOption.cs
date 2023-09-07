using UnityEngine;
using System.Collections;

public class PlayerOption : MonoBehaviour {

	#region - singleton -
	static PlayerOption instance;
	public static PlayerOption Instance
	{get{return instance;}}
	#endregion

	[SerializeField] float m_ConsumeWaitTime;
	public float ConsumeWaitTime{get{return m_ConsumeWaitTime;}}

	[SerializeField] float m_ConsumeRadiusOnLevelUp = 0.5f;
	public float ConsumeRadiusOnLevelUp{get{return m_ConsumeRadiusOnLevelUp;}}

	[SerializeField] float m_ConsumeInterval = 0.1f;
	public float ConsumeInterval{get{return m_ConsumeInterval;}}

    [SerializeField] float m_ConsumeRadiusOnStageUp = 0.7f;
    public float ConsumeRadiusOnStageUp { get { return m_ConsumeRadiusOnStageUp; } }

    void Awake()
	{
		#region - singleton -
		instance = this;
		#endregion
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
