using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class InGame : MonoBehaviour
{
    #region - singleton -
    static InGame _instance;
    
    public static InGame Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    [SerializeField] Vector3 _resumeTextPumpingScale = new Vector3(2.0f, 2.0f, 2.0f);
    [SerializeField] float _pumpingValue = 0.2f;
    [SerializeField] float _waitTime = 1.5f;
    [SerializeField] float _lifeTime = 1.0f;

    [SerializeField] Text _message;
    [SerializeField] GameObject _pauseButton;
    [SerializeField] Text _levelUp;
    
    [SerializeField] GameObject _pause;
    [SerializeField] GameObject _gameOver;
    [SerializeField] GameObject _stageClear;

    [SerializeField] Text _tutorial;

	static bool m_GamePaused; public static bool GamePaused { get { return m_GamePaused; } }

    Light _light;
	float _storedTimeScale = 0.0f;
	
	#region - init & update -
	void Awake()
	{
		_instance = this;
	}

    IEnumerator Start()
    {
        _light = GameObject.Find("LightObject").GetComponent<Light>();

        _pauseButton.SetActive(false);
        _levelUp.gameObject.SetActive(false);

        _pause.SetActive(false);
        _gameOver.SetActive(false);
        _stageClear.SetActive(false);

        _tutorial.gameObject.SetActive(false);

        _message.gameObject.SetActive(true);
        _message.text = "Ready";

        yield return new WaitForSeconds(_waitTime);

        _pauseButton.SetActive(true);

        _message.text = "GO!!";

        Hashtable hash = new Hashtable();
        hash.Add("scale", _resumeTextPumpingScale);
        hash.Add("time", _lifeTime * _pumpingValue);
        hash.Add("ignoretimescale", true);
        hash.Add("looptype", iTween.LoopType.pingPong);

        iTween.ScaleTo(_message.gameObject, hash);

        yield return new WaitForSeconds(_lifeTime);

        _message.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!m_GamePaused)
                OnPause();
            else
                OnResume();
        }
    }
	#endregion

    #region - level up -
    public void ShowLevelUp()
    {
        StopCoroutine("UpdateLevelUp");
        StartCoroutine("UpdateLevelUp");
    }

    IEnumerator UpdateLevelUp()
    {
        const float LIFE_TIME = 2.0f;
        const float COLOR_LERP_CORRECTION = 0.8f;
        const float MOVE_CORRECTION = 100.0f;

        _levelUp.gameObject.SetActive(true);

        float accumulatedTimeSec = 0.0f;

        float lerp = 0.0f;
        Color color = GetRandomColor();

        RectTransform levelUpTransform = _levelUp.GetComponent<RectTransform>();

        levelUpTransform.anchoredPosition = Vector2.zero;

        while (accumulatedTimeSec < LIFE_TIME)
        {
            yield return null;

            //
            lerp += Time.deltaTime * COLOR_LERP_CORRECTION;
            _levelUp.color = Color.Lerp(color, Color.white, lerp);

            //
            Vector2 position = levelUpTransform.anchoredPosition;
            position.y += MOVE_CORRECTION * Time.deltaTime;
            levelUpTransform.anchoredPosition = position;

            accumulatedTimeSec += Time.deltaTime;
        }

        _levelUp.gameObject.SetActive(false);
    }

    public void ShowStageUp()
    {
        StopCoroutine("UpdateStageUp");
        StartCoroutine("UpdateStageUp");
    }

    IEnumerator UpdateStageUp()
    {
        const float LIFE_TIME = 2.0f;
        const float COLOR_LERP_CORRECTION = 0.8f;
        const float MOVE_CORRECTION = 100.0f;

        _levelUp.gameObject.SetActive(true);

        float accumulatedTimeSec = 0.0f;

        float lerp = 0.0f;
        Color color = GetRandomColor();

        RectTransform levelUpTransform = _levelUp.GetComponent<RectTransform>();

        levelUpTransform.anchoredPosition = Vector2.zero;

        while (accumulatedTimeSec < LIFE_TIME)
        {
            yield return null;

            //
            lerp += Time.deltaTime * COLOR_LERP_CORRECTION;
            _levelUp.color = Color.Lerp(color, Color.white, lerp);

            //
            Vector2 position = levelUpTransform.anchoredPosition;
            position.y += MOVE_CORRECTION * Time.deltaTime;
            levelUpTransform.anchoredPosition = position;

            accumulatedTimeSec += Time.deltaTime;
        }

        _levelUp.gameObject.SetActive(false);
    }

    Color GetRandomColor()
    {
        switch (Random.Range(0, 6))
        {
            case 0: return Color.blue;
            case 1: return Color.cyan;
            case 2: return Color.magenta;
            case 3: return Color.green;
            case 4: return Color.red;
            case 5: return Color.yellow;
        }

        return Color.black;
    }
    #endregion

    #region - pause -
    public void OnPause()
    {
        m_GamePaused = true;

        _storedTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        _light.intensity = 0.2f;

        SimplePad.TurnOff();

        _pauseButton.SetActive(false);

        _pause.SetActive(true);
    }

    public void OnResume()
    {
        m_GamePaused = false;

        Time.timeScale = _storedTimeScale;

        _light.intensity = 0.5f;

        SimplePad.TurnOn();

        _pauseButton.SetActive(true);

        _pause.SetActive(false);
    }

    public void OnGoToTitle()
    {
        MainFlow.Instance.SceneConversion("Title");

        Time.timeScale = _storedTimeScale;
    }
    #endregion - pause -

    #region - player death -
    public void PlayerDeath()
    {
        _pauseButton.SetActive(false);

        SoundManager.Instance.SetPitch(1.0f);

        StartCoroutine(DelayPlayerDeath());
    }

    public void OnGoToStageSelect()
    {
        StartCoroutine(DelaySceneConversion("Stage_Select_New", 0.5f));
    }

    public void OnRetry()
    {
        int sceneIndex = MainFlow.Instance.SelectedStageIdx;
        StartCoroutine(DelaySceneConversion(sceneIndex.ToString(), 0.5f));
    }

    IEnumerator DelayPlayerDeath()
    {
        yield return new WaitForSeconds(2.0f);

        _gameOver.SetActive(true);
    }

    IEnumerator DelaySceneConversion(string sceneName, float delayTimeSec)
    {
        yield return new WaitForSeconds(delayTimeSec);

        MainFlow.Instance.SceneConversion(sceneName);
    }
    #endregion

    #region - stage clear -
    public void StageClear()
    {
        _pauseButton.SetActive(false);

        StartCoroutine(DelayShowClearMessage());
    }

    IEnumerator DelayShowClearMessage()
    {
        yield return new WaitForSeconds(1.0f);

        _stageClear.SetActive(true);
    }
    #endregion

    #region - tutorial - 
    const string TUTORIAL_TWEEN_NAME = "tutorial";
    const float TUTORIAL_POP_UP_TIME = 0.5f;

    public void ShowTutorial(string message, float scaleMultiply)
    {
        _tutorial.text = message;

        _tutorial.gameObject.SetActive(true);
        _tutorial.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

        iTween.StopByName(_tutorial.gameObject, TUTORIAL_TWEEN_NAME);

        Hashtable hash = new Hashtable();
        hash.Add("name", TUTORIAL_TWEEN_NAME);
        hash.Add("scale", Vector3.one * scaleMultiply);
        hash.Add("time", TUTORIAL_POP_UP_TIME);
        hash.Add("ignoretimescale", true);

        iTween.ScaleTo(_tutorial.gameObject, hash);
    }

    public void HideTutorial()
    {
        iTween.StopByName(_tutorial.gameObject, TUTORIAL_TWEEN_NAME);

        _tutorial.gameObject.SetActive(false);
    }

    #endregion
}