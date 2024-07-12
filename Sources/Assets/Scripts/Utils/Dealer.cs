using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ディーラー
/// </summary>
public class Dealer : MonoBehaviour {
    private DealerHand hand;   // 現在のハンド

    private void Awake() {
        this.Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="point">所持点数</param> 
    public void Initialize() {
        this.hand = null;
    }

    /// <summary>
    /// ハンドを追加する。
    /// </summary>
    /// <param name="hand">追加するハンド</param>
    public void AddHand(GameObject handPrefab) {
        if (this.hand is null) {
            this.hand = Instantiate(handPrefab, this.transform).GetComponent<DealerHand>();
        }
    }

    /// <summary>
    /// ハンドを初期化する。
    /// </summary>
    public void ResetHand() {
        if (this.hand is not null) {
            Destroy(this.hand.gameObject);
            this.hand = null;
        }
    }

    /// <summary>
    /// 現在のハンドを取得する。
    /// </summary>
    /// <returns>現在のハンド</returns>
    public DealerHand GetHand() {
        return this.hand;
    }
}
