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
        [Header("ARCore")]
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

        [Header("Stats")]
        //Stats variables
        public GameObject statsPrefab;
        private GameObject statsAnchor;
        private GameObject statHappiness, statLifeExpect, statPoints;

        [Header("UI")]
        //UI variables
        private bool hidingUI, animateHandle;
        private float uiHandleUp, uiHandleDown;
        private bool fPaneActive, switchingToFPane, animateFPane; private float uiFPaneUp, uiFPaneDown;
        private bool ePaneActive, switchingToEPane, animateEPane; private float uiEPaneUp, uiEPaneDown;
        private bool hPaneActive, switchingToHPane, animateHPane; private float uiHPaneUp, uiHPaneDown;
        private bool switchingToMPane, animateMPane; private float uiMPaneUp, uiMPaneDown;
        public float animSpeed;
        public GameObject UIHandle, UIFeedPane, UIMainPane, UIExercisePane, UIHygeinePane, UIPoints, UI;
        private Text PointsText, happylifeTextContent;
        private GameObject happylifeText;
        private float happyTextBegin, happyTextEnd, lifeTextBegin, lifeTextEnd;
        private bool happyTextAnimate, lifeTextAnimate;
        private float happylifeTimer;
        private bool nextdayPressed;
        private bool infoboxIsCounting = false;
        public Button nextDayButton;

        [Header("Infoboxes")]
        //Infobox variables
        public GameObject InfoMenu;
        public GameObject FoodInfoMeat;
        public GameObject FoodInfoPlant;
        public GameObject HygeineInfoEco;
        public GameObject HygeineInfoCom;
        public GameObject ExerInfoWalk;
        public GameObject ExerInfoCar;
        public GameObject HygeineInfoComShort;
        public GameObject HygeineInfoEcoShort;
        public GameObject ExerInfoCarShort;
        public GameObject ExerInfoWalkShort;
        public GameObject FoodInfoMeatShort;
        public GameObject FoodInfoPlantShort;
        private bool infoMenuActive;
        private bool infoScreenActive;
        private string currentInfoScreen;
        private bool animateInfobox;
        private bool infoboxIsDown;
        private float infoboxUp = 413f;
        private float infoboxDown = 50f;
        private GameObject CurrentInfobox;
        private float infoboxTimer;
        private int[] infoboxCount = new int[6];
        //[0] foodmeat, [1] foodplant, [2] hygeineCom, [3] hygeineEco, [4] exerCar, [5] exerWalk

        //"points" variables
        private float dayScore;
        private int points, deductNeg, deductPos;

        //Animation variables
        private Animator anim;
        private float animTimer;
        private bool animActive;
        private string currentAnim;
        private ParticleSystem EatParticles;
        private GameObject pet;

        [Header("Audio")]
        //Audio Varibales
        public AudioClip barkClip;
        public AudioClip shakeClip;
        public AudioClip eatClip;
        public AudioClip pantClip;
        public AudioClip scratchClip;
        public AudioSource barkSource, shakeSource, eatSource, pantSource, scratchSource;

        public void Start()
        {
            canSpawn = true;

            //Audio initializations
            barkSource.clip = barkClip;
            shakeSource.clip = shakeClip;
            eatSource.clip = eatClip;
            pantSource.clip = pantClip;
            scratchSource.clip = scratchClip;

            //UI Initialisations
            UI.SetActive(false);
            animateHandle = false;
            hidingUI = false;
            uiHandleUp = -1516f; uiHandleDown = -1787f;
            uiMPaneUp = 725f; uiMPaneDown = 485f;
            //TODO: make these 1 variable
            uiFPaneUp = -89f; uiFPaneDown = -334;
            uiEPaneUp = -89f; uiEPaneDown = -334;
            uiHPaneUp = -89f; uiHPaneDown = -334;
            switchingToFPane = switchingToEPane = switchingToHPane = switchingToMPane = false;
            fPaneActive = ePaneActive = hPaneActive = false;
            PointsText = UIPoints.GetComponent<Text>();
            happylifeText = GameObject.FindGameObjectWithTag("happylifeText");
            happylifeTextContent = happylifeText.GetComponent<Text>();
            happyTextBegin = 22f; happyTextEnd = 45f;
            lifeTextBegin = -8f; lifeTextEnd = -40f;
            happylifeTextContent.color = new Color(0, 0, 0, 0);

            //"Points" initialization
            deductNeg = 2;
            deductPos = 4;
            points = 10;
            dayScore = 0f;

            //Infobox initiliazations
            infoMenuActive = false;
            for (int i = 0; i > 5; i++)
            {
                infoboxCount[i] = 0;
            }
            // CurrentInfobox.SetActive(false);

        }

        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (canSpawn)
            {
                SpawnPet();
            }

            PointsText.text = "Points: " + points + "/10";

            if (!canSpawn)
            {
                if (statHappiness.transform.localScale.x >= 1f)
                {
                    statHappiness.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                if (statLifeExpect.transform.localScale.x >= 1f)
                {
                    statLifeExpect.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }

            if (points == 0){
                nextDayButton.interactable = true;
            }
            else{
                nextDayButton.interactable = false;
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


            //infobox animation
            if (animateInfobox && !infoboxIsDown && !infoboxIsCounting)
            {
                CurrentInfobox.transform.Translate(new Vector3(0, -animSpeed, 0));
                if (CurrentInfobox.transform.localPosition.y <= infoboxDown)
                {
                    infoboxIsDown = true;
                    animateInfobox = false;
                }
            }
            if (animateInfobox && infoboxIsDown && !infoboxIsCounting)
            {
                CurrentInfobox.transform.Translate(new Vector3(0, animSpeed, 0));
                if (CurrentInfobox.transform.localPosition.y >= infoboxUp)
                {
                    animateInfobox = false;
                    infoboxIsDown = false;
                }
            }

            if (infoboxIsDown)
            {   
                infoboxIsCounting = true;
                infoboxTimer -= Time.deltaTime;
                if (infoboxTimer < 0)
                {
                    infoboxIsCounting = false;
                    animateInfobox = true;
                }
            }
            
            //Stat change number animation
            if (happyTextAnimate)
            {
                happylifeText.transform.localPosition = new Vector3(0, happyTextBegin, 0);
                happylifeTimer -= Time.deltaTime;
                if (happylifeTimer < 0)
                {
                    happylifeTextContent.color = new Color(0, 0, 0, 0);
                    happyTextAnimate = false;
                }
            }

            if (lifeTextAnimate)
            {
                happylifeText.transform.localPosition = new Vector3(0, lifeTextBegin, 0);
                happylifeTimer -= Time.deltaTime;
                if (happylifeTimer < 0)
                {
                    happylifeTextContent.color = new Color(0, 0, 0, 0);
                    happyTextAnimate = false;
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
                        eatSource.Play();
                        break;
                    case "shake":
                        anim.SetBool("isShaking", state);
                        shakeSource.Play();
                        break;
                    case "bark":
                        anim.SetBool("isBarking", state);
                        barkSource.Play();
                        break;
                    case "scratch":
                        anim.SetBool("isScratching", state);
                        scratchSource.Play();
                        break;
                    case "sit":
                        anim.SetBool("isSitting", state);
                        pantSource.Play();
                        break;
                    case "walk":
                        anim.SetBool("isWalking", state);
                        break;
                    default:
                        break;
                }
            }
        }

        public void changeStat(string caseSwitch)
        {
            switch (caseSwitch)
            {
                case "feedNeg":
                    if ((points - deductNeg) >= 0)
                    {
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("eat", true);
                        EatParticles.Play();
                        if (infoboxCount[0] < 2)
                        {
                            ToggleInfoboxShort(FoodInfoMeatShort);
                            infoboxCount[0] = infoboxCount[0] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+1";

                    }
                    break;
                case "feedPos":
                    if ((points - deductPos) >= 0)
                    {
                        points -= deductPos;
                        dayScore += 0.05f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("eat", true);
                        EatParticles.Play();
                        if (infoboxCount[1] < 2)
                        {
                            ToggleInfoboxShort(FoodInfoPlantShort);
                            infoboxCount[1] = infoboxCount[1] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+2";
                    }
                    break;
                case "hygNeg":
                    if ((points - deductNeg) >= 0)
                    {
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("shake", true);
                        if (infoboxCount[2] < 2)
                        {
                            ToggleInfoboxShort(HygeineInfoComShort);
                            infoboxCount[2] = infoboxCount[2] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+1";

                    }
                    break;
                case "hygPos":
                    if ((points - deductPos) >= 0)
                    {
                        points -= deductPos;
                        dayScore += 0.05f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("shake", true);
                        if (infoboxCount[3] < 2)
                        {
                            ToggleInfoboxShort(HygeineInfoEcoShort);
                            infoboxCount[3] = infoboxCount[3] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+2";
                    }
                    break;
                case "exerNeg":
                    if ((points - deductNeg) >= 0)
                    {
                        points -= deductNeg;
                        dayScore += 0.01f;
                        statHappiness.transform.localScale += new Vector3(0.1f, 0f, 0f);
                        setAnim("sit", true);
                        if (infoboxCount[4] < 2)
                        {
                            ToggleInfoboxShort(ExerInfoCarShort);
                            infoboxCount[4] = infoboxCount[4] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+1";
                    }
                    break;
                case "exerPos":
                    if ((points - deductPos) >= 0)
                    {
                        points -= deductPos;
                        dayScore += 0.05f;
                        statHappiness.transform.localScale += new Vector3(0.2f, 0f, 0f);
                        setAnim("run", true);
                        if (infoboxCount[5] < 2)
                        {
                            ToggleInfoboxShort(ExerInfoWalkShort);
                            infoboxCount[5] = infoboxCount[5] + 1;
                        }
                        happylifeTimer = 1.5f;
                        happyTextAnimate = true;
                        happylifeTextContent.color = new Color(0, 1, 0, 1);
                        happylifeTextContent.text = "+2";
                    }
                    break;
                default: break;
            }
        }

        public void nextDay()
        {
            nextdayPressed = true;

            float lifeScore = 0.08f;
            statLifeExpect.transform.localScale -= new Vector3(lifeScore, 0f, 0f);
            statLifeExpect.transform.localScale += new Vector3(dayScore, 0f, 0f);
            Debug.Log("Dayscore: " + dayScore + " | " + "Lifescore: " + lifeScore + " | " + "Difference: " + (lifeScore - dayScore));
           
            points = 10;
            happylifeTimer = 2.5f;
            lifeTextAnimate = true;
            if (lifeScore < dayScore)
            {
                happylifeTextContent.color = new Color(0, 1, 0, 1);
                happylifeTextContent.text = "+" + (lifeScore-dayScore) * 10;
            }
            if (lifeScore > dayScore)
            {
                happylifeTextContent.color = new Color(1, 0, 0, 1);
                happylifeTextContent.text = "-" + (lifeScore-dayScore) * 10;
            }
            dayScore = 0f;
        }

        public void ToggleInfoMenu()
        {

            if (!infoMenuActive)
            {
                InfoMenu.SetActive(true);
                infoMenuActive = true;
            }
            else
            {
                InfoMenu.SetActive(false);
                infoMenuActive = false;
            }
        }

        public void ToggleInfoBox(string caseswitch)
        {
           
            switch (caseswitch)
            {
                case "food-meat":
                    currentInfoScreen = "food-meat";
                    if (!infoScreenActive)
                    {
                        FoodInfoMeat.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        FoodInfoMeat.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "food-plant":
                    currentInfoScreen = "food-plant";
                    if (!infoScreenActive)
                    {
                        FoodInfoPlant.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        FoodInfoPlant.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "hygeine-commercial":
                    currentInfoScreen = "hygeine-commercial";
                    if (!infoScreenActive)
                    {
                        HygeineInfoCom.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        HygeineInfoCom.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "hygeine-eco":
                    currentInfoScreen = "hygeine-eco";
                    if (!infoScreenActive)
                    {
                        HygeineInfoEco.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        HygeineInfoEco.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "exercise-walk":
                    currentInfoScreen = "exercise-walk";
                    if (!infoScreenActive)
                    {
                        ExerInfoWalk.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        ExerInfoWalk.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "exercise-car":
                    currentInfoScreen = "exercise-car";
                    if (!infoScreenActive)
                    {
                        ExerInfoCar.SetActive(true);
                        infoScreenActive = true;
                    }
                    else
                    {
                        ExerInfoCar.SetActive(false);
                        infoScreenActive = false;
                    }
                    break;

                case "back":
                    ToggleInfoBox(currentInfoScreen);
                    break;

                default:
                    break;
            }
        }

        public void ToggleInfoboxShort(GameObject gameObject)
        {
            CurrentInfobox = gameObject;
            infoboxTimer = 6f;
            animateInfobox = true;
        }

        public void RemoveInfoboxShort(GameObject gameObject){
            gameObject.transform.localPosition = new Vector3(0, infoboxUp, 0);
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
                    //statsAnchor = (GameObject)Instantiate(statsPrefab, hit.Pose.position, hit.Pose.rotation);


                    anim = GameObject.FindGameObjectWithTag("Pet").GetComponent<Animator>();
                    pet = GameObject.FindGameObjectWithTag("Pet");
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
                    statsPrefab.transform.position = anchor.transform.position;
                    statsPrefab.transform.parent = anchor.transform;
                    statsPrefab.transform.Translate(new Vector3(0.029f, 0.676f, -0.088f));

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
