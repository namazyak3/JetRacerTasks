using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSettings : MonoBehaviour
{
    public void Start()
    {
        // 物理演算の学習ステップ
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // フレーム制限の解除
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        // 品質レベルの設定
        QualitySettings.SetQualityLevel(0);

        // その他
        Application.runInBackground = true;
    }
}