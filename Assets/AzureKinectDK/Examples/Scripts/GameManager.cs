using UnityEngine;
using UnityEngine.SceneManagement;
using Stahle.Utility;
using System.Collections.Generic;
using UnityEditor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System;
using MEC;

namespace APRLM.Game
{
    public enum GameState
    {
        PlayScenePressed =0, //Read the Poses we want to use for this session and give them to canvas
        PoseListPopulated, //The canvas has written the poses to UI fed to it by this GameManager
        WaitingForUserToPressStart, //nothing to do, waiting for user
        StartButtonPressed, //The user has pressed the start button to begin capturing pose data
        ReadyNext,
        ReadyNextCountDownOver,
        CaptureCompleted
            //TODO finish game states
            //TODO delegates for game states and message broadcasting, static events...etc
    }
    public class GameManager : PersistantSingleton<GameManager>
    {

        [Header("Drag Poses you've created onto the 'Pose List'.",order =0)]
        [Space(-10,order = 1)]
        [Header("Multiple Poses can be dropped at once.",order = 2)]

        [Tooltip("Right click in any folder in Project window to make a new Pose.")]

        public List<Pose> poseList;
        public GameState currentState;
        public Pose currentPose;
        public GameObject[] blockman; //todo refactor into a blockmanMaker.cs
        public GameObject blockPrefab;

        protected override void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            base.Awake();
            currentState = GameState.PlayScenePressed;
            CheckSettings();
            MakeBlockMan();
			print(SceneManager.GetActiveScene().name);
        }
        private void Start()
        {
            Debug.Log("gm start called");
            Timing.RunCoroutine(Main());
        }
        public List<Pose> GetPoseList()
        {
            return poseList;
        }
        private void CheckSettings()
        {
            if(poseList.Count < 1)
            {
                Debug.Log("!No poses were dragged into the Pose List!");
                EditorApplication.isPlaying = false;
            }
            else
            {
                //currentPose gets set to first Pose in the list
                currentPose = poseList[0];
            }

        }
        private bool CheckForPoses()
        {
            if (poseList.Count < 1)
            {
                Debug.Log("!No poses were dragged into the Pose List!");
                return false;
            }
            else
            {
                return true;
            }
        }
        public void LoadScene(int scene)
        {
            if(CheckForPoses())
            {
                //Load another scene on different thread
                SceneManager.LoadSceneAsync(scene);
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }
        //todo put block man under this GameManager so they dont dissapear
        private void MakeBlockMan()
        {
            int size = (int)JointId.Count;

            blockman = new GameObject[size];

            for (var i = 0; i < size; i++)
            {
                //make a cube for every joint
                //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject cube = Instantiate(blockPrefab,transform);
                //deactivate it - (its Start() or OnEnable() won't be called)
                cube.SetActive(false);
                //give cube a name of matching joint
                cube.name = Enum.GetName(typeof(JointId), i);
                //why do we multiply by .4?  idk
                cube.transform.localScale = Vector3.one * 0.4f;
                //add our cube to the skeleton[]
                blockman[i] = cube;
            }
        }

        //todo maybe this gm should just have a main coroutine thing that controls whole program?
        //can replace the firing of static events with global bool flags stored on this gm?
        //maybe consolidate scenes for simplicity for now
        IEnumerator<float> Main()
        {
            for(int i=0;i<poseList.Count;i++)
            {
				yield return Timing.WaitUntilTrue(() => SceneManager.GetActiveScene().name == "CaptureScene");
				DebugRenderer.Instance.canUpdate = true;

				for (int i2 = 0; i2 < 3; i2++)
				{
					DebugRenderer.Instance.TakeSinglePicture();
				}
				

				yield return Timing.WaitUntilTrue(() => currentState == GameState.CaptureCompleted);
				print("capture completed, state change");
                currentState = GameState.PlayScenePressed;
                LoadScene(0); //this loaded 3 times

            }
            yield return Timing.WaitForOneFrame;
        }

    }
}