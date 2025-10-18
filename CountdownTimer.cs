using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Start()
    {
        if (timerText != null)
        {
            // --- スタイルの設定 ---
            // 太字 & 大きめ
            timerText.fontStyle = FontStyles.Bold;
            timerText.fontSize = 200;

            // トマト色のグラデーション（上が赤、下が緑）
            timerText.enableVertexGradient = true;
            timerText.colorGradient = new VertexGradient(
                new Color(1f, 0.2f, 0.2f), // 上: 赤
                new Color(0.2f, 0.8f, 0.2f), // 下: 緑
                new Color(1f, 0.2f, 0.2f),
                new Color(0.2f, 0.8f, 0.2f)
            );

            // --- 表示するテキストを設定 ---
            timerText.text = "Finish!";
        }
    }
}