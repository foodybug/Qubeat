using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	#region - singleton -
	static SoundManager m_Instance; public static SoundManager Instance{get{return m_Instance;}}
	#endregion
	//[SerializeField] GUIText musicName_;

	[SerializeField] List<AudioData> listEffect_;
	[SerializeField] List<AudioData> listBgm_;
	[SerializeField] List<AudioData> listStage_;

	Dictionary<string, AudioData> m_dicEffect = new Dictionary<string, AudioData>();
	Dictionary<string, AudioData> m_dicBgm = new Dictionary<string, AudioData>();
	Dictionary<string, AudioData> m_dicStage = new Dictionary<string, AudioData>();
	Dictionary<int, AudioData> m_dicStageIdx = new Dictionary<int, AudioData>();

	AudioSource m_Audio;

	AudioData m_CurMusic;

	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
		#region - audio set -
		m_Audio = GetComponent<AudioSource>();
		if(m_Audio == null)
			m_Audio = gameObject.AddComponent<AudioSource>();

		DontDestroyOnLoad(gameObject);
		#endregion
		#region - dictionary set -
		for(int i=0; i<listEffect_.Count; ++i)
		{
			m_dicEffect.Add(listEffect_[i].name, listEffect_[i]);
		}

		for(int i=0; i<listBgm_.Count; ++i)
		{
			m_dicBgm.Add(listBgm_[i].name, listBgm_[i]);
		}

		for(int i=0; i<listStage_.Count; ++i)
		{
			m_dicStage.Add(listStage_[i].name, listStage_[i]);
			m_dicStageIdx.Add(listStage_[i].idx, listStage_[i]);
		}
		#endregion
	}

	void Start()
	{
//		m_Audio.Play();
	}

	public void PlayEffect(string _name)
	{
		if(m_dicEffect.ContainsKey(_name) == true)
			m_Audio.PlayOneShot(m_dicEffect[_name].clip);
	}

	public void PlayBgm(string _name)
	{
		#region - same music -
		if(m_CurMusic != null && m_CurMusic.name == _name)
		{
			Debug.Log("SoundManager:: PlayBgm: same music. skip this");
			return;
		}
		#endregion

		if(m_dicBgm.ContainsKey(_name) == true)
			_PlayMusic(m_dicBgm[_name]);
	}

	public void PlayStage(string _name)
	{
		#region - same music -
		if(m_CurMusic != null && m_CurMusic.name == _name)
		{
			Debug.Log("SoundManager:: PlayStage: same music. skip this");
			return;
		}
		#endregion

		if(m_dicStage.ContainsKey(_name) == true)
			_PlayMusic(m_dicStage[_name]);
	}

	void _PlayMusic(AudioData _data)
	{
		StartCoroutine(_ChangeMusic_CR(_data));
	}

	IEnumerator _ChangeMusic_CR(AudioData _data)
	{
		if(m_CurMusic != null)
		{
			float decreaseSpeed = 2f;
			while(true)
			{
				m_Audio.volume -= Time.deltaTime * decreaseSpeed;
				if(m_Audio.volume <= 0f)
					break;

				yield return null;
			}
		}

		m_CurMusic = _data;
		m_Audio.clip = m_CurMusic.clip;
		m_Audio.Play();
		m_Audio.pitch = 1f;
		m_Audio.volume = 1f;
		
		//musicName_.text = m_CurMusic.display;
	}

	public void PlayStageByIndex(int _idx)
	{
		int idx = _idx % m_dicStageIdx.Count;
		if(m_dicStageIdx.ContainsKey(idx) == true)
			_PlayMusic(m_dicStageIdx[idx]);
		else
			Debug.LogError("SoundManager:: PlayStageByIndex: invalid stage index = " + _idx);
	}

	public void SetPitch(float _pitch)
	{
		if(m_CurMusic != null)
			m_Audio.pitch = _pitch;
	}

	public bool IsPlaying(string _name)
	{
		if(m_CurMusic == null)
			return false;

		return m_CurMusic.name == _name;
	}

    public string currentMusicName
    {
        get
        {
            return (m_CurMusic != null) ? m_CurMusic.display : "";
        }
    }
}

[System.Serializable]
public class AudioData
{
	[SerializeField] int idx_; public int idx{get{return idx_;}}
	[SerializeField] string name_; public string name{get{return name_;}}
	[SerializeField] string display_; public string display{get{return display_;}}
	[SerializeField] AudioClip clip_; public AudioClip clip{get{return clip_;}}
}