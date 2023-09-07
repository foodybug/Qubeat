using UnityEngine;
using System.Collections;

public abstract class YBaseEntity : YBaseComponent {
	
	//#region - info -
	protected int m_Id = -1; public int Id {get{return m_Id;}}
//	public abstract eEntityType EntityType{get;}	
	
//	protected eRegion m_Region;public eRegion Region{get{return m_Region;}}
	//#endregion
	
	//#region - entity -
//	protected static YEntityManager s_EntityManager = null;
	//#endregion
	
	//#region - status -
	[SerializeField] protected int m_Level = 1;public int Level{get{return m_Level;}}
	[SerializeField] protected float m_Hp;public float Hp{get{return m_Hp;}}
	[SerializeField] protected float m_HpMax;public float HpMax{get{return m_HpMax;}}
	
	protected bool m_Living = true;public bool Living{get{return m_Living;}}
	protected Color m_Color; public Color color{get{return m_Color;}}
	//#endregion
	
	//#region - init & creation -
	public override void Init()
	{
		base.Init();
		
		m_Id = gameObject.GetInstanceID();
	}
	
	public abstract void SetCreationData(YCreationData _data);
	//#endregion

	public virtual void Init_AfterCreation(){}

	static YMessage msgBlank = new Msg_Blank();
	public void CoroutineProc(string cr, bool activate, object v = null)
	{
		if (cr.Contains("_CR") == false)
			cr = cr + "_CR";

		if (v == null)
			v = msgBlank;

		if (activate == true)
			StartCoroutine(cr, v);
		else
			StopCoroutine(cr);
	}

	#region - object operation -
	protected void AdjustVertices(int _lv, float _revision, bool reset = false)
	{
//		MeshFilter filter = GetComponent<MeshFilter>();
//		Vector3[] prevVert = filter.mesh.vertices;
//		Vector2[] prevUv = filter.mesh.uv;
//		int[] prevTris = filter.mesh.triangles;
//		
//		filter.mesh.Clear();
//		
//		Vector3[] newVert = new Vector3[prevVert.Length];
//		for(int i=0; i<prevVert.Length; ++i)
//		{
//			newVert[i] = prevVert[i] * _lv * _revision;
//		}
//		
//		filter.mesh.vertices = newVert;
//		filter.mesh.uv = prevUv;
//		filter.mesh.triangles = prevTris;
		
		if(reset == false)
			transform.localScale *= _lv * _revision;
		else
			transform.localScale = Vector3.one * _lv * _revision;

		// exceptional case 
		if (_lv == 1)
		{
			SphereCollider col = GetComponent<Collider>() as SphereCollider;

			if(reset == false)
				col.radius *= 1.3f;
			else
				col.radius = 1.3f;
		}
	}
	
	protected void SetRandomColor()
	{
		_InitColor(new Vector2(Random.Range(0f, 1f), Random.Range(1 / EntityStatus.s_UnitUV, 1f)));
	}
	
	private void _InitColor(Vector2 _uv)
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] uv = mesh.uv;
		
		for(int i=0; i<uv.Length; ++i)
		{
			uv[i] = _uv;
		}
		
		mesh.uv = uv;
		
		EntityStatus status = Resources.Load("Asset/EntityStatus") as EntityStatus;
		m_Color = status.GetColorByUv(_uv);
	}
	
	protected void SetBlackColor()
	{
		_InitColor(new Vector2(0, 0));
	}

	protected void ChangeEdgeColor()
	{
	}
	#endregion
}