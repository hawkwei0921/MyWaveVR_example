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
    public Toggle debugMode_toggle;

    int Scene0_enable = 1;
    int Scene1_enable = 1;
    int Scene2_enable = 1;

    int DebugMode = 0;
    int showUI = 0;

    private void Start()
    {
        initPerfs();
        initUIs();
    }

    private void initUIs() {
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


        if (DebugMode == 1)
            debugMode_toggle.isOn = true;
        else
            debugMode_toggle.isOn = false;

        debugMode_toggle.onValueChanged.AddListener(OnDebugModeValueChanged);

        if (showUI == 1)
            showUI_toggle.isOn = true;
        else
            showUI_toggle.isOn = false;

        showUI_toggle.onValueChanged.AddListener(OnShowUIValueChanged);
    }

    private void loadPrefs() {
        Scene0_text.text = PlayerPrefs.GetString("Scene0Data");
        Scene1_text.text = PlayerPrefs.GetString("Scene1Data");
        Scene2_text.text = PlayerPrefs.GetString("Scene2Data");
        DebugMode = PlayerPrefs.GetInt("DebugMode");
        showUI = PlayerPrefs.GetInt("ShowUI");
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

    private void OnDebugModeValueChanged(bool value) {
        PlayerPrefs.SetInt("DebugMode", Convert.ToInt32(value));
    }

    private void OnShowUIValueChanged(bool value)
    {
        PlayerPrefs.SetInt("ShowUI", Convert.ToInt32(value));
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
