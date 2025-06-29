using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

[System.Serializable]
public class AxleInfos
{
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public bool motor;
    public bool steering;
}

public class JetRacerAgent : Agent
{
    // インスペクタ上で設定するパラメータ
    [Header("Base Parameter")]
    public float maxMotorTorque = 10.0f;  // 最大トルク
    public float maxSteeringAngle = 30.0f;  // 最大ステアリング角

    // パラメータが記述されたAssetへの参照
    [Space(10)]
    [Header("Course Settings")]
    public CourseSettings courseSettings;

    // ホイールへの参照
    [Space(10)]
    [Header("Wheel installation")]
    public List<AxleInfos> axleInfos;

    // 状態保存用変数
    private Vector3 startPosition;  // スタート位置
    private Quaternion startRotation;  // スタート角度

    private int latestCheckpoint;  // 通過済み最終チェックポイント

    private bool courseIn;  // コースイン状態
    private float courseOutTime;  // コースアウト時間

    private float totalReward;  // 収益
    private float stepReward;  // 1ステップの報酬

    private float step;  // 経過ステップ

    void Start()
    {
        // スタート位置の取得
        startPosition = transform.localPosition;
        startRotation = Quaternion.Euler(transform.localEulerAngles);
    }

    void OnTriggerStay(Collider other)
    {
        // コースイン状態を判定して記憶
        if (courseSettings.enableCourseOutCheck)
        {
            Match match = Regex.Match(other.gameObject.name, "Course");
            if (match.Success)
            {
                courseIn = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // コースアウトを判定して記憶
        if (courseSettings.enableCourseOutCheck)
        {
            Match match = Regex.Match(other.gameObject.name, $@"{courseSettings.courseBaseName}");
            if (match.Success)
            {
                courseIn = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // チェックポイントを判定して記憶
        if (courseSettings.enableCheckpoints)
        {
            Match match = Regex.Match(other.gameObject.name, $@"{courseSettings.checkpointBaseName} \((\d+)\)");
            if (match.Success)
            {
                // チェックポイント番号を取得
                int checkpoint = int.Parse(match.Groups[1].Value);  // 現在のチェックポイント

                // チェックポイント番号が連続であれば報酬を与える
                if (checkpoint == latestCheckpoint + 1)
                {
                    latestCheckpoint = checkpoint;
                    stepReward += courseSettings.checkpointReward;
                    AddReward(courseSettings.checkpointReward);

                    // ゴールした場合は報酬を上書きしてエピソードを終了
                    if (checkpoint == courseSettings.numCheckpoints)
                    {
                        totalReward += courseSettings.goalReward;
                        SetReward(courseSettings.goalReward);
                        EndEpisode();
                    }
                }
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        // 物理的状態の初期化
        transform.localPosition = startPosition;  // 位置
        transform.localRotation = startRotation;  // 回転
        SetAxleState(axleInfos, 0f, 0f);  // トルク・ステアリング角

        // 状態格納用変数の初期化
        if (courseSettings.enableCheckpoints)
        {
            latestCheckpoint = 0;
        }

        if (courseSettings.enableCourseOutCheck)
        {
            courseIn = true;
            courseOutTime = 0;
        }

        if (courseSettings.debugMode)
        {
            Debug.Log("Total Reward: " + totalReward + ", Episode Step: " + step);
        }
        totalReward = 0;
        stepReward = 0;

        step = 0;
    }

    void FixedUpdate()
    {
        // コースアウト時間の更新
        if (courseSettings.enableCourseOutCheck)
        {
            if (courseIn)
            {
                courseOutTime = 0;
            }
            else
            {
                courseOutTime += Time.fixedDeltaTime;
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 制御値を取得
        float motor = maxMotorTorque * actions.ContinuousActions[0];
        float steering = maxSteeringAngle * actions.ContinuousActions[1];

        // トルク・ステアリングを制御
        SetAxleState(axleInfos, motor, steering);

        if (courseSettings.enableCourseOutCheck)
        {
            // コースアウト状態のときにペナルティを与える
            if (courseSettings.applyPenaltyDuringCourseOut && !courseIn)
            {
                stepReward += courseSettings.courseOutPenalty;
                AddReward(courseSettings.courseOutPenalty);
            }

            // コースアウト時にペナルティを与えてエピソードを終了する
            if (courseSettings.endEpisodeWhenCourseOut && courseOutTime >= courseSettings.courseOutToleranceTime)
            {
                totalReward += courseSettings.courseOutEndPenalty;
                SetReward(courseSettings.courseOutEndPenalty);
                EndEpisode();
            }
        }

        totalReward += stepReward;
        stepReward = 0;

        step += 1;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ヒューリスティックモード用の操作設定
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    // ホイールのトルク・ステアリング角をセットする関数
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