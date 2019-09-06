using UnityEngine;
using System.Collections.Generic;
using APRLM.Game;
using UnityEngine.UI;

namespace APRLM.UI
{
	public class APRLM_MainMenu : MonoBehaviour
	{
        //dragged in manually, else is last child
        [Tooltip("Dragged in manually!")]
        public Text poseText;

        private void Start()
        {
            //Request pose data from GM
            ParsePoseList(GameManager.Instance.poseList);
        }

        private void ParsePoseList(List<Pose> poseList)
        {
            foreach(Pose p in poseList)
            {
                poseText.text += p.poseName.ToString() + "\n";
            }
        }

        public void StartButton_LinkedToButton()
		{
            GameManager.Instance.LoadScene((int)SceneEnums.Scenes.ReadyNext);
		}
	}
}