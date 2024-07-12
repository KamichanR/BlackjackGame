using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// チップを管理するクラス
/// </summary>
public class ChipManager : MonoBehaviour
{
    private int bet; // ベット額
    private bool isPressedDealButton; // Dealボタンが押されたかどうか
    public TMP_Text BetLabel; // ベット額を表示するためのラベル
    public GameObject Value1Chip; // $1チップ
    public GameObject Value5Chip; // $5チップ
    public GameObject Value10Chip; // $10チップ
    public GameObject Value25Chip; // $25チップ
    public GameObject Value50Chip; // $50チップ
    public GameObject Value100Chip; // $100チップ
    public GameObject Value500Chip; // $500チップ
    public GameObject Value1000Chip; // $1000チップ
    public GameObject DealButton; // ディール開始ボタン
    public GameObject ResetBetButton; // ベットリセットボタン

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void Awake() {
        this.bet = 0;
        this.UpdateBetLabel();
    }

    /// <summary>
    /// チップ・ディール開始ボタン・ベットリセットボタンを表示する
    /// </summary>
    public void Activate() {
        this.Value1Chip.SetActive(true);
        this.Value5Chip.SetActive(true);
        this.Value10Chip.SetActive(true);
        this.Value25Chip.SetActive(true);
        this.Value50Chip.SetActive(true);
        this.Value100Chip.SetActive(true);
        this.Value500Chip.SetActive(true);
        this.Value1000Chip.SetActive(true);
        this.DealButton.SetActive(true);
        this.ResetBetButton.SetActive(true);
    }

    /// <summary>
    /// チップ・ディール開始ボタン・ベットリセットボタンを非表示にする
    /// </summary>
    public void Deactivate() {
        this.Value1Chip.SetActive(false);
        this.Value5Chip.SetActive(false);
        this.Value10Chip.SetActive(false);
        this.Value25Chip.SetActive(false);
        this.Value50Chip.SetActive(false);
        this.Value100Chip.SetActive(false);
        this.Value500Chip.SetActive(false);
        this.Value1000Chip.SetActive(false);
        this.DealButton.SetActive(false);
        this.ResetBetButton.SetActive(false);
    }

    /// <summary>
    /// ベット額を増やし、ラベルを更新する
    /// </summary>
    /// <param name="value">増やす額</param>
    public void AddBet(int value) {
        this.bet += value;
        this.UpdateBetLabel();
    }

    /// <summary>
    /// ベット額を取得する
    /// </summary>
    /// <returns>ベット額</returns>
    public int GetBet() {
        return this.bet;
    }

    /// <summary>
    /// ベット額をリセットし、ラベルを更新する
    /// </summary>
    public void Reset() {
        this.bet = 0;
        this.UpdateBetLabel();
        this.Activate();
    }

    /// <summary>
    /// ベット額の表示を更新する
    /// </summary>
    public void UpdateBetLabel() {
        this.BetLabel.text = this.bet.ToString();
    }

    /// <summary>
    /// Dealボタンが押されたかどうかを設定する
    /// </summary>
    /// <param name="value">Dealボタンが押されたかどうか</param>
    /// <remarks>Dealボタンが押された場合はtrue、それ以外はfalse</remarks>
    public void SetIsPressedDealButton(bool value) {
        this.isPressedDealButton = value;
    }

    /// <summary>
    /// Dealボタンが押されたかどうかを取得する
    /// </summary>
    /// <returns>Dealボタンが押されたかどうか</returns>
    /// <remarks>Dealボタンが押された場合はtrue、それ以外はfalse</remarks>
    public bool GetIsPressedDealButton() {
        return this.isPressedDealButton;
    }
}
