using System.Collections;

using UnityEngine;

public class Developer_Mark : MonoBehaviour
{
    [SerializeField] float waitTime_LogoVisibile_ = 1.0f;
	[SerializeField] float waitTime_InputEnable_ = 3.0f;
    [SerializeField] GameObject logo_;
    [SerializeField] GameObject retroLogo_;

	IEnumerator Start()
    {
        logo_.SetActive(false);
        retroLogo_.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

        yield return new WaitForSeconds(waitTime_LogoVisibile_);

        logo_.SetActive(true);

        Hashtable hash = new Hashtable();
        hash.Add("y", 0.0f);
        hash.Add("time", waitTime_InputEnable_ * 0.5f);
        hash.Add("ignoretimescale", true);
        hash.Add("looptype", iTween.LoopType.none);
        hash.Add("easetype", iTween.EaseType.easeInElastic);
        iTween.ScaleTo(logo_, hash);

        hash.Clear();
        hash.Add("y", 1.0f);
        hash.Add("delay", waitTime_InputEnable_ * 0.5f);
        hash.Add("time", waitTime_InputEnable_ * 0.5f);
        hash.Add("ignoretimescale", true);
        hash.Add("looptype", iTween.LoopType.none);
        hash.Add("easetype", iTween.EaseType.easeInExpo);
        iTween.ScaleTo(retroLogo_, hash);

        yield return new WaitForSeconds(waitTime_InputEnable_);

        MainFlow.Instance.SceneConversion("Title");
	}
}