using UnityEngine;
using APRLM.Game;
using UnityEngine.UI;

namespace APRLM.UI
{
    public class APRLM_ReadyNextMenu : MonoBehaviour
    {
        [Tooltip("Dragged in manually, else 5th child")]
		public Text poseNameText;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        void OnEnable()
		{
            poseNameText.text = GameManager.Instance.currentPose.ToString();
		}
        public void NextButton_LinkedToButton()
        {
            GameManager.Instance.LoadScene((int)SceneEnums.Scenes.GetReady);
        }
    }
}