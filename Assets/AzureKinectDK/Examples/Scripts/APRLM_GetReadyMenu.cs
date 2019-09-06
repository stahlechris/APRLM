using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using APRLM.Game;

public class APRLM_GetReadyMenu : MonoBehaviour
{
    [Tooltip("Manually dragged in, else last child")]
    public Text countdownText;
    [Tooltip("Manually dragged in, else 2nd child")]
    public Text poseNameText;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void OnEnable()
    {
        poseNameText.text = GameManager.Instance.currentPose.ToString();
        StartCoroutine(Countdown());
    }
    //do the 3..2..1.. thing
    IEnumerator Countdown()
    {
        countdownText.text = "3...";
        yield return new WaitForSeconds(1);
        countdownText.text += "2...";
        yield return new WaitForSeconds(1);
        countdownText.text += "1...";

        //addition 4.9.2019
        //YOU cannot call the debugRenderer.instance until it has been encountered once, else explosion
        //DebugRenderer.Instance.canUpdate = true;
        //clear the skeletons before entering the capture scene
        //DebugRenderer.Instance.skeletons.Clear();
        //9.6 if we have made it past the first iteration of the loop
        if(GameManager.Instance.currentPoseIndex > 1)
        {
            DebugRenderer.Instance.canUpdate = true;
        }
        GameManager.Instance.LoadScene((int)SceneEnums.Scenes.Capture);
    }
}
