using UnityEngine;

[CreateAssetMenu(menuName = "MLAgents/CourseSettings")]
public class CourseSettings : ScriptableObject
{
    [Header("CourseOut Settings")]
    public bool enableCourseOutCheck = true;  // コースアウトのチェックの有効化
    public string courseBaseName = "Course";  // コースの基本名
    public float courseOutToleranceTime = 0.5f;  // コースアウトの許容秒数

    [Space]
    public bool endEpisodeWhenCourseOut = true;  // コースアウトでエピソード終了

    [Space]
    public bool applyPenaltyOnCourseOutEnd = true;  // コースアウトによるエピソード終了時にペナルティを与える
    public float courseOutEndPenalty = -1.0f;  // コースアウトによるエピソード終了時に与えられるペナルティ

    [Space]
    public bool applyPenaltyDuringCourseOut = false; // コースアウト状態のときにペナルティを与える
    public float courseOutPenalty = -0.01f;  // コースアウト状態のときに与えられるペナルティ

    [Space(10)]
    [Header("Checkpoint Settings")]
    public bool enableCheckpoints = true;  // チェックポイントの有効化
    public string checkpointBaseName = "CP";  // チェックポイントGameObjectの基本名
    public int numCheckpoints = 0;  // チェックポイントの数
    public float checkpointReward = 1.0f;  // チェックポイント通過時の報酬
    public float goalReward = 10.0f;  // ゴール到着時の報酬

    [Space(10)]
    [Header("Other Settings")]
    public bool debugMode = false;  // デバッグモード (ログ出力)
}