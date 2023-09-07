using UnityEngine;
using System.Collections;

public class UI_Alignment : ScriptableObject {
	
	public InventoryAlignment Inventory_;
}

[System.Serializable]
public class InventoryAlignment
{
	public Rect bgPanel_;
	
	public Rect itemButton_;
}