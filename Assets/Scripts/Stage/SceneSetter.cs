//#define _EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Xml;
using System.IO;

#region - struct for initialize -
[System.Serializable]
public class Setter_Roamer
{
	public int[] count_Lv_ = new int[10];
}

[System.Serializable]
public class Setter_Stone
{
	public int[] count_Lv_ = new int[10];
}

[System.Serializable]
public class Setter_Stalker
{
	public int[] count_Lv_ = new int[10];
}

[System.Serializable]
public class Setter_Bullet
{
	public int[] count_Lv_ = new int[10];
}

[System.Serializable]
public class Setter_Ghost
{
	public float initSpeed_ = 1f;
	public float deltaSpeed_ = 0.5f;
	public float interval_ = 10f;
}
#endregion

[ExecuteInEditMode]
public class SceneSetter : MonoBehaviour
{
	#region - singleton -
	static SceneSetter instance;
	public static SceneSetter Instance
	{get{return instance;}}
	#endregion
	#region - member -
	[SerializeField] int m_CurStageIdx = 1; public int curStageIdx { get { return m_CurStageIdx; } }
	int _allStageClearCount = 0; public int allStageClearCount { get { return _allStageClearCount; } }
	[SerializeField] float m_CameraHeight = 10;
	[SerializeField] bool m_Tutorial = false;
	
	[SerializeField] Setter_Roamer m_RoamerInfo; public Setter_Roamer RoamerInfo{get{return m_RoamerInfo;}}
	[SerializeField] Setter_Stone m_StoneInfo; public Setter_Stone StoneInfo{get{return m_StoneInfo;}}
	[SerializeField] Setter_Stalker m_StalkerInfo; public Setter_Stalker StalkerInfo{get{return m_StalkerInfo;}}
	[SerializeField] Setter_Bullet m_BulletInfo; public Setter_Bullet BulletInfo{get{return m_BulletInfo;}}
	[SerializeField] Setter_Ghost m_GhostInfo; public Setter_Ghost GhostInfo{get{return m_GhostInfo;}}
	[SerializeField] int m_BossLv = 3;
    readonly int minLv = 1;
    readonly int maxLv = 10;

    int entityCnt = 0;

    Dictionary<int, List<YCreationData>> m_dicLvCreation = new Dictionary<int, List<YCreationData>>();
    #endregion

    void Awake()
	{
		instance = this;
	}
	
	void Start ()
	{
		if(Application.isPlaying == false)
			return;
		
		#region - player -
		CreationData_Player playerCreation = new CreationData_Player(Vector3.zero, 2);
		Player player = YEntityManager.Instance.CreateEntity(playerCreation) as Player;
		YEntityManager.Instance.SetPlayerEntity(player);
		#endregion
		
		#region - camera -
		GameObject cameraObj = new GameObject("Camera");
		cameraObj.AddComponent<YCameraManager>();
		Camera.main.transform.parent = cameraObj.transform;
        Camera.main.depth = -1;
//		Camera.main.gameObject.AddComponent<YCameraManager>();
		YCameraManager.Instance.SetPosOnEntity(player, m_CameraHeight);
		#endregion
		
		if(MainFlow.Instance != null)
			SetStageIdx(MainFlow.Instance.SelectedStageIdx);

		//SetScene();
		//SetSceneFromTable();
		SetSceneByCurve();
		
		if(m_Tutorial == true)
			Instantiate(Resources.Load("Misc/TutorialManager"));
	}
	
	void SetStageIdx(int _idx)
	{
		m_CurStageIdx = _idx;
		
		if(_idx == 1)
			Instantiate(Resources.Load("Misc/TutorialManager"));
	}

	public void LevelUp()
    {
		++m_CurStageIdx;

		if (m_CurStageIdx > 100)
		{
			++_allStageClearCount;
		}
	}

	#region - loading(obsolete) -
	void SetScene()
	{
		Vector3 pos;
        m_dicLvCreation.Clear();
		#region - roamer -
		for(int i=0; i<10; ++i)
		{
			for(int j=0; j<m_RoamerInfo.count_Lv_[i]; ++j)
			{
				pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
				CreationData_Roamer roamerCreation = new CreationData_Roamer(pos, i+1);
				YEntityManager.Instance.CreateEntity(roamerCreation);
			}
		}
		#endregion
		#region - stone -
		for(int i=0; i<10; ++i)
		{
			for(int j=0; j<m_StoneInfo.count_Lv_[i]; ++j)
			{
				pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
				CreationData_Stone stoneCreation = new CreationData_Stone(pos, i+1);
				YEntityManager.Instance.CreateEntity(stoneCreation);
			}
		}
		#endregion
		#region - stalker -
		for(int i=0; i<10; ++i)
		{
			for(int j=0; j<m_StalkerInfo.count_Lv_[i]; ++j)
			{
				pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
				CreationData_Stalker stalkerCreation = new CreationData_Stalker(pos, i+1);
				YEntityManager.Instance.CreateEntity(stalkerCreation);
			}
		}
		#endregion
		#region - bullet -
		for(int i=0; i<10; ++i)
		{
			for(int j=0; j<m_BulletInfo.count_Lv_[i]; ++j)
			{
				pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
				CreationData_Bullet bulletCreation = new CreationData_Bullet(pos, i+1);
				YEntityManager.Instance.CreateEntity(bulletCreation);
			}
		}
        #endregion
        #region - ghost -
        YEntityManager.Instance.GenerateGhost(m_GhostInfo.initSpeed_, m_GhostInfo.deltaSpeed_, m_GhostInfo.interval_);
		#endregion
		#region - boss -
		pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
		CreationData_Boss BossCreation = new CreationData_Boss(pos, m_BossLv);
		Boss boss = YEntityManager.Instance.CreateEntity(BossCreation) as Boss;
		YEntityManager.Instance.SetBossEntity(boss);
		#endregion
	}
    #endregion
    #region - loading by xml -
    void SetSceneFromTable()
	{
		#region - load -
		XmlDocument xmlDoc = new XmlDocument();
		
		TextAsset textAsset = Resources.Load("Asset/StageInfo") as TextAsset;
		MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(textAsset.text));
		StreamReader srr = new StreamReader(stream);
		StringReader sr = new StringReader(srr.ReadToEnd());
		string str = sr.ReadToEnd();
		
		xmlDoc.LoadXml(str);
		XmlElement root = xmlDoc.DocumentElement;
		XmlNodeList list = root.GetElementsByTagName("Stage_" + m_CurStageIdx);
//		Debug.Log("SceneSetter::SetSceneFromTable: m_CurStageIdx = " + m_CurStageIdx);
		
		XmlElement stage = null;
		foreach(XmlNode node in list)
		{
			if(stage != null) Debug.LogError("SceneSetter::SetSceneFromTable: same named stage record = " + "Stage_" + m_CurStageIdx);
			stage = node as XmlElement;
		}
		#endregion
		#region - stage size -
		XmlElement stageSize = null;
		list = stage.GetElementsByTagName("Stage_Size");
		foreach(XmlNode node in list)
		{
			if(stageSize != null) Debug.LogError("SceneSetter::SetSceneFromTable: same named stage record = StageSize");
			stageSize = node as XmlElement;
		}
		
		float size = float.Parse(stageSize.GetAttribute("Size"));
		YStageManager.Instance.SetStageInfo(size);
        #endregion
        #region - enemies -
        m_dicLvCreation.Clear();
        entityCnt = 0;

        list = stage.GetElementsByTagName("Enemy");
        foreach (XmlNode node in list)
        {
            XmlElement element = node as XmlElement;
            if (element.GetAttribute("lv") == "1")
            {
                CreateEnemies<CreationData_Roamer>(GetElement(element, "Roamer"));
                CreateEnemies<CreationData_Stone>(GetElement(element, "Stone"));
                CreateEnemies<CreationData_Stalker>(GetElement(element, "Stalker"));
                CreateEnemies<CreationData_Bullet>(GetElement(element, "Bullet"));
                //CreateEnemies<CreationData_Dancer>(GetElement(element, "Dancer"), 0);
                //CreateEnemies<CreationData_Specter>(GetElement(element, "Specter"), 0);

                Debug.Log("SceneSetter:: SetSceneFromTable: entity count = " + entityCnt);
            }
            else
            {
                int evolLv = int.Parse(element.GetAttribute("lv"));
                List<YCreationData> listCreation = new List<YCreationData>();

                _AddToCreationList(ref listCreation, GetCreationData<CreationData_Roamer>(GetElement(element, "Roamer")));
                _AddToCreationList(ref listCreation, GetCreationData<CreationData_Stone>(GetElement(element, "Stone")));
                _AddToCreationList(ref listCreation, GetCreationData<CreationData_Stalker>(GetElement(element, "Stalker")));
                _AddToCreationList(ref listCreation, GetCreationData<CreationData_Bullet>(GetElement(element, "Bullet")));
                //_AddToCreationList(ref listCreation, GetCreationData<CreationData_Dancer>(GetElement(element, "Dancer"), 0));
                //_AddToCreationList(ref listCreation, GetCreationData<CreationData_Specter>(GetElement(element, "Specter"), 0));

                m_dicLvCreation.Add(evolLv, listCreation);
            }
        }

        YEntityManager.Instance.RegisterEvolEntities(m_dicLvCreation);
        #endregion
        #region - ghost & boss -
        //ghost
        XmlElement ghost = null;
		list = stage.GetElementsByTagName("Ghost");
		foreach(XmlNode node in list)
		{
			if(ghost != null) Debug.LogError("SceneSetter::SetSceneFromTable: same named stage record = Ghost");
			ghost = node as XmlElement;
		}
		
		float initSpeed = float.Parse(ghost.GetAttribute("Init_Speed"));
		float deltaSpeed = float.Parse(ghost.GetAttribute("Delta_Speed"));
		float interval = float.Parse(ghost.GetAttribute("Interval"));

        YEntityManager.Instance.GenerateGhost(initSpeed, deltaSpeed, interval);

        //boss
        XmlElement boss = null;
		list = stage.GetElementsByTagName("Boss");
		foreach(XmlNode node in list)
		{
			if(boss != null) Debug.LogError("SceneSetter::SetSceneFromTable: same named stage record = Ghost");
			boss = node as XmlElement;
		}
		
		int lv = int.Parse(boss.GetAttribute("lv"));

        Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
		CreationData_Boss BossCreation = new CreationData_Boss(pos, lv);
		Boss bossEntity = YEntityManager.Instance.CreateEntity(BossCreation) as Boss;
		YEntityManager.Instance.SetBossEntity(bossEntity);
        #endregion
        #region - additional entity -
        //XmlElement addition = null;
        //list = stage.GetElementsByTagName("Addition");
        //foreach (XmlNode node in list)
        //{
        //    if (addition != null) Debug.LogError("SceneSetter::SetSceneFromTable: same named stage record = Roamer");
        //    addition = node as XmlElement;
        //}

        //for (int i = 1; i <= 10; ++i)
        //{
        //    int count = int.Parse(roamer.GetAttribute("lv." + i));
        //    for (int j = 0; j < count; ++j)
        //    {
        //        pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
        //        CreationData_Roamer roamerCreation = new CreationData_Roamer(0, pos, i);
        //        YEntityManager.Instance.CreateEntity(roamerCreation);

        //        ++entityCnt;
        //    }
        //}
        #endregion
    }

    XmlElement GetElement(XmlElement parent, string str)
    {
        XmlElement ret = null;
        XmlNodeList list = parent.GetElementsByTagName(str);
        foreach (XmlNode node in list)
        {
            if (ret != null) Debug.LogError("SceneSetter:: GetElement: same named stage record = " + str);
            ret = node as XmlElement;
        }

        //if (ret == null)
        //    Debug.LogWarning("SceneSetter:: GetElement: [" + str + "] XmlElement is not found");

        return ret;
    }

    void CreateEnemies<T>(XmlElement element) where T : YCreationData, new()
    {
        if (element == null)
            return;

        for (int i = minLv; i <= maxLv; ++i)
        {
            string strCount = element.GetAttribute("lv." + i);
            int count = 0;
            if (int.TryParse(strCount, out count) == true)
            {
                for (int j = 0; j < count; ++j)
                {
                    Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
                    T creation = new T();
                    creation.SetData(pos, i);
                    YEntityManager.Instance.CreateEntity(creation);

                    ++entityCnt;
                }
            }
        }
    }

    List<YCreationData> GetCreationData<T>(XmlElement element) where T : YCreationData, new()
    {
        if (element == null)
        {
            //Debug.LogWarning("SceneSetter:: GetCreationData: element is null");
            return null;
        }

        List<YCreationData> listCreation = new List<YCreationData>();

        for (int i = minLv; i <= maxLv; ++i)
        {
            string strCount = element.GetAttribute("lv." + i);
            int count = 0;
            if (int.TryParse(strCount, out count) == true)
            {
                for (int j = 0; j < count; ++j)
                {
                    Vector3 pos = Vector3.zero;
                    T creation = new T();
                    creation.SetData(pos, i, true);

                    listCreation.Add(creation);
                }
            }
        }

        return listCreation;
    }

    void _AddToCreationList(ref List<YCreationData>  listCreation, List<YCreationData> list)
    {
        if(list != null)
           listCreation.AddRange(list);
    }
    #endregion
    #region - loading by curve -
    void SetSceneByCurve()
    {
        YStageManager.Instance.SetStageInfo(1);
        m_dicLvCreation.Clear();
        entityCnt = 0;

        StageSetter_Linear ssl = StageSetter_Linear.instance;
        List<YCreationData> listInitStage = ssl.GetStageData(m_CurStageIdx);
        foreach (YCreationData node in listInitStage)
            YEntityManager.Instance.CreateEntity(node);

        for (int i = 1; i < StageSetter_Linear.stageCount; ++i)
        {
            List<YCreationData> list = ssl.GetStageData(i);
            m_dicLvCreation.Add(i, list);
        }

        YEntityManager.Instance.RegisterEvolEntities(m_dicLvCreation);
    }

    //IEnumerator SetSceneByCurve()
    //{
    //	YStageManager.Instance.SetStageInfo(1);
    //	m_dicLvCreation.Clear();
    //	entityCnt = 0;

    //	List<YCreationData> list = new List<YCreationData>();

    //	StageSetter_Linear ssl = StageSetter_Linear.instance;
    //	ssl.SetStageIndex(0);
    //	yield return StartCoroutine(ssl.GetStageData(list));
    //	foreach (YCreationData node in list)
    //		YEntityManager.Instance.CreateEntity(node);

    //	list.Clear();
    //	for (int i = 1; i < StageSetter_Linear.stageSize; ++i)
    //	{
    //		ssl.SetStageIndex(i);
    //		yield return StartCoroutine(ssl.GetStageData(list));
    //		m_dicLvCreation.Add(i, list);
    //	}

    //	YEntityManager.Instance.RegisterEvolEntities(m_dicLvCreation);
    //}
    #endregion

    public string difficulty_ = "0";
	[SerializeField] bool m_Calculate = false;
//	[SerializeField] bool m_Reset = false;
	void Update ()
	{
		if(m_Calculate == true)
		{
			m_Calculate = false;
			CalculateDifficulty();
			SaveStageInfo();
		}
	}
	
	public void CalculateDifficulty()
	{
#if _EDITOR
		float difficulty = 0;
		
		#region - difficulty calculate -
		for(int i=0; i<10; ++i)
		{
			float v = Mathf.Pow(i, 1.4f) * m_RoamerInfo.count_Lv_[i];
			if(i == 0) v = -0.7f * m_RoamerInfo.count_Lv_[i];
			difficulty += v;
		}
		
		for(int i=0; i<10; ++i)
		{
			float v = Mathf.Pow(i, 1.2f) * m_StoneInfo.count_Lv_[i];
			if(i == 0) v = -0.9f * m_RoamerInfo.count_Lv_[i];
			difficulty += v;
		}
		
		for(int i=0; i<10; ++i)
		{
			float v = Mathf.Pow(i, 1.8f) * m_StalkerInfo.count_Lv_[i];
			if(i == 0) v = -0.5f * m_RoamerInfo.count_Lv_[i];
			difficulty += v;
		}
		
		for(int i=0; i<10; ++i)
		{
			float v = Mathf.Pow(i, 2.2f) * m_BulletInfo.count_Lv_[i];
			if(i == 0) v = -0.1f * m_RoamerInfo.count_Lv_[i];
			difficulty += v;
		}
		
		float dif1 = Mathf.Round(difficulty);
		difficulty_ =  "(" + dif1.ToString();
		difficulty += m_GhostInfo.initSpeed_ * 30f * m_GhostInfo.deltaSpeed_ * 3f * 60f / m_GhostInfo.interval_;
		
		float dif2 = Mathf.Round(difficulty);
		difficulty_ += " + " + (dif2 - dif1).ToString();
		difficulty *= m_BossLv;
		
		float dif3 = Mathf.Round(difficulty);
		difficulty_ += ") * " + (dif3 / dif2).ToString();
		
		float dif4 = YStageManager.s_StdStageSize / GetComponent<YStageManager>().GetStageExtent();
		dif4 = Mathf.Round(dif4);
		difficulty *= dif4;
		
		difficulty_ += " / " + dif4.ToString();
		difficulty_ += " = " + Mathf.Round(difficulty).ToString();
		
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		string[] lvs = UnityEditor.EditorApplication.currentScene.Split('/');
		string lv = lvs[lvs.Length - 1].Replace(".unity", "");
		status.difficulty_[int.Parse(lv) - 1] = difficulty;
		UnityEditor.EditorUtility.SetDirty(status);
		#endregion
#endif
	}
	
	void SaveStageInfo()
	{
#if _EDITOR
		#region - file load -
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		string[] lvs = UnityEditor.EditorApplication.currentScene.Split('/');
		string lv = lvs[lvs.Length - 1].Replace(".unity", "");
		
		XmlDocument xmlDoc = new XmlDocument();
		
		TextAsset textAsset = Resources.Load("Asset/StageInfo") as TextAsset;
		MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(textAsset.text));
		StreamReader srr = new StreamReader(stream);
		StringReader sr = new StringReader(srr.ReadToEnd());
		string str = sr.ReadToEnd();
		#endregion
		#region - root & init -
		xmlDoc.LoadXml(str);
		XmlElement root = xmlDoc.DocumentElement;
		XmlNodeList del = root.GetElementsByTagName("Stage_" + lv);
		foreach(XmlNode node in del)
		{
			root.RemoveChild(node);
		}
		
		XmlElement element = xmlDoc.CreateElement("Stage_" + lv);
		root.AppendChild(element);
		#endregion
		#region - stage setting -
		XmlElement roamer = xmlDoc.CreateElement("Roamer");
		element.AppendChild(roamer);
		for(int j=0; j<10; ++j)
		{
			int count = m_RoamerInfo.count_Lv_[j];
			roamer.SetAttribute("lv." + (j + 1), count.ToString());
		}
		
		XmlElement stone = xmlDoc.CreateElement("Stone");
		element.AppendChild(stone);
		for(int j=0; j<10; ++j)
		{
			int count = m_StoneInfo.count_Lv_[j];
			stone.SetAttribute("lv." + (j + 1), count.ToString());
		}
		
		XmlElement stalker = xmlDoc.CreateElement("Stalker");
		element.AppendChild(stalker);
		for(int j=0; j<10; ++j)
		{
			int count = m_StalkerInfo.count_Lv_[j];
			stalker.SetAttribute("lv." + (j + 1), count.ToString());
		}
		
		XmlElement bullet = xmlDoc.CreateElement("Bullet");
		element.AppendChild(bullet);
		for(int j=0; j<10; ++j)
		{
			int count = m_BulletInfo.count_Lv_[j];
			bullet.SetAttribute("lv." + (j + 1), count.ToString());
		}
		
		XmlElement ghost = xmlDoc.CreateElement("Ghost");
		element.AppendChild(ghost);
		ghost.SetAttribute("Init_Speed", m_GhostInfo.initSpeed_.ToString());
		ghost.SetAttribute("Delta_Speed", m_GhostInfo.deltaSpeed_.ToString());
		ghost.SetAttribute("Interval", m_GhostInfo.interval_.ToString());
		
		XmlElement boss = xmlDoc.CreateElement("Boss");
		element.AppendChild(boss);
		boss.SetAttribute("lv", m_BossLv.ToString());
		
		XmlElement stageSize = xmlDoc.CreateElement("Stage_Size");
		element.AppendChild(stageSize);
		float size = YStageManager.s_StdStageSize / GetComponent<YStageManager>().GetStageExtent();
		stageSize.SetAttribute("Size", size.ToString());
		
		XmlElement difficulty = xmlDoc.CreateElement("Diffculty");
		element.AppendChild(difficulty);
		difficulty.SetAttribute("Difficulty",difficulty_.ToString());
		#endregion
		#region - sort & save -
		List<XmlNode> listNode = new List<XmlNode>();
		XmlNodeList list = root.ChildNodes;
		foreach(XmlNode node in list)
		{
			listNode.Add(node);
		}
		
		listNode.Sort(delegate(XmlNode _a, XmlNode _b) {
			
			string[] strA = _a.LocalName.Split('_');
			string aa = strA[strA.Length - 1];
			string[] strB = _b.LocalName.Split('_');
			string bb = strB[strB.Length - 1];
			
			int a = int.Parse(aa);
			int b = int.Parse(bb);
			
			if(a < b)
				return -1;
			else if(a == b)
				return 0;
			else
				return 1;
			
		});
		
		root.RemoveAll();
		foreach(XmlNode node in listNode)
		{
			root.AppendChild(node);
		}
		
		TextWriter tw = new StreamWriter(Application.dataPath + "/Resources/Asset/StageInfo.xml", false, Encoding.UTF8);
		xmlDoc.Save(tw);
		tw.Close();
		#endregion
#endif
	}
	
	void ResetStageInfo()
	{
		#region - reset -
		for(int i=0; i<10; ++i)
		{
			m_RoamerInfo.count_Lv_[i] = 0;
		}
		
		for(int i=0; i<10; ++i)
		{
			m_StoneInfo.count_Lv_[i] = 0;
		}
		
		for(int i=0; i<10; ++i)
		{
			m_StalkerInfo.count_Lv_[i] = 0;
		}
		
		for(int i=0; i<10; ++i)
		{
			m_BulletInfo.count_Lv_[i] = 0;
		}
		#endregion
	}
}

