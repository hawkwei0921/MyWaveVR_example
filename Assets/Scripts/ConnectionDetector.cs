using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDetector : MonoBehaviour
{
    public Image coverImage;
    public Sprite[] coverTextures;

    private void Awake()
    {
        coverImage = GetComponent<Image>();
        ConnectivityManager.Instance.AddConnectivityListener(OnConnectivityChange);
    }
    private void OnDestroy()
    {
        ConnectivityManager.Instance.RemoveConnectivityListener(OnConnectivityChange);
    }
    private void OnConnectivityChange(bool isConnected, string errorMsg)
    {
            if(isConnected)
                coverImage.sprite = coverTextures[1];
            else
                coverImage.sprite = coverTextures[0];

        if (!string.IsNullOrEmpty(errorMsg))
        {
            Debug.LogWarning(errorMsg);
        }
    }
}
