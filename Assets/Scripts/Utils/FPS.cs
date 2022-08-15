using TMPro;
using UMP;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    private TMP_Text textField;
    private float fps = 75;
    private float accTime = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        accTime = 0;
    }

    private void Start()
    {
        textField = gameObject.GetComponent<TMP_Text>();
    }


    void LateUpdate()
    {
        if (textField != null)
        {
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            accTime += unscaledDeltaTime;

            // Avoid crash when timeScale is 0.
            if (unscaledDeltaTime == 0)
            {
                textField.text = "0fps";
                return;
            }

            string text = "";

            float interp = unscaledDeltaTime / (0.5f + unscaledDeltaTime);
            float currentFPS = 1.0f / unscaledDeltaTime;
            fps = Mathf.Lerp(fps, currentFPS, interp);
            var vrFps = fps;
            // Avoid update Canvas too frequently.
            if (accTime < 0.20f)
                return;
            accTime = 0;
            if(vrFps != 0)
                text += UniversalMediaPlayer.FrameRateBAK + "/" + Mathf.RoundToInt(vrFps) + "fps";
            textField.text = text;
        }
    }

}

