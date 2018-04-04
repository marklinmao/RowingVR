//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using XRSettings = UnityEngine.XR.XRSettings;
//using UnityEngine.XR.XRSettings;


public class PlayerController : MonoBehaviour
{
    private const string MESSAGE_CANVAS_NAME = "MessageCanvas";
    private const string MESSAGE_TEXT_NAME = "MessageText";
    private const string LASER_GAMEOBJECT_NAME = "Laser";

    private const string CONTROLLER_CONNECTING_MESSAGE = "Controller connecting...";
    private const string CONTROLLER_DISCONNECTED_MESSAGE = "Controller disconnected";
    private const string CONTROLLER_SCANNING_MESSAGE = "Controller scanning...";
    private const string NON_GVR_PLATFORM =
      "Please select a supported Google VR platform via 'Build Settings > Android | iOS > Switch Platform'\n";
    private const string VR_SUPPORT_NOT_CHECKED =
      "Please make sure 'Player Settings > Virtual Reality Supported' is checked\n";
    private const string EMPTY_VR_SDK_WARNING_MESSAGE =
      "Please add 'Daydream' or 'Cardboard' under 'Player Settings > Virtual Reality SDKs'\n";

    // Java class, method, and field constants.
    private const int ANDROID_MIN_DAYDREAM_API = 24;
    private const string FIELD_SDK_INT = "SDK_INT";
    private const string PACKAGE_BUILD_VERSION = "android.os.Build$VERSION";
    private const string PACKAGE_DAYDREAM_API_CLASS = "com.google.vr.ndk.base.DaydreamApi";
    private const string METHOD_IS_DAYDREAM_READY = "isDaydreamReadyPlatform";

    private bool isDaydream = false;


    private GameManager gameManager;
    private Rigidbody rb;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initialCameraRotation;

    private float speed = 0f;
    private Vector3 speedAccumulated = new Vector3(0, 0, 0);
    public float speedNumber = 0f;
    private float acceleration = 0.5f;

    private float timestampOfClickBtnDown = 0.0f;
    private float timeElapsed = 0f;

    public GameObject messageCanvas;
    public Text messageText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialCameraRotation = Camera.main.transform.rotation;
        gameManager = (GameManager)Camera.main.GetComponent(typeof(GameManager));


        if (messageCanvas == null)
        {
            //messageCanvas = GameObject.Find(MESSAGE_CANVAS_NAME);
            messageCanvas = transform.Find(MESSAGE_CANVAS_NAME).gameObject;
            if (messageCanvas != null)
            {
                messageText = messageCanvas.transform.Find(MESSAGE_TEXT_NAME).GetComponent<Text>();
            }
        }
    }

    void Update()
    {
        //show oxygen alarm screen
        if (gameManager.IsPlayingState())
        {

        }

        speedNumber = speedAccumulated.magnitude / 20;
        //TODO: show speed value in canvas
        UpdateStatusMessage();

    }

    public static bool playerSettingsHasCardboard()
    {
        string[] playerSettingsVrSdks = XRSettings.supportedDevices;
        return Array.Exists<string>(playerSettingsVrSdks,
            element => element.Equals(GvrSettings.VR_SDK_CARDBOARD));
    }

    public static bool playerSettingsHasDaydream()
    {
        string[] playerSettingsVrSdks = XRSettings.supportedDevices;
        return Array.Exists<string>(playerSettingsVrSdks,
            element => element.Equals(GvrSettings.VR_SDK_DAYDREAM));
    }

    private void UpdateStatusMessage()
    {
/*
        if (messageText == null || messageCanvas == null)
        {
            return;
        }
#if !UNITY_ANDROID && !UNITY_IOS
      messageText.text = NON_GVR_PLATFORM;
      messageCanvas.SetActive(true);
      return;
#else
#if UNITY_EDITOR
        if (!UnityEditor.PlayerSettings.virtualRealitySupported)
        {
            messageText.text = VR_SUPPORT_NOT_CHECKED;
            messageCanvas.SetActive(true);
            return;
        }
#endif  // UNITY_EDITOR

        bool isVrSdkListEmpty = !playerSettingsHasCardboard() && !playerSettingsHasDaydream();
        if (!isDaydream)
        {
            if (messageCanvas.activeSelf)
            {
                messageText.text = EMPTY_VR_SDK_WARNING_MESSAGE;
                messageCanvas.SetActive(isVrSdkListEmpty);
            }
            return;
        }

        string vrSdkWarningMessage = isVrSdkListEmpty ? EMPTY_VR_SDK_WARNING_MESSAGE : "";
        string controllerMessage = "";
        GvrPointerGraphicRaycaster graphicRaycaster =
          messageCanvas.GetComponent<GvrPointerGraphicRaycaster>();
        // This is an example of how to process the controller's state to display a status message.
        switch (GvrControllerInput.State)
        {
            case GvrConnectionState.Connected:
                break;
            case GvrConnectionState.Disconnected:
                controllerMessage = CONTROLLER_DISCONNECTED_MESSAGE;
                messageText.color = Color.white;
                break;
            case GvrConnectionState.Scanning:
                controllerMessage = CONTROLLER_SCANNING_MESSAGE;
                messageText.color = Color.cyan;
                break;
            case GvrConnectionState.Connecting:
                controllerMessage = CONTROLLER_CONNECTING_MESSAGE;
                messageText.color = Color.yellow;
                break;
            case GvrConnectionState.Error:
                controllerMessage = "ERROR: " + GvrControllerInput.ErrorDetails;
                messageText.color = Color.red;
                break;
            default:
                // Shouldn't happen.
                Debug.LogError("Invalid controller state: " + GvrControllerInput.State);
                break;
        }
        messageText.text = string.Format("{0}\n{1}", vrSdkWarningMessage, controllerMessage);
        if (graphicRaycaster != null)
        {
            graphicRaycaster.enabled =
              !isVrSdkListEmpty || GvrControllerInput.State != GvrConnectionState.Connected;
        }
        messageCanvas.SetActive(isVrSdkListEmpty ||
                                (GvrControllerInput.State != GvrConnectionState.Connected));
#endif  // !UNITY_ANDROID && !UNITY_IOS
*/
        Vector3 accel_vector = GvrControllerInput.Accel;
        Vector3 gyro_vector = GvrControllerInput.Gyro;
        string vector_text = "X:  " + Math.Round(accel_vector.x, 2).ToString() +
            "\nY:  " + Math.Round(accel_vector.y, 2).ToString() +
            "\nZ:  " + Math.Round(accel_vector.z, 2).ToString();
        string gyro_text = "X:  " + gyro_vector.x.ToString() +
            "\nY:  " + gyro_vector.y.ToString() +
            "\nZ:  " + gyro_vector.z.ToString();
        messageCanvas.SetActive(true);
        double total_accel = accel_vector.x + accel_vector.y + accel_vector.z - 9.8;
        messageText.text = vector_text;
    }

    private void FixedUpdate()
    {

        if (gameManager.IsPlayingState())
        {
            // Speed up by clicking on TouchPad
            if (GvrControllerInput.ClickButton)
            {
                speed += acceleration;

                Vector3 targetPos = GvrControllerInput.Orientation * Vector3.forward;
                Vector3 direction = targetPos;
                rb.AddRelativeForce(direction.normalized * speed, ForceMode.Force);

                speedAccumulated = direction.normalized * speed + speedAccumulated;
            }
            else
            {
                speed = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Land"))
        {
            Debug.Log(">>>>>>>>>>>>>collision!>>>>>>>>>>>>>>>>>" + other.gameObject.tag + ", " + other.gameObject.name);
            gameManager.SwitchToFailureState();
        }
    }

}