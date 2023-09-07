using UnityEngine;
using System.Collections;

public class SimplePad : MonoBehaviour
{
    enum State
    {
        None,
        Down,
        Hold,
        Up,
    }
	
	enum eSpeedBtn {Off, On}

    static SimplePad _instance;
	
	static YMover s_PlayerMover = null;

    [SerializeField] Texture _background;
    [SerializeField] Texture _pad;
	[SerializeField] Texture _speed;
	[SerializeField] GameObject _arrow;
	[SerializeField] GameObject _pointer;
    [SerializeField] float _radius = 50.0f;
    //	[SerializeField] float _revision = 0.25f;
    //	[SerializeField] float _maxSqrRatio = 10f;

    State _state = State.None;
    //	eSpeedBtn _speedBtn = eSpeedBtn.Off;
    //	Vector2 _speedBtnPos = Vector2.zero;
    Vector2 _centerPosition = Vector2.zero;
    Vector2 _currentPosition = Vector2.zero;
    Vector2 _ratio = Vector2.zero;
	
	Vector3 _arrowScale = Vector3.zero;
	
	[SerializeField] float posRev = 2f;
	[SerializeField] float speedThresh = 0.4f;
	[SerializeField] float lowSpeed = 0.2f;

    Material matPointer;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        matPointer = _pointer.GetComponent<Renderer>().material;
    }
	
	void Start()
	{
		_arrowScale = _arrow.transform.localScale;
		
//		_speedBtnPos = new Vector2( Screen.width * 0.15f, Screen.height * 0.2f);
		_currentPosition = _centerPosition = new Vector2( Screen.width * 0.15f, Screen.height * 0.2f);
	}
	
	void OnDisable()
	{
		_arrow.transform.position = new Vector3(99999f, 99999f, 99999f);
		_pointer.transform.position = new Vector3(99999f, 99999f, 99999f);
	}
	
	Vector2 __currentPosition;
	Vector2 __position;
	float __ratio;
	float __adjust;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _state = State.Down;

            _centerPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            _state = State.Hold;

            __currentPosition = Input.mousePosition;

            __position = __currentPosition - _centerPosition;
			__ratio = __position.magnitude;
            if (__ratio >= _radius)
            {
                __position = __position.normalized * _radius;
				__ratio = _radius; //revised
            }

            _ratio = __position / _radius * posRev;

            _currentPosition = _centerPosition + __position;
			
//			if((__currentPosition - _speedBtnPos).magnitude < _speed.width)
//				_speedBtn = eSpeedBtn.On;
//			else
//				_speedBtn = eSpeedBtn.Off;
			
			if(s_PlayerMover != null)
			{
//				__adjust = ( __ratio * _revision / _radius);
//				__adjust = ( __ratio / _radius) * 0.5f ;
				
//				if(_speedBtn == eSpeedBtn.On)
//					__adjust *= 2f;
				
//				if(__adjust > 1f)
//					__adjust = 1f;
//				
//				s_PlayerMover.SpeedModified(__adjust);
				
				float adjust = _ratio.sqrMagnitude;
				if( adjust > speedThresh)
					s_PlayerMover.SpeedModified( 1f);
				else
					s_PlayerMover.SpeedModified( lowSpeed);
			}
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _state = State.Up;

            _currentPosition = _centerPosition;
        }
        else
        {
            _state = State.None;
            _ratio = Vector2.zero;
        }
    }
	
	Vector3 __dir;
	Vector3 __pos;
	Vector3 __rotate;
    Vector3 __scale = new Vector3(0.3f, 0f, 0.3f);
	void LateUpdate()
	{
		Player player = YEntityManager.Instance.PlayerEntity;
		if(_ratio != Vector2.zero && player != null && player.Living == true)
		{
			__dir = new Vector3(SimplePad.ratio.x, 0, SimplePad.ratio.y) * (0.4f + player.Level * 0.1f);
			
			#region - pointer -
			_pointer.transform.position = __dir * posRev + player.transform.position;
			_pointer.transform.LookAt(player.transform.position);
			__rotate = _pointer.transform.eulerAngles;
			__rotate.y = __rotate.y + 180f;
			_pointer.transform.eulerAngles = __rotate;

            __scale.z = __dir.magnitude;
            _pointer.transform.localScale = __scale;
			#endregion
			
			#region - arrow -
			__pos = __dir * (1 + player.Level * 0.1f) / posRev + player.transform.position;
			_arrow.transform.position = __pos;
			_arrow.transform.LookAt(player.transform.position);
			__rotate = _arrow.transform.eulerAngles;
			__rotate.y = __rotate.y + 180f;
			_arrow.transform.eulerAngles = __rotate;
			#endregion
		}
		else
		{
			_arrow.transform.position = new Vector3(99999f, 99999f, 99999f);
			_pointer.transform.position = new Vector3(99999f, 99999f, 99999f);
		}
	}

    void OnGUI()
    {
        //        GUI.TextField(new Rect(10.0f, 10.0f, 300.0f, 20.0f), "Ratio: X - " + _ratio.x + ", Y - " + _ratio.y);

        if (_state == State.Hold)
        {
            if (_background != null)
            {
                Rect backgroundRect = new Rect(_centerPosition.x - _background.width * 0.5f,
                                               Screen.height - (_centerPosition.y + _background.height * 0.5f),
                                               _background.width,
                                               _background.height);

                GUI.DrawTexture(backgroundRect, _background);
            }

            if (_pad != null)
            {
                Rect padRect = new Rect(_currentPosition.x - _pad.width * 0.5f,
                                        Screen.height - (_currentPosition.y + _pad.height * 0.5f),
                                        _pad.width,
                                        _pad.height);

                GUI.DrawTexture(padRect, _pad);
            }

//			if (_speed != null)
//			{
//				Rect speedRect = new Rect(_speedBtnPos.x - _speed.width * 0.5f,
//	                                        Screen.height - (_speedBtnPos.y + _speed.height * 0.5f),
//	                                        _speed.width,
//	                                        _speed.height);
//	
////	            GUI.DrawTexture(speedRect, _speed);
//			
//				if(GUI.RepeatButton(speedRect, _speed) == true)
//				{
//					_speedBtn = eSpeedBtn.On;
//				}
//				else
//				{
//					_speedBtn = eSpeedBtn.Off;
//				}
//			}
        }
    }

    public static Vector2 ratio
    {
        get
        {
            return _instance._ratio;
        }
    }
	
	public static void DestroyThis()
	{
		if(_instance != null)
			Destroy(_instance.gameObject);
	}
	
	public static void PlayerInitiated(Player _player)
	{
		s_PlayerMover = _player.GetComponent<YMover>();
	}

    public static void PlayerGetExp(Color color)
    {
        _instance.matPointer.SetColor("_TintColor", color);
    }

    public static void PlayerLevelUp()
	{
		Player player = YEntityManager.Instance.PlayerEntity;
		if(player != null && player.Living == true && _instance != null)
			_instance._arrow.transform.localScale = _instance._arrowScale * (1 + player.Level * 0.1f);
	}
	
	public static void TurnOff()
	{
		if(_instance != null)
			_instance.enabled = false;
		
	}
	
	public static void TurnOn()
	{
		if(_instance != null)
			_instance.enabled = true;
	}
}