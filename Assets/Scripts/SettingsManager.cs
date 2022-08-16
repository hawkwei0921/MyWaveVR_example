using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle Scene0_toggle;
    public Toggle Scene1_toggle;
    public Toggle Scene2_toggle;

    public TMP_InputField Scene0_text;
    public TMP_InputField Scene1_text;
    public TMP_InputField Scene2_text;

    public Toggle showUI_toggle;
    public Toggle debugFPS_toggle;
    public Toggle debugMsg_toggle;
    public Toggle debugLogcat_toggle;
    public Toggle debugGraphy_toggle;
    public Toggle debugUtils_toggle;

    int Scene0_enable = 1;
    int Scene1_enable = 1;
    int Scene2_enable = 1;

    int DebugFPS = 0;
    int DebugMsg = 0;
    int DebugLogcat = 0;
    int DebugGraphy = 0;
    int DebugUtils = 0;

    int showUI = 0;

    private void Start()
    {
        initPerfs();
        initUIs();
    }

    private void initUIs() {
        if (showUI == 1)
            showUI_toggle.isOn = true;
        else
            showUI_toggle.isOn = false;
        showUI_toggle.onValueChanged.AddListener(OnShowUIValueChanged);

        if (Scene0_enable == 1)
            Scene0_toggle.isOn = true;
        else
            Scene0_toggle.isOn = false;
        Scene0_toggle.onValueChanged.AddListener(OnScene0ToggleValueChanged);

        if (Scene1_enable == 1)
            Scene1_toggle.isOn = true;
        else
            Scene1_toggle.isOn = false;
        Scene1_toggle.onValueChanged.AddListener(OnScene1ToggleValueChanged);

        if (Scene2_enable == 1)
            Scene2_toggle.isOn = true;
        else
            Scene2_toggle.isOn = false;
        Scene2_toggle.onValueChanged.AddListener(OnScene2ToggleValueChanged);


        if (DebugFPS == 1)
            debugFPS_toggle.isOn = true;
        else
            debugFPS_toggle.isOn = false;
        debugFPS_toggle.onValueChanged.AddListener(OnDebugFPSValueChanged);

        if (DebugMsg == 1)
            debugMsg_toggle.isOn = true;
        else
            debugMsg_toggle.isOn = false;
        debugMsg_toggle.onValueChanged.AddListener(OnDebugMsgValueChanged);

        if (DebugLogcat == 1)
            debugLogcat_toggle.isOn = true;
        else
            debugLogcat_toggle.isOn = false;
        debugFPS_toggle.onValueChanged.AddListener(OnDebugLogcatValueChanged);

        if (DebugGraphy == 1)
            debugGraphy_toggle.isOn = true;
        else
            debugGraphy_toggle.isOn = false;
        debugGraphy_toggle.onValueChanged.AddListener(OnDebugGraphyValueChanged);

        if (DebugUtils == 1)
            debugUtils_toggle.isOn = true;
        else
            debugUtils_toggle.isOn = false;
        debugUtils_toggle.onValueChanged.AddListener(OnDebugUtilsValueChanged);

    }

    private void loadPrefs() {
        showUI = PlayerPrefs.GetInt("ShowUI");

        Scene0_text.text = PlayerPrefs.GetString("Scene0Data");
        Scene1_text.text = PlayerPrefs.GetString("Scene1Data");
        Scene2_text.text = PlayerPrefs.GetString("Scene2Data");

        DebugFPS = PlayerPrefs.GetInt("DebugFPS");
        DebugMsg = PlayerPrefs.GetInt("DebugMsg");
        DebugLogcat = PlayerPrefs.GetInt("DebugLogcat");
        DebugGraphy = PlayerPrefs.GetInt("DebugGraphy");
        DebugUtils = PlayerPrefs.GetInt("DebugUtils");
    }

    private void OnShowUIValueChanged(bool value)
    {
        PlayerPrefs.SetInt("ShowUI", Convert.ToInt32(value));
    }

    private void OnScene0ToggleValueChanged(bool value)
    {
        PlayerPrefs.SetInt("Scene0_enable", Convert.ToInt32(value));
    }

    private void OnScene1ToggleValueChanged(bool value)
    {
        PlayerPrefs.SetInt("Scene1_enable", Convert.ToInt32(value));
    }

    private void OnScene2ToggleValueChanged(bool value)
    {
        PlayerPrefs.SetInt("Scene2_enable", Convert.ToInt32(value));
    }

    private void OnDebugFPSValueChanged(bool value) {
        PlayerPrefs.SetInt("DebugFPS", Convert.ToInt32(value));
    }
    private void OnDebugMsgValueChanged(bool value)
    {
        PlayerPrefs.SetInt("DebugMsg", Convert.ToInt32(value));
    }
    private void OnDebugLogcatValueChanged(bool value)
    {
        PlayerPrefs.SetInt("DebugLogcat", Convert.ToInt32(value));
    }
    private void OnDebugGraphyValueChanged(bool value)
    {
        PlayerPrefs.SetInt("DebugGraphy", Convert.ToInt32(value));
    }
    private void OnDebugUtilsValueChanged(bool value)
    {
        PlayerPrefs.SetInt("DebugUtils", Convert.ToInt32(value));
    }


    public void onClickSave() {
        savePrefs();
    }

    public void onClickReset()
    {
        resetPrefs();
    }

    public void onClickStart()
    {
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
    }
    public void onClickExit()
    {
        Application.Quit();
    }

    private void initPerfs() { 
        if (PlayerPrefs.GetString("Scene0Data").Equals("")) {
            PlayerPrefs.SetString("Scene0Data", CONSTANTS.DEFAULT_Scene0Data);
        }
        if (PlayerPrefs.GetString("Scene1Data").Equals(""))
        {
            PlayerPrefs.SetString("Scene1Data", CONSTANTS.DEFAULT_Scene1Data);
        }
        if (PlayerPrefs.GetString("Scene2Data").Equals(""))
        {
            PlayerPrefs.SetString("Scene2Data", CONSTANTS.DEFAULT_Scene2Data);
        }
        PlayerPrefs.SetInt("Scene0_enable", 1);
        PlayerPrefs.SetInt("Scene1_enable", 1);
        PlayerPrefs.SetInt("Scene2_enable", 1);
        loadPrefs();
    }
    private void savePrefs() {
        if (!Scene0_text.text.Equals(""))
            PlayerPrefs.SetString("Scene0Data", Scene0_text.text);
        else
            PlayerPrefs.SetString("Scene0Data", CONSTANTS.DEFAULT_Scene0Data);
        if (!Scene1_text.text.Equals(""))
            PlayerPrefs.SetString("Scene1Data", Scene1_text.text);
        else
            PlayerPrefs.SetString("Scene1Data", CONSTANTS.DEFAULT_Scene1Data);

        if (!Scene2_text.text.Equals(""))
            PlayerPrefs.SetString("Scene2Data", Scene2_text.text);
        else
            PlayerPrefs.SetString("Scene2Data", CONSTANTS.DEFAULT_Scene2Data);

    }

    private void resetPrefs()
    {
        PlayerPrefs.SetString("Scene0Data", CONSTANTS.DEFAULT_Scene0Data);
        PlayerPrefs.SetString("Scene1Data", CONSTANTS.DEFAULT_Scene1Data);
        PlayerPrefs.SetString("Scene2Data", CONSTANTS.DEFAULT_Scene2Data);
    }

}
