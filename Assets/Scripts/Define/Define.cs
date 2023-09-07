using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum eEntityType {Player, Stalker, Roamer, Boss}
public enum eRegion {NONE, Blue, Red}

public enum eItemType {Honey, Berserk, Exaltation, Assembly, Dissolution, Princess}

//public class RegionController
//{
//	static eRegion[] OppositeRegions_Blue = new eRegion[]{eRegion.Red_Spy, eRegion.Red_Queen, eRegion.Red_Egg, eRegion.Roamer};
//	static eRegion[] OppositeRegions_Red = new eRegion[]{eRegion.Blue_Spy, eRegion.Blue_Queen, eRegion.Blue_Egg, eRegion.Roamer};
//	static eRegion[] OppositeRegions_Bird = new eRegion[]{eRegion.Blue_Spy, eRegion.Red_Spy};
//	
//	public static eRegion[] GetOppositeRegion(eRegion _region)
//	{
//		switch(_region)
//		{
//		case eRegion.Blue_Spy:
//			return OppositeRegions_Blue;
//		case eRegion.Red_Spy:
//			return OppositeRegions_Red;
//		case eRegion.Roamer:
//			return OppositeRegions_Bird;
//		default:
//			Debug.LogError("RegionController::GetOppositeRegion: invalid region access [" + _region + "]");
//			return null;
//		}
//	}
//	
//	public static eRegion GetQueenRegion(eRegion _region)
//	{
//		switch(_region)
//		{
//		case eRegion.Blue_Spy:
//		case eRegion.Blue_Egg:
//		case eRegion.Blue_Queen:
//			return eRegion.Blue_Queen;
//		case eRegion.Red_Spy:
//		case eRegion.Red_Egg:
//		case eRegion.Red_Queen:
//			return eRegion.Red_Queen;
//		default:
//			Debug.LogError("RegionController::GetQueenRegion: invalid region access [" + _region + "]");
//			return eRegion.NONE;
//		}
//	}
//	
//	public static eRegion GetEggRegion(eRegion _region)
//	{
//		switch(_region)
//		{
//		case eRegion.Blue_Spy:
//		case eRegion.Blue_Queen:
//		case eRegion.Blue_Egg:
//			return eRegion.Blue_Egg;
//		case eRegion.Red_Spy:
//		case eRegion.Red_Queen:
//		case eRegion.Red_Egg:
//			return eRegion.Red_Egg;
//		default:
//			Debug.LogError("RegionController::GetEggRegion: invalid region access [" + _region + "]");
//			return eRegion.NONE;
//		}
//	}
//	
//	public static eRegion GetChildRegion(eRegion _region)
//	{
//		switch(_region)
//		{
//		case eRegion.Blue_Queen:
//			return eRegion.Blue_Spy;
//		case eRegion.Red_Queen:
//			return eRegion.Red_Spy;
//		default:
//			Debug.LogError("RegionController::GetChildRegion: invalid region access [" + _region + "]");
//			return eRegion.NONE;
//		}
//	}
//	
//	public static bool DecideSameRegion(eRegion _region1, eRegion _region2)
//	{
//		string[] regions1 = _region1.ToString().Split(new char[]{'_'});
//		string[] regions2 = _region2.ToString().Split(new char[]{'_'});
//		
//		if(regions1[0] == regions2[0])
//			return true;
//		else
//			return false;
//				
//	}
//}