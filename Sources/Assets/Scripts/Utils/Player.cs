using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// プレイヤー
/// </summary>
public class Player : MonoBehaviour {
    private int point;  // 所持点数
    private PlayerHand hand;   // 現在のハンド
    private List<PlayerHand> doneHands; // プレイヤーの完了したハンド
    private List<PlayerHand> undoneHands;   // プレイヤーの完了していないハンド（現在のハンドを含まない）
    public TMP_Text pointLabel; // 所持点数を表示するラベル

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void Awake() {
        this.Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="point">所持点数</param> 
    public void Initialize() {
        this.point = 1000;
        this.hand = null;
        this.doneHands = new List<PlayerHand>();
        this.undoneHands = new List<PlayerHand>();
        this.UpdatePoint();
    }

    /// <summary>
    /// プレイヤーの所持点数を加算する。
    /// </summary>
    /// <param name="point">加算する点数</param> 
    public void AddPoint(int point) {
        this.point += point;
        this.UpdatePoint();
    }

    /// <summary>
    /// プレイヤーの所持点数表示を更新する。
    /// </summary>
    public void UpdatePoint() {
        this.pointLabel.text = this.point.ToString();
    }

    /// <summary>
    /// プレイヤーのハンドを追加する。
    /// </summary>
    /// <param name="hand">追加するハンド</param>
    /// <remarks>現在のハンドが存在しない場合は、<c>this.hand</c>に新しいハンドを追加する。</remarks>
    /// <remarks>現在のハンドが存在する場合は、<c>undoneHands</c>に新しいハンドを追加する。</remarks>
    public void AddHand(GameObject handPrefab, int bet) {
        Vector3 shiftPosition = new Vector3(0.0f, 0.0f, -1.5f);

        // すべてのハンドを移動する
        foreach (PlayerHand doneHand in doneHands) {
            doneHand.transform.localPosition += shiftPosition;
        }
        if (this.hand is not null) {
            this.hand.transform.localPosition += shiftPosition;
        }
        foreach (PlayerHand undoneHand in undoneHands) {
            undoneHand.transform.localPosition += shiftPosition;
        }

        // 新しいハンドを作成する
        PlayerHand hand = Instantiate(handPrefab, this.transform).GetComponent<PlayerHand>();
        for (int i = 0; i < this.GetNumberOfHands(); i++) {
            hand.transform.localPosition -= shiftPosition;
        }

        // ハンドに賭け金を設定する。
        hand.SetBet(bet);

        // ハンドを代入する
        if (this.hand is null) {
            this.hand = hand;
            this.hand.SetIsActive(true);
        } else {
            undoneHands.Add(hand);
        }

    }

    /// <summary>
    /// プレイヤーのハンドを次に進める。
    /// </summary>
    /// <remarks>現在のハンドが<c>null</c>である場合は、何もしない。</remarks>
    /// <remarks>現在のハンドを<c>doneHands</c>に追加する。</remarks>
    /// <remarks><c>undoneHands</c>が空の場合は、<c>this.hand</c>を<c>null</c>にする。</remarks>
    /// <remarks><c>undoneHands</c>が空でない場合は、<c>this.hand</c>に<c>undoneHands</c>の先頭のハンドを代入し、<c>undoneHands</c>から削除する。</remarks>
    public void NextHand() {
        if (this.hand is null) {
            return;
        }
        
        this.hand.SetIsActive(false);
        doneHands.Add(this.hand);
        if (undoneHands.Count == 0) {
            this.hand = null;
        } else {
            this.hand = undoneHands[0];
            this.hand.SetIsActive(true);
            undoneHands.RemoveAt(0);
        }
    }

    /// <summary>
    /// プレイヤーがスタンドする。
    /// </summary>
    public void Stand() {
        if (this.hand is null) {
            return;
        }
        this.hand.SetIsFinished(true);
    }

    /// <summary>
    /// ハンドを初期化する。
    /// </summary>
    public void ResetHands() {
        hand = null;
        doneHands.Clear();
        undoneHands.Clear();
        
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーがすべてのハンドを終了したかを取得する。
    /// </summary>
    /// <returns>プレイヤーがすべてのハンドを終了したか</returns>
    public bool IsDone() {
        return hand is null && undoneHands.Count == 0;
    }

    /// <summary>
    /// プレイヤーがマルチダウンできるかを取得する。
    /// </summary>
    /// <param name="multiplier">賭け金の倍率</param>
    /// <returns>プレイヤーがマルチダウンできるか</returns>
    public bool IsMultipleable(int multiplier) {
        if (this.hand is null) {
            return false;
        }

        return this.point >= this.hand.GetBet() * (multiplier - 1) && this.hand.GetNumberOfCards() <= 2;
    }

    /// <summary>
    /// プレイヤーがスプリットできるかを取得する。
    /// </summary>
    /// <returns>プレイヤーがスプリットできるか</returns>
    public bool IsSplittable() {
        if (this.hand is null) {
            return false;
        }

        return this.GetNumberOfHands() == 1 && this.hand.GetNumberOfCards() == 2 && 
            this.hand.GetCard(0).Point(false) == this.hand.GetCard(1).Point(false) && this.IsMultipleable(2);
    }

    /// <summary>
    /// プレイヤーの所持点数を取得する。
    /// </summary>
    /// <returns>プレイヤーの所持点数</returns>
    public int GetPoint() {
        return this.point;
    }

    /// <summary>
    /// 現在のハンドを取得する。
    /// </summary>
    /// <returns>現在のハンド</returns>
    public PlayerHand GetHand() {
        return this.hand;
    }

    /// <summary>
    /// プレイヤーの完了したハンドを取得する。
    /// </summary>
    /// <returns>プレイヤーの完了したハンド</returns> 
    public List<PlayerHand> GetDoneHands() {
        return doneHands;
    }

    /// <summary>
    /// プレイヤーのハンドの数を取得する。
    /// </summary>
    public int GetNumberOfHands() {
        return doneHands.Count + (hand is null ? 0 : 1) + undoneHands.Count;
    }

    /// <summary>
    /// 未完了のハンドを取得する。
    /// </summary>
    /// <returns>未完了のハンドのリスト</returns>
    public List<PlayerHand> GetUndoneHands() {
        return this.undoneHands;
    }
}
