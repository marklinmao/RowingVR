//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialCameraRotation = Camera.main.transform.rotation;
        gameManager = (GameManager)Camera.main.GetComponent(typeof(GameManager));
    }

    void Update()
    {
        //show oxygen alarm screen
        if (gameManager.IsPlayingState())
        {

        }

        speedNumber = speedAccumulated.magnitude / 20;
        //TODO: show speed value in canvas

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