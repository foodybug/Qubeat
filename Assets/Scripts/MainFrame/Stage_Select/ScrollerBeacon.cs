using UnityEngine;
using System.Collections;

public class ScrollerBeacon : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
//	IEnumerator OnMouseDown()
//    {
//		if(Stage_Select.EditMode == true)
//		{
//	        Vector3 scrSpace = Camera.main.WorldToScreenPoint (transform.position);
//	        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, scrSpace.z));
//	
//	        while (Input.GetMouseButton(0))
//	        {
//	            Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, scrSpace.z);
//	
//	            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
//	
//	            transform.position = curPosition;
//						
//	            yield return null;
//	        }
//			
//			SendMessageUpwards("SetDragArea_Stage_Scroller");
//		}
//    }
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position, 0.1f);
	}
}

