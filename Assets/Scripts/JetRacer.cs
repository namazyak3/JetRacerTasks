using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetRacer : MonoBehaviour
{
    [Header("Base Parameter")]
    public float maxMotorTorque;
    public float maxSteeringAngle;

    [Space(5)]
    [Header("Wheel installation")]
    public List<AxleInfos> axleInfos;

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        SetAxleState(axleInfos, motor, steering);
    }

    private void SetAxleState(List<AxleInfos> axleInfos, float motor, float steering)
    {
        foreach (AxleInfos axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheelCollider.steerAngle = steering;
                axleInfo.rightWheelCollider.steerAngle = steering;
            }

            if (axleInfo.motor)
            {
                axleInfo.leftWheelCollider.motorTorque = motor;
                axleInfo.rightWheelCollider.motorTorque = motor;
            }
        }
    }
}