//#define _EDITOR

using UnityEngine;
#if _EDITOR
using UnityEditor;
#endif

using System.Xml;
using System.Collections;
using System.IO;
using System.Text;

public class StageLog : MonoBehaviour {
	
#if _EDITOR
	[MenuItem ("MyMenu/Calculate Stage Difficulty")]
	static void CalculateStageDifficulty()
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
		
		foreach(XmlNode node in root)
		{
			XmlElement element = node as XmlElement;
			XmlElement roamer = element.GetElementsByTagName("Roamer")[0] as XmlElement;
			XmlElement stone = element.GetElementsByTagName("Stone")[0] as XmlElement;
			XmlElement stalker = element.GetElementsByTagName("Stalker")[0] as XmlElement;
			XmlElement bullet = element.GetElementsByTagName("Bullet")[0] as XmlElement;
			XmlElement ghost = element.GetElementsByTagName("Ghost")[0] as XmlElement;
			XmlElement boss = element.GetElementsByTagName("Boss")[0] as XmlElement;
			XmlElement size = element.GetElementsByTagName("Stage_Size")[0] as XmlElement;
			XmlElement __difficulty = element.GetElementsByTagName("Difficulty")[0] as XmlElement;
			
			float difficulty = 0;
			string difficulty_ = "";
			
			//roamer
			for(int i=0; i<10; ++i)
			{
				float count = float.Parse(roamer.GetAttribute("lv." + (i+1)));
				float v = Mathf.Pow(i, 1.4f) * count;
				if(i == 0) v = -0.7f * count;
				difficulty += v;
			}
			//stone
			for(int i=0; i<10; ++i)
			{
				float count = float.Parse(stone.GetAttribute("lv." + (i+1)));
				float v = Mathf.Pow(i, 1.2f) * count;
				if(i == 0) v = -0.9f * count;
				difficulty += v;
			}
			//stalker
			for(int i=0; i<10; ++i)
			{
				float count = float.Parse(stalker.GetAttribute("lv." + (i+1)));
				float v = Mathf.Pow(i, 1.8f) * count;
				if(i == 0) v = -0.5f * count;
				difficulty += v;
			}
			//bullet
			for(int i=0; i<10; ++i)
			{
				float count = float.Parse(bullet.GetAttribute("lv." + (i+1)));
				float v = Mathf.Pow(i, 2.2f) * count;
				if(i == 0) v = -0.1f * count;
				difficulty += v;
			}
			
			float dif1 = Mathf.Round(difficulty);
			difficulty_ =  "(" + dif1.ToString();
			float ghostInit = float.Parse(ghost.GetAttribute("Init_Speed"));
			float ghostDelta = float.Parse(ghost.GetAttribute("Delta_Speed"));
			float ghostInterval = float.Parse(ghost.GetAttribute("Interval"));
			
			difficulty += ghostInit * 30f * ghostDelta * 3f * 60f / ghostInterval;
			
			float bossLv = float.Parse(boss.GetAttribute("lv"));
			float dif2 = Mathf.Round(difficulty);
			difficulty_ += " + " + (dif2 - dif1).ToString();
			difficulty *= bossLv;
			
			float dif3 = Mathf.Round(difficulty);
			difficulty_ += ") * " + (dif3 / dif2).ToString();
			
			float dif4 = float.Parse(size.GetAttribute("Size"));
			dif4 = Mathf.Round(dif4);
			difficulty /= Mathf.Pow(dif4, 1.6f);
			
			difficulty_ += " / [" + dif4 + "(pow(1.5) = " + Mathf.Pow(dif4, 1.6f) + ")]";
			difficulty_ += " = " + Mathf.Round(difficulty).ToString();
			
			__difficulty.SetAttribute("Difficulty", difficulty_);
		}
		
		TextWriter tw = new StreamWriter(Application.dataPath + "/Resources/Asset/StageInfo.xml", false, Encoding.UTF8);
		xmlDoc.Save(tw);
		tw.Close();
		#endregion
	}
	
	static void ExtractStageInfo () {
		
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/Asset/StageInfo.txt", false);
		string content = "";
		
		for(int i=1; i<=status.LastStageIndex; ++i)
		{
			Application.LoadLevel(i.ToString());
			Debug.Log("stage = " + i + " is loaded");
			GameObject obj = GameObject.Find("GameMain");
			SceneSetter setter = obj.GetComponent<SceneSetter>();
			
			writer.WriteLine("Stage " + i + "\n");
			
			
			content = "";
			writer.WriteLine("\nRoamer");
			for(int j=0; j<10; ++j)
			{
				int count = setter.RoamerInfo.count_Lv_[j];
				if(count == 0)
					content += "(lv." + (j + 1) + ":" + count + ")";
				else
					content += "[LV." + (j + 1) + "==" + count + "]";
			}
			writer.WriteLine(content);
			
			content = "";
			writer.WriteLine("\nStone");
			for(int j=0; j<10; ++j)
			{
				int count = setter.StoneInfo.count_Lv_[j];
				if(count == 0)
					content += "(lv." + (j + 1) + ":" + count + ")";
				else
					content += "[LV." + (j + 1) + "==" + count + "]";
			}
			writer.WriteLine(content);
			
			content = "";
			writer.WriteLine("\nStalker");
			for(int j=0; j<10; ++j)
			{
				int count = setter.StalkerInfo.count_Lv_[j];
				if(count == 0)
					content += "(lv." + (j + 1) + ":" + count + ")";
				else
					content += "[LV." + (j + 1) + "==" + count + "]";
			}
			writer.WriteLine(content);
			
			content = "";
			writer.WriteLine("\nBullet");
			for(int j=0; j<10; ++j)
			{
				int count = setter.BulletInfo.count_Lv_[j];
				if(count == 0)
					content += "(lv." + (j + 1) + ":" + count + ")";
				else
					content += "[LV." + (j + 1) + "==" + count + "]";
			}
			writer.WriteLine(content);
			
			
			writer.WriteLine("\nGhost");
			writer.WriteLine("InitSpeed:" + setter.GhostInfo.initSpeed_);
			writer.WriteLine("DeltaSpeed:" + setter.GhostInfo.deltaSpeed_);
			writer.WriteLine("Interval:" + setter.GhostInfo.interval_);
			writer.WriteLine("");
			
			setter.CalculateDifficulty();
			writer.WriteLine("Difficuly = " + setter.difficulty_);
			
			writer.WriteLine("\n\n");
			
			
		}
		
		writer.Close();
	}
#endif
	
//	[MenuItem ("MyMenu/Import stage info(Be Careful!!)")]
//	static void ImportStageInfo () {
//		
//		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
//		StreamReader reader = new StreamReader(Application.dataPath + "/Resources/Asset/StageInfo.txt", false);
//		string content = "";
//		
//		for(int i=1; i<=status.LastStageIndex; ++i)
//		{
//			Application.LoadLevel(i.ToString());
//			Debug.Log("stage = " + i + " is loaded");
//			GameObject obj = GameObject.Find("GameMain");
//			SceneSetter setter = obj.GetComponent<SceneSetter>();
//			
//			writer.WriteLine("Stage " + i + "\n");
//			
//			
//			content = "";
//			writer.WriteLine("\nRoamer");
//			for(int j=0; j<10; ++j)
//			{
//				int count = setter.RoamerInfo.count_Lv_[j];
//				if(count == 0)
//					content += "(lv." + (j + 1) + ":" + count + ")";
//				else
//					content += "[LV." + (j + 1) + "==" + count + "]";
//			}
//			writer.WriteLine(content);
//			
//			content = "";
//			writer.WriteLine("\nStone");
//			for(int j=0; j<10; ++j)
//			{
//				int count = setter.StoneInfo.count_Lv_[j];
//				if(count == 0)
//					content += "(lv." + (j + 1) + ":" + count + ")";
//				else
//					content += "[LV." + (j + 1) + "==" + count + "]";
//			}
//			writer.WriteLine(content);
//			
//			content = "";
//			writer.WriteLine("\nStalker");
//			for(int j=0; j<10; ++j)
//			{
//				int count = setter.StalkerInfo.count_Lv_[j];
//				if(count == 0)
//					content += "(lv." + (j + 1) + ":" + count + ")";
//				else
//					content += "[LV." + (j + 1) + "==" + count + "]";
//			}
//			writer.WriteLine(content);
//			
//			content = "";
//			writer.WriteLine("\nBullet");
//			for(int j=0; j<10; ++j)
//			{
//				int count = setter.BulletInfo.count_Lv_[j];
//				if(count == 0)
//					content += "(lv." + (j + 1) + ":" + count + ")";
//				else
//					content += "[LV." + (j + 1) + "==" + count + "]";
//			}
//			writer.WriteLine(content);
//			
//			
//			writer.WriteLine("\nGhost");
//			writer.WriteLine("InitSpeed:" + setter.GhostInfo.initSpeed_);
//			writer.WriteLine("DeltaSpeed:" + setter.GhostInfo.deltaSpeed_);
//			writer.WriteLine("Interval:" + setter.GhostInfo.interval_);
//			
//			writer.WriteLine("\n\n");
//		}
//		
//		writer.Close();
//	}
	
//	public static DungeonInfo XmlLoad( TextAsset _xml )
//	{
//		XmlDocument doc = new XmlDocument();
//		
//		try
//		{
//			doc.LoadXml( _xml.text );
//		}
//		catch( XmlException e )
//		{
//			Debug.Log( e.Message );
//			return null;
//		}
//		
//		XmlElement rootElement = doc.DocumentElement;
//		if( rootElement == null )
//		{
//			Debug.Log( "Xml Root Have Not" );
//			return null;
//		}
//		
//		XmlNodeList nodes = rootElement.ChildNodes;
//		if( nodes == null )
//		{
//			Debug.Log( "No Nodes" );
//			return null;
//		}
//		
//		DungeonInfo dungeonInfo = new DungeonInfo();
//		foreach( XmlNode node in nodes )
//		{
//			XmlElement element = (XmlElement)node;
//			switch( element.Name )
//			{
//			case "DUNGEON_INFO" : 
//				try
//				{	
//					LoadDungeonInfo( ref dungeonInfo, element );
//				}
//				catch( Exception e )
//				{
//					Debug.Log( e.Message );
//				}
//				break;
//			case "FIELD" :
//				try
//				{
//					LoadField( ref dungeonInfo, element );
//				}
//				catch( Exception e )
//				{
//					Debug.Log( e.Message );
//				}
//				break;				
//			}
//		}
//		
//		return dungeonInfo;
//	}
//	
//	
//	static void LoadDungeonInfo( ref DungeonInfo _dungeonInfo, XmlElement _element )
//	{
//		_dungeonInfo.dungeonID = Convert.ToInt32( _element.GetAttribute( "dungeonID" ) );
//		_dungeonInfo.dungeonName = _element.GetAttribute( "dungeonName" );
//		_dungeonInfo.bgm = _element.GetAttribute( "bgm" );
//		_dungeonInfo.ver = Convert.ToSingle( _element.GetAttribute( "ver" ) );
//	}
//	
//	static void LoadField( ref DungeonInfo _dungeonInfo, XmlElement _element )
//	{
//		BattleFieldInfo fieldInfo = new BattleFieldInfo();
//		XmlNodeList nodes = _element.ChildNodes;
//		foreach( XmlNode node in nodes )
//		{
//			XmlElement element = (XmlElement)node;
//			switch( element.Name )
//			{
//			case "FIELD_ID" :
//				fieldInfo.fieldID = Convert.ToInt32( element.GetAttribute( "fieldID" ) );
//				break;
//			case "SOUND" :
//				fieldInfo.bgm = element.GetAttribute( "bgm" );
//				break;
//			case "BASETILE" :
//				fieldInfo.baseTile = element.GetAttribute( "baseTile" );
//				break;
//			case "DECOTILE_LIST" :
//				LoadDecoTile( ref fieldInfo, element );
//				break;
//			case "SKILLTILE_LIST" :
//				LoadSkillTile( ref fieldInfo, element );
//				break;
//			case "MONSTER_LIST" :
//				LoadMonsterList( ref fieldInfo, element );
//				break;
//				
//			}
//		}
//		_dungeonInfo.battleFieldDic.Add( fieldInfo.fieldID, fieldInfo );
//	}
//	
//	static void LoadDecoTile( ref BattleFieldInfo _fieldInfo, XmlElement _element )
//	{
//		XmlNodeList nodes = _element.ChildNodes;
//		foreach( XmlNode node in nodes )
//		{
//			XmlElement element = (XmlElement)node;
//			
//			TileInfo tileInfo = new TileInfo();			
//			tileInfo.posX = Convert.ToInt32( element.GetAttribute( "posX" ) );
//			tileInfo.posY = Convert.ToInt32( element.GetAttribute( "posY" ) );
//			tileInfo.tileName = element.GetAttribute( "tileName" );
//			_fieldInfo.decoTileList.Add( tileInfo );
//		}
//	}
//	
//	static void LoadSkillTile( ref BattleFieldInfo _fieldInfo, XmlElement _element )
//	{
//		XmlNodeList nodes = _element.ChildNodes;
//		foreach( XmlNode node in nodes )
//		{
//			XmlElement element = (XmlElement)node;
//			
//			TileInfo tileInfo = new TileInfo();			
//			tileInfo.posX = Convert.ToInt32( element.GetAttribute( "posX" ) );
//			tileInfo.posY = Convert.ToInt32( element.GetAttribute( "posY" ) );
//			tileInfo.tileName = element.GetAttribute( "tileName" );
//			_fieldInfo.skillTileList.Add( tileInfo );
//		}
//	}
//	
//	static void LoadMonsterList( ref BattleFieldInfo _fieldInfo, XmlElement _element )
//	{
//		XmlNodeList nodes = _element.ChildNodes;
//		foreach( XmlNode node in nodes )
//		{
//			XmlElement element = (XmlElement)node;
//			
//			MonsterOnTile onTile = new MonsterOnTile();
//			onTile.fieldID = Convert.ToInt32( element.GetAttribute( "fieldID" ) );
//			onTile.objectID = Convert.ToInt32( element.GetAttribute( "monsterID" ) );
//			
//			// test
//			onTile.resourceName = element.GetAttribute( "resourceName" );
//			
//			onTile.posX = Convert.ToInt32( element.GetAttribute( "posX" ) );
//			onTile.posY = Convert.ToInt32( element.GetAttribute( "posY" ) );
//			
//			_fieldInfo.monsterList.Add( onTile );
//		}
//	}
}
