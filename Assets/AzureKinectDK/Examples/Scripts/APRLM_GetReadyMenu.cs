using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using APRLM.Game;
public class APRLM_GetReadyMenu : MonoBehaviour
{
    [Tooltip("Manually dragged in, else last child")]
    public Text countdownText;
    [Tooltip("Manually dragged in, else 2nd child")]
    public Text poseNameText;

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
        GameManager.Instance.LoadScene((int)SceneEnums.Scenes.Capture);
    }
}
