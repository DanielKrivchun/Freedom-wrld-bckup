using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ErrorUI : MonoBehaviour
{
    #region VARIABLES
    [Header("Internet Connection")]
    public GameObject ErrorUIObject;
    public RectTransform ErrorPopup;
    public TextMeshProUGUI ErrorText;
    [Space]
    public GameObject ReconnectingObj;
    [Space]
    public Button HomeButton;
    public Button LeaveButton;
    private bool isPaused;

    private bool button_waiter;

    private bool CheckForConnection;
    private float deltaTime = 0.0f;
    private float fps;

    private bool LeavingToHome;

    #endregion

    #region UNITY METHODS
    private void OnEnable()
    {
        NetworkEventManager.e_network_errors += _NetworkError;
        HomeButton.onClick.AddListener(_GoingHome);
        LeaveButton.onClick.AddListener(_GoingHome);
    }

    private void OnDisable()
    {
        NetworkEventManager.e_network_errors -= _NetworkError;
        HomeButton.onClick.RemoveListener(_GoingHome);
        LeaveButton.onClick.RemoveListener(_GoingHome);
    }

    private void Update()
    {
        if (CheckForConnection)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;

            if (fps > 10f)
            {
                CheckForConnection = false;
                ReconnectingObj.SetActive(false);
            }
        }
    }
    #endregion

    #region BUTTON CALLBACK AND EVENT CALLBACKS
    private async void _GoingHome()
    {
        if (button_waiter) return;
        LeavingToHome = true;
        button_waiter = true;
        Utils._DoButtonAnimation(HomeButton.transform);
        Utils._DoButtonAnimation(LeaveButton.transform);
        await Utils._Waiter(200);
        if (RaceManager.instance != null)
        {
            await RaceManager.instance._LeaveGame();
        }
        button_waiter = false;
        SceneManager.LoadScene(_Strings.PetCareScene);
    }

    private void _NetworkError(string _s)
    {
        if (LeavingToHome) return;
        if (NetwrokUI.Instance.LeavedGame) return;
        ErrorPopup.transform.localScale = Vector3.zero;
        ErrorUIObject.SetActive(true);
        ErrorText.text = _s;
        ErrorPopup.transform.DOScale(1f, 0.5f);
    }
    #endregion

    #region UNITY LISTENERES FOR BACKGROUND CODE

    void OnApplicationFocus(bool hasFocus)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isPaused = !hasFocus;

            if (isPaused)
            {
                ReconnectingObj.SetActive(true);
            }
            else
            {
                CheckForConnection = true;
            }
        }

    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;

        if (isPaused)
        {
            ReconnectingObj.SetActive(true);
        }
        else
        {
            CheckForConnection = true;
        }

    }
    #endregion;

}
