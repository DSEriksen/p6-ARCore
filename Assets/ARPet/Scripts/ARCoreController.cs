//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif


    /// Controls the HelloAR example.
    public class ARCoreController : MonoBehaviour
    {
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        public Camera FirstPersonCamera;


        /// A prefab for tracking and visualizing detected planes.
        public GameObject DetectedPlanePrefab;

        /// A model to place when a raycast from a user touch hits a plane.
        public GameObject petPlanePrefab;

        /// A model to place when a raycast from a user touch hits a feature point.
        public GameObject petPointPrefab;

        /// The rotation in degrees need to apply to model when the Andy model is placed.
        private const float k_ModelRotation = 180.0f;

        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        private bool m_IsQuitting = false;

        //---Following are custom variables unique to ARCoreP6---
        private bool canSpawn;
        private GameObject petModel;
        private Anchor anchor;

        //Stats variables
        public GameObject statsPrefab;
        private GameObject statsAnchor;
        private GameObject statHappiness, statLifeExpect, statPoints;

        //UI variables
        private bool hidingUI, animateHandle;
        private float uiHandleUp, uiHandleDown;
        private bool fPaneActive, switchingToFPane, animateFPane; private float uiFPaneUp, uiFPaneDown;
        private bool ePaneActive, switchingToEPane, animateEPane; private float uiEPaneUp, uiEPaneDown;
        private bool hPaneActive, switchingToHPane, animateHPane; private float uiHPaneUp, uiHPaneDown;
        private bool switchingToMPane, animateMPane; private float uiMPaneUp, uiMPaneDown;
        private float animSpeed;
        public GameObject UIHandle;
        public GameObject UIFeedPane;
        public GameObject UIMainPane;
        public GameObject UIExercisePane;
        public GameObject UIHygeinePane;
        public GameObject UIPoints;
        public GameObject UI;
        private Text PointsText;

        //"points" variables
        private float deductNeg, deductPos, points, dayScore, pointsToString;

        //Animation variables
        private Animator anim;
        private float animTimer;
        private bool animActive;
        private string currentAnim;
        private ParticleSystem EatParticles;

        public void Start()
        {
            canSpawn = true;

            //UI Initialisations
            UI.SetActive(false);
            animSpeed = 25f;
            animateHandle = false;
            hidingUI = false;
            uiHandleUp = -1451f; uiHandleDown = -1719f;
            uiMPaneUp = 725f; uiMPaneDown = 485f;
            uiFPaneUp = -83f; uiFPaneDown = -334;
            uiEPaneUp = -83f; uiEPaneDown = -334;
            uiHPaneUp = -83f; uiHPaneDown = -334;
            switchingToFPane = switchingToEPane = switchingToHPane = switchingToMPane = false;
            fPaneActive = ePaneActive = hPaneActive = false;
            PointsText = UIPoints.GetComponent<Text>();

            //"Points" initialization
            deductNeg = 0.2f;
            deductPos = 0.4f;
            points = 1.0f;
            dayScore = 0f;

        }

        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (canSpawn)
            {
                SpawnPet();
            }


            pointsToString = points * 10;
            PointsText.text = "Points: " + pointsToString + "/10";

            if(!canSpawn){
                if (statHappiness.transform.localScale.x >= 1f)
                {
                    statHappiness.transform.localScale = new Vector3(1f, 0f, 0f);
                }
                if (statLifeExpect.transform.localScale.x >= 1f)
                {
                    statLifeExpect.transform.localScale = new Vector3(1f, 0f, 0f);
                }
            }

            //UI animation section
            //Handle

            if (animateHandle && !hidingUI)
            {
                UIHandle.transform.Translate(new Vector3(0, -animSpeed, 0), Space.World);
                if (UIHandle.transform.localPosition.y <= uiHandleDown)
                {
                    animateHandle = false;
                    hidingUI = true;
                }
            }
            if (animateHandle && hidingUI)
            {
                UIHandle.transform.Translate(new Vector3(0, animSpeed, 0));
                if (UIHandle.transform.localPosition.y >= uiHandleUp)
                {
                    animateHandle = false;
                    hidingUI = false;
                }
            }

            //----Feedpane switch----
            if (switchingToFPane)
            {
                //Move mainpane down
                if (!animateMPane && !fPaneActive && !animateFPane) animateMPane = true;
                if (animateMPane && !fPaneActive)
                {
                    UIMainPane.transform.Translate(new Vector3(0, -animSpeed, 0));
                    if (UIMainPane.transform.localPosition.y <= uiMPaneDown)
                    {
                        animateFPane = true;
                        animateMPane = false;
                    }
                }
                //Move feedpane up
                if (animateFPane && !fPaneActive)
                {
                    UIFeedPane.transform.Translate(new Vector3(0, animSpeed, 0));
                    if (UIFeedPane.transform.localPosition.y >= uiFPaneUp)
                    {
                        animateFPane = false;
                        fPaneActive = true;
                        switchingToFPane = false;
                    }
                }
            }

            //----Exercise pane switch----
            if (switchingToEPane)
            {
                //Move mainpane down
                if (!animateMPane && !ePaneActive && !animateEPane) animateMPane = true;
                if (animateMPane && !ePaneActive)
                {
                    UIMainPane.transform.Translate(new Vector3(0, -animSpeed, 0));
                    if (UIMainPane.transform.localPosition.y <= uiMPaneDown)
                    {
                        animateEPane = true;
                        animateMPane = false;
                    }
                }
                //Move exercisepane up
                if (animateEPane && !ePaneActive)
                {
                    UIExercisePane.transform.Translate(new Vector3(0, animSpeed, 0));
                    if (UIExercisePane.transform.localPosition.y >= uiEPaneUp)
                    {
                        animateEPane = false;
                        ePaneActive = true;
                        switchingToEPane = false;
                    }
                }
            }

            //---Hygeine pane switch----
            if (switchingToHPane)
            {
                //Move mainpane down
                if (!animateMPane && !hPaneActive && !animateHPane) animateMPane = true;
                if (animateMPane && !hPaneActive)
                {
                    UIMainPane.transform.Translate(new Vector3(0, -animSpeed, 0));
                    if (UIMainPane.transform.localPosition.y <= uiMPaneDown)
                    {
                        animateHPane = true;
                        animateMPane = false;
                    }
                }
                //Move hygeinepane up
                if (animateHPane && !ePaneActive)
                {
                    UIHygeinePane.transform.Translate(new Vector3(0, animSpeed, 0));
                    if (UIHygeinePane.transform.localPosition.y >= uiHPaneUp)
                    {
                        animateHPane = false;
                        hPaneActive = true;
                        switchingToHPane = false;
                    }
                }
            }


            //---Mainpane switch----
            if (switchingToMPane)
            {
                //if current pane active is feedpane
                if (fPaneActive)
                {
                    animateFPane = true;
                    if (animateFPane)
                    {
                        UIFeedPane.transform.Translate(new Vector3(0, -animSpeed, 0));
                        if (UIFeedPane.transform.localPosition.y <= uiFPaneDown)
                        {
                            animateFPane = false;
                            fPaneActive = false;
                            animateMPane = true;
                        }
                    }
                }
                //if exercisepane active
                if (ePaneActive)
                {
                    animateEPane = true;
                    if (animateEPane)
                    {
                        UIExercisePane.transform.Translate(new Vector3(0, -animSpeed, 0));
                        if (UIExercisePane.transform.localPosition.y <= uiEPaneDown)
                        {
                            animateEPane = false;
                            ePaneActive = false;
                            animateMPane = true;
                        }
                    }
                }
                //if hygeine active
                if (hPaneActive)
                {
                    animateHPane = true;
                    if (animateHPane)
                    {
                        UIHygeinePane.transform.Translate(new Vector3(0, -animSpeed, 0));
                        if (UIHygeinePane.transform.localPosition.y <= uiHPaneDown)
                        {
                            animateHPane = false;
                            hPaneActive = false;
                            animateMPane = true;
                        }
                    }
                }
                //move mainpane up
                if (animateMPane)
                {
                    UIMainPane.transform.Translate(new Vector3(0, animSpeed, 0));
                    if (UIMainPane.transform.localPosition.y >= uiMPaneUp)
                    {
                        animateMPane = false;
                        switchingToMPane = false;
                    }
                }
            }



            //Pet animation section
            if (animActive)
            {
                animTimer -= Time.deltaTime;
                if (animTimer < 0)
                {
                    setAnim(currentAnim, false);
                    animActive = false;
                    if (EatParticles.IsAlive())
                    {
                        EatParticles.Stop();
                    }
                }
            }
        }

        public void SwitchPane(int caseSwitch)
        {
            switch (caseSwitch)
            {
                case 0:
                    switchingToFPane = true;
                    break;
                case 1:
                    switchingToHPane = true;
                    break;
                case 2:
                    switchingToEPane = true;
                    break;
                default: break;
            }
        }

        public void SwitchToMainPane()
        {
            switchingToMPane = true;
        }

        public void ToggleUI()
        {
            animateHandle = true;
        }

        public void ToggleStats()
        {
            if (statsAnchor.activeSelf)
            {
                statsAnchor.SetActive(false);
            }
            else
            {
                statsAnchor.SetActive(true);
            }
        }

        private void setAnim(string caseSwitch, bool state)
        {
            animActive = true;
            animTimer = 2.2f;
            currentAnim = caseSwitch;
            if (animActive)
            {
                switch (caseSwitch)
                {
                    case "run":
                        anim.SetBool("isRunning", state);
                        break;
                    case "eat":
                        anim.SetBool("isEating", state);
                        break;
                    case "shake":
                        anim.SetBool("isShaking", state);
                        break;
                    case "bark":
                        anim.SetBool("isBarking", state);
                        break;
                    case "scratch":
                        anim.SetBool("isScratching", state);
                        break;
                    case "sit":
                        anim.SetBool("isSitting", state);
                        break;
                    case "walk":
                        anim.SetBool("isWalking", state);
                        break;
                    default:
                        break;
                }
            }
        }

        //TODO: change so buttons do all the input, and not code
        //      requires custom eventscript
        public void changeStat(string caseSwitch)
        {
            if (points - deductNeg > 0 || points - deductPos > 0)
            {
                switch (caseSwitch)
                {
                    case "feedNeg":
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("eat", true);
                        EatParticles.Play();
                        break;
                    case "feedPos":
                        points -= deductPos;
                        dayScore += 0.03f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("eat", true);
                        EatParticles.Play();
                        break;
                    case "hygNeg":
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("shake", true);
                        break;
                    case "hygPos":
                        points -= deductPos;
                        dayScore += 0.03f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("shake", true);
                        break;
                    case "exerNeg":
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("sit", true);
                        break;
                    case "exerPos":
                        points -= deductPos;
                        dayScore += 0.03f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("run", true);
                        break;
                    default: break;
                }

                Debug.Log(points);
            }
            else { return; }
        }

        public void nextDay()
        {
            float lifeScore = 0.1f - dayScore;
            dayScore = 0f;
            points = 1f;
            statLifeExpect.transform.localScale -= new Vector3(lifeScore, 0f, 0f);
            statHappiness.transform.localScale -= new Vector3(0.2f, 0f, 0f);
        }

        private void SpawnPet()
        {

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.

                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {

                    // Choose the Andy model for the Trackable that got hit.
                    GameObject prefab;
                    if (hit.Trackable is FeaturePoint)
                    {
                        prefab = petPointPrefab;
                    }
                    else
                    {
                        prefab = petPlanePrefab;
                    }

                    // Instantiate Andy model at the hit pose.
                    petModel = (GameObject)Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                    statsAnchor = (GameObject)Instantiate(statsPrefab, hit.Pose.position, hit.Pose.rotation);

                    anim = GameObject.FindGameObjectWithTag("Pet").GetComponent<Animator>();
                    statHappiness = GameObject.FindGameObjectWithTag("statHappiness");
                    statLifeExpect = GameObject.FindGameObjectWithTag("statLifeExpect");
                    statPoints = GameObject.FindGameObjectWithTag("statPoints");
                    EatParticles = GameObject.FindGameObjectWithTag("Mouth").GetComponent<ParticleSystem>();

                    // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                    petModel.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                    // world evolves.
                    anchor = (Anchor)hit.Trackable.CreateAnchor(hit.Pose);

                    // Make Andy model a child of the anchor.
                    petModel.transform.parent = anchor.transform;
                    statsAnchor.transform.parent = anchor.transform;
                    statsAnchor.transform.Translate(0.30f, 0.60f, 0.35f);

                    UI.SetActive(true);
                    statHappiness.transform.localScale -= new Vector3(0.5f, 0f, 0f);
                    statLifeExpect.transform.localScale -= new Vector3(0.5f, 0f, 0f);

                    canSpawn = false;
                }
            }
        }

        public void reset()
        {
            SceneManager.LoadScene("mainscene");
        }

        //---ARCore specific functions---

        /// Check and update the application lifecycle.

        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }


        /// Actually quit the application.

        private void _DoQuit()
        {
            Application.Quit();
        }


        /// Show an Android toast message.

        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
