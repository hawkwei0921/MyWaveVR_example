using HTC.UnityPlugin.Vive;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Native;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject loadingIconGO;
    public GameObject exitProgressbarGO;

    public GameObject debugModeCanvas;
    public GameObject RightControllerUiReticleGO;
    public GameObject RightControllerUiLineGO;
    public GameObject LeftControllerUiReticleGO;
    public GameObject LeftControllerUiLineGO;

    public GameObject UIpanelsGO;

    int DebugMode = 0;
    int showUI = 1;
    int Scene0_enable = 1;
    int Scene1_enable = 1;
    int Scene2_enable = 1;

    // Start is called before the first frame update
    void Start()
    {
        loadPrefs();
        initUIs();

       // showLoadingIcon(true);
        switchScene("MyScene0");

    }

    private void loadPrefs()
    {
        DebugMode = PlayerPrefs.GetInt("DebugMode");
        showUI = PlayerPrefs.GetInt("ShowUI");
        Scene0_enable = PlayerPrefs.GetInt("Scene0_enable");
        Scene1_enable = PlayerPrefs.GetInt("Scene1_enable");
        Scene2_enable = PlayerPrefs.GetInt("Scene2_enable");
    }

    private void initUIs() {
        if (DebugMode == 1)
            debugModeCanvas.SetActive(true);
        else
            debugModeCanvas.SetActive(false);

        if (showUI == 1)
        {
            RightControllerUiReticleGO.GetComponent<ReticlePoser>().enabled = true;
            RightControllerUiLineGO.GetComponent<GuideLineDrawer>().enabled = true;
            RightControllerUiLineGO.GetComponent<LineRenderer>().enabled = true;
            LeftControllerUiReticleGO.GetComponent<ReticlePoser>().enabled = true;
            LeftControllerUiLineGO.GetComponent<GuideLineDrawer>().enabled = true;
            LeftControllerUiLineGO.GetComponent<LineRenderer>().enabled = true;
            UIpanelsGO.SetActive(true);
        }
        else
        {
            RightControllerUiReticleGO.GetComponent<ReticlePoser>().enabled = false;
            RightControllerUiLineGO.GetComponent<GuideLineDrawer>().enabled = false;
            RightControllerUiLineGO.GetComponent<LineRenderer>().enabled = false;
            LeftControllerUiReticleGO.GetComponent<ReticlePoser>().enabled = false;
            LeftControllerUiLineGO.GetComponent<GuideLineDrawer>().enabled = false;
            LeftControllerUiLineGO.GetComponent<LineRenderer>().enabled = false;
            UIpanelsGO.SetActive(false);
        }
    }

    static bool switchingScene = false;
    public void switchScene(string sceneName) {
        Debug.LogWarning("sceneName= " + sceneName);

        cycleSceneName(ref sceneName);

        Debug.LogWarning("cycleSceneName()   sceneName= " + sceneName);
        switchingScene = true;
        if (IsScene_CurrentlyLoaded(sceneName))
        {
            Debug.LogWarning("scene " + sceneName + " was loaded.");
        }
        else
        {
            // showLoadingIcon(true);
            if (IsScene_CurrentlyLoaded(sceneName) == false)
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        // keep one camera scene
        if (IsScene_CurrentlyLoaded("MyScene0") && !sceneName.Equals("MyScene0"))
            SceneManager.UnloadScene("MyScene0");
        if (IsScene_CurrentlyLoaded("MyScene1") && !sceneName.Equals("MyScene1"))
            SceneManager.UnloadScene("MyScene1");
        if (IsScene_CurrentlyLoaded("MyScene2") && !sceneName.Equals("MyScene2"))
            SceneManager.UnloadScene("MyScene2");

        switchingScene = false;
    }

    private void cycleSceneName(ref string sceneName) {
        if (Scene0_enable != 1 || Scene1_enable != 1 || Scene2_enable != 1)
        {
            if (sceneName.Equals("MyScene0") && (Scene0_enable == 0))
                sceneName = "MyScene1";
            if (sceneName.Equals("MyScene1") && (Scene1_enable == 0))
                sceneName = "MyScene2";
            if (sceneName.Equals("MyScene2") && (Scene2_enable == 0))
                sceneName = "MyScene0";

        }
    }

    public void closeVrApp() {
        Log.w(CONSTANTS.TAG, "closeVrApp");
        Application.Quit();
    }

    bool IsScene_CurrentlyLoaded(string sceneName_no_extention)
    {
#if UNITY_EDITOR
        for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);

            if (scene.name == sceneName_no_extention)
            {
                return true;//the scene is already loaded
            }
        }
        //scene not currently loaded in the hierarchy:
        return false;
#else
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName_no_extention)
            {
                //the scene is already loaded
                return true;
            }
        }

        return false;//scene not currently loaded in the hierarchy
#endif
    }

    private float pressedTime = 0.0f;
    private float showbarTime = 5.0f;
    private float exitTime = 10.0f;
    private int PROGRESSBAR_FULL_WIDTH = 400;

    private void Update()
    {

        if (showUI == 0)
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.N))
#else
            if (ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Trigger) ||
            ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Grip) ||
            ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Pad) ||
            ViveInput.GetPress(HandRole.RightHand, ControllerButton.Trigger) ||
            ViveInput.GetPress(HandRole.RightHand, ControllerButton.Grip) ||
            ViveInput.GetPress(HandRole.RightHand, ControllerButton.Pad) )
#endif
            {
                pressedTime += Time.deltaTime;
                Log.i(CONSTANTS.TAG, "pressedTime = " + pressedTime);
                // Subtracting two is more accurate over time than resetting to zero.
                if (pressedTime > showbarTime)
                {
                    if (exitProgressbarGO.activeSelf == false)
                        exitProgressbarGO.SetActive(true);
                    RectTransform  rt = (RectTransform)exitProgressbarGO.transform.GetChild(1).gameObject.transform;
                    rt.sizeDelta = new Vector2((pressedTime - showbarTime)/(exitTime- showbarTime)* PROGRESSBAR_FULL_WIDTH, rt.sizeDelta.y);

                    if (pressedTime > exitTime)
                    {
                        Log.i(CONSTANTS.TAG, "pressedTime > exitTime=" + exitTime);
                        showbarTime = pressedTime;
                        pressedTime = pressedTime - exitTime;
                        closeVrApp();
                    }
                }
                else {
                    if (exitProgressbarGO.activeSelf == true)
                        exitProgressbarGO.SetActive(false);
                }
            }
            else {
                // Log.i(CONSTANTS.TAG, "pressedTime = 0.0f");
                pressedTime = 0.0f;
                if (exitProgressbarGO.activeSelf == true)
                    exitProgressbarGO.SetActive(false);
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.N))
#else
            if (ViveInput.GetPressUp(HandRole.LeftHand, ControllerButton.Trigger) ||
            ViveInput.GetPressUp(HandRole.LeftHand, ControllerButton.Grip) ||
            ViveInput.GetPressUp(HandRole.LeftHand, ControllerButton.Pad) ||
            ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger) ||
            ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Grip) ||
            ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Pad) )
#endif
            {
                 Log.i(CONSTANTS.TAG, "Switch scene button GetPressUp   switchingScene="+ switchingScene);
                if (switchingScene == false)
                {
                    if (IsScene_CurrentlyLoaded("MyScene0"))
                        switchScene("MyScene1");
                    else if (IsScene_CurrentlyLoaded("MyScene1"))
                        switchScene("MyScene2");
                    else if (IsScene_CurrentlyLoaded("MyScene2"))
                        switchScene("MyScene0");
                    else
                        Log.i(CONSTANTS.TAG, "unknown scene");
                }
            }
        }
    }


    protected void showLoadingIcon(bool bShow)
    {
        StartCoroutine(runScanningIcon(bShow));
        // LoadingIcon(bShow);
    }


    private IEnumerator runScanningIcon(bool bShow)
    {
        Log.i(CONSTANTS.TAG, "runScanningIcon  bShow=" + bShow);
        if (bShow)
        {
            loadingIconGO.SetActive(true);
            yield return new WaitForSeconds(CONSTANTS.LOADING_ANIMATION_TIMEOUT);
            Log.i(CONSTANTS.TAG, "runScanningIcon  " + CONSTANTS.LOADING_ANIMATION_TIMEOUT + " sec --");
            loadingIconGO.SetActive(false);
        }
        else
        {
            loadingIconGO.SetActive(false);
        }
    }
}
