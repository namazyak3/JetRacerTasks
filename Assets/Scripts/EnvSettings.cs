using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSettings : MonoBehaviour
{
    public void Start()
    {
        // 物理演算の学習ステップ
        Time.fixedDeltaTime = 0.05f;

        // フレーム制限の解除
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        // 品質レベルの設定
        QualitySettings.SetQualityLevel(0);

        // ウィンドウサイズの削減
        Screen.SetResolution(160, 120, false);

        // その他
        Application.runInBackground = true;
    }
}