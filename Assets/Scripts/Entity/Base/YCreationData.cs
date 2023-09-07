using UnityEngine;
using System.Collections;

public abstract class YCreationData
{
    //public int id_;
    public Vector3 pos_;
	public int lv_;

    public bool isRegister_ = false;

    public YCreationData() { }

    public YCreationData(Vector3 _pos, int _lv)
    {
        //id_ = _id;
        pos_ = _pos;
        lv_ = _lv;
    }

    public void SetData(Vector3 _pos, int _lv, bool _register = false)
    {
        //id_ = _id;
        pos_ = _pos;
        lv_ = _lv;

        isRegister_ = _register;
    }

    public void Proc_RegisteredEntity()
    {
        if(isRegister_ == true)
            pos_ = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos(80f);
    }

    public abstract YBaseEntity Create();
}

public class CreationData_Player : YCreationData
{
	public CreationData_Player(Vector3 _pos, int _lv)
        : base(_pos, _lv)
	{
	}

    public override YBaseEntity Create()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Player/Player")) as GameObject;

        Player player = obj.AddComponent<Player>();
        player.Init();
        player.SetCreationData(this);

        YMover mover = obj.AddComponent<YMover>();
        mover.Init();

        YCollider collider = obj.AddComponent<YCollider>();
        collider.Init();
        collider.SetCollisionData(player.Level, new System.Type[] { typeof(Roamer) }, false);	

        YSpinner spinner = obj.AddComponent<YSpinner>();
        spinner.Init();

        return player;
    }
}

public class CreationData_Roamer : YCreationData
{
    static GameObject rootObj;

    public CreationData_Roamer() { }

    public CreationData_Roamer(Vector3 _pos, int _lv)
        : base(_pos, _lv)
    {
    }

    public override YBaseEntity Create()
    {
		GameObject obj = YEntityManager.Instance.GetCachedObject();
		if(obj == null)
			return null;

		obj.SetActive(true);

        Roamer entity = obj.AddComponent<Roamer>();
        entity.Init();
        entity.SetCreationData(this);
		entity.Init_AfterCreation();

        if(rootObj == null)
            rootObj = new GameObject("ROOT_Roamer");
        entity.transform.SetParent(rootObj.transform);

        return entity;
    }
}

public class CreationData_Stone : YCreationData
{
    static GameObject rootObj;

    public CreationData_Stone() { }

    public CreationData_Stone(Vector3 _pos, int _lv)
        : base(_pos, _lv)
    {
    }

    public override YBaseEntity Create()
    {
		GameObject obj = YEntityManager.Instance.GetCachedObject();
		if(obj == null)
			return null;

		obj.SetActive(true);

        Stone entity = obj.AddComponent<Stone>();
        entity.Init();
        entity.SetCreationData(this);
		entity.Init_AfterCreation();

        if (rootObj == null)
            rootObj = new GameObject("ROOT_Stone");
        entity.transform.SetParent(rootObj.transform);

        return entity;
    }
}

public class CreationData_Stalker : YCreationData
{
    static GameObject rootObj;

    public CreationData_Stalker() { }

    public CreationData_Stalker(Vector3 _pos, int _lv)
        : base(_pos, _lv)
    {
    }

    public override YBaseEntity Create()
    {
		GameObject obj = YEntityManager.Instance.GetCachedObject();
		if(obj == null)
			return null;

		obj.SetActive(true);

        Stalker entity = obj.AddComponent<Stalker>();
        entity.Init();
        entity.SetCreationData(this);
		entity.Init_AfterCreation();

        if (rootObj == null)
            rootObj = new GameObject("ROOT_Stalker");
        entity.transform.SetParent(rootObj.transform);

        return entity;
    }
}

public class CreationData_Bullet : YCreationData
{
    static GameObject rootObj;

    public CreationData_Bullet() { }

    public CreationData_Bullet(Vector3 _pos, int _lv)
        : base(_pos, _lv)
    {
    }

    public override YBaseEntity Create()
    {
		GameObject obj = YEntityManager.Instance.GetCachedObject();
		if(obj == null)
			return null;

		obj.SetActive(true);

        Bullet entity = obj.AddComponent<Bullet>();
        entity.Init();
        entity.SetCreationData(this);
		entity.Init_AfterCreation();

        if (rootObj == null)
            rootObj = new GameObject("ROOT_Bullet");
        entity.transform.SetParent(rootObj.transform);

        return entity;
    }
}

//public class CreationData_Dancer : YCreationData
//{
//    public CreationData_Dancer() { }

//    public CreationData_Dancer( Vector3 _pos, int _lv)
//        : base(_id, _pos, _lv)
//    {
//    }

//    public override YBaseEntity Create()
//    {
//        GameObject obj = YEntityManager.Instance.GetCachedObject();
//        if (obj == null)
//            return null;

//        obj.SetActive(true);

//        Dancer entity = obj.AddComponent<Dancer>();
//        entity.Init();
//        entity.SetCreationData(this);
//        entity.Init_AfterCreation();

//        return entity;
//    }
//}

//public class CreationData_Specter : YCreationData
//{
//    public CreationData_Specter() { }

//    public CreationData_Specter(Vector3 _pos, int _lv)
//        : base(_id, _pos, _lv)
//    {
//    }

//    public override YBaseEntity Create()
//    {
//        GameObject obj = YEntityManager.Instance.GetCachedObject();
//        if (obj == null)
//            return null;

//        obj.SetActive(true);

//        Specter entity = obj.AddComponent<Specter>();
//        entity.Init();
//        entity.SetCreationData(this);
//        entity.Init_AfterCreation();

//        return entity;
//    }
//}

public class CreationData_Boss : YCreationData
{
	public CreationData_Boss(Vector3 _pos, int _lv)
        : base(_pos, _lv)
    {
    }

    public override YBaseEntity Create()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Enemy/Boss/Boss")) as GameObject;

        Boss entity = obj.AddComponent<Boss>();
        entity.Init();
        entity.SetCreationData(this);

        YMover mover = obj.AddComponent<YMover>();
        mover.Init();

        YCollider collider = obj.AddComponent<YCollider>();
        collider.Init();
        collider.SetBossCollisionData(entity.Level, new System.Type[] { typeof(Player) }, true);

        return entity;
    }
}

public class CreationData_Ghost : YCreationData
{
	public float speed_;
	
	public CreationData_Ghost( Vector3 _pos, int _lv)
        : base(_pos, 0)
	{
	}

    public void SetSpeed(float _speed)
    {
        speed_ = _speed;
    }

    public override YBaseEntity Create()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Enemy/Ghost/Ghost")) as GameObject;

        Ghost entity = obj.AddComponent<Ghost>();
        entity.Init();
        entity.SetCreationData(this);

        YMover mover = obj.AddComponent<YMover>();
        mover.Init();

        YCollider collider = obj.AddComponent<YCollider>();
        collider.Init();
        collider.SetGhostCollisionData(entity.Level, new System.Type[] { typeof(Player) }, true);
        collider.SetRefreshRate(0.1f);

        return entity;
    }
}