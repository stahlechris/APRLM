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
        }
        private void Start()
        {
            Debug.Log("gm start called");
            Timing.RunCoroutine(Main());

            //SceneManager.sceneLoaded += OnSceneLoaded;
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
        //private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
        //{
        //    print(scene.name);

        //    if(scene.name == "CaptureScene")
        //    {
        //        DebugRenderer.Instance.canUpdate = true;
        //        ///addition 9.4/2019
        //        print(scene.name + "printed because if(scene.name == CaptureScene)");
        //        //if we are in the capture scene, the countdown is over
        //        currentState = GameState.ReadyNextCountDownOver;
        //    }
        //}

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

        public void LoadSceneAdditive(int scene)
        {
            if (CheckForPoses())
            {
                //Load another scene on different thread
                SceneManager.LoadSceneAsync(scene,LoadSceneMode.Additive);
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
            print("Blockman was created in GM");
        }

        IEnumerator<float> Main()
        {
            for(int i=0;i<poseList.Count;i++)
            {
                //Wait until the capture is completed, by capturing X skeletons
                yield return Timing.WaitUntilTrue(() => currentState == GameState.CaptureCompleted);
                print("capture completed, state change in GM");
				
				// 9.4.2019 saw 5 in the Skeletons length in inspector, can write out before clearing
                DebugRenderer.Instance.skeletons.Clear();
                //9.4 we confirmed that list is clear before going into capture scene for the second time.

                //Load the menu
                //todo update pose list
                currentState = GameState.PlayScenePressed;

                //this is here for testing if the pose list gets decremented, we want to load back to ReadyNextMenu irl
       

                LoadScene((int)SceneEnums.Scenes.MainMenu);
            }
        }
        //private void OnDisable()//a persistant singleton class will only have this called once, when program ending.
        //{
        //    print("OnDisabled GM");
        //    //Must unsubscribe from event, else explosion.
        //    SceneManager.sceneLoaded -= OnSceneLoaded;
        //}
    }
}