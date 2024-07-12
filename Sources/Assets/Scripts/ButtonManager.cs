using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actionボタンを管理するクラス
/// </summary>
public class ButtonManager : MonoBehaviour
{
    public GameObject hitButton; // Hitボタン
    public GameObject standButton; // Standボタン
    public GameObject doubleButton; // Double(2x)ボタン
    public GameObject tripleButton; // Triple(3x)ボタン
    public GameObject quadrupleButton; // Quadruple(4x)ボタン
    public GameObject splitButton; // Splitボタン
    public GameObject nextGameButton; // Next Gameボタン
    private Action? action; // 選択されたアクション

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    /// <remarks>ボタンを非表示にする</remarks>
    private void Awake() {
        this.Deactivate();
    }

    /// <summary>
    /// すべてのActionボタンを非表示にする
    /// </summary>
    public void Deactivate() {
        this.hitButton.SetActive(false);
        this.standButton.SetActive(false);
        this.doubleButton.SetActive(false);
        this.tripleButton.SetActive(false);
        this.quadrupleButton.SetActive(false);
        this.splitButton.SetActive(false);
        this.nextGameButton.SetActive(false);
    }

    /// <summary>
    /// プレイヤーのアクションを選択するためのボタンを表示する
    /// </summary>
    /// <param name="player">プレイヤー</param>
    /// <remarks>Hit, Stand, Split, Double, Triple, Quadrupleボタンを表示する</remarks>
    /// <remarks>Split, Double, Triple, Quadrupleボタンは、プレイヤーが選択可能な場合のみ表示する</remarks>
    public void Activate(Player player) {
        this.hitButton.SetActive(true);
        this.standButton.SetActive(true);

        if (player.IsSplittable()) {
            this.splitButton.SetActive(true);
        }

        if (player.IsMultipleable(2)) {
            this.doubleButton.SetActive(true);
        }

        if (player.IsMultipleable(3)) {
            this.tripleButton.SetActive(true);
        }

        if (player.IsMultipleable(4)) {
            this.quadrupleButton.SetActive(true);
        }
    }

    /// <summary>
    /// Next Gameボタンを表示する
    /// </summary>
    public void ActivateNextGame() {
        this.nextGameButton.SetActive(true);
    }

    /// <summary>
    /// 選択されたアクションをHitに設定する
    /// </summary>
    public void SetHitAction() {
        this.action = Action.Hit;
    }

    /// <summary>
    /// 選択されたアクションをStandに設定する
    /// </summary>
    public void SetStandAction() {
        this.action = Action.Stand;
    }

    /// <summary>
    /// 選択されたアクションをDoubleに設定する
    /// </summary>
    public void SetDoubleAction() {
        this.action = Action.Double;
    }

    /// <summary>
    /// 選択されたアクションをTripleに設定する
    /// </summary>
    public void SetTripleAction() {
        this.action = Action.Triple;
    }

    /// <summary>
    /// 選択されたアクションをQuadrupleに設定する
    /// </summary>
    public void SetQuadrupleAction() {
        this.action = Action.Quadruple;
    }

    /// <summary>
    /// 選択されたアクションをSplitに設定する
    /// </summary>
    public void SetSplitAction() {
        this.action = Action.Split;
    }

    /// <summary>
    /// 選択されたアクションをNext Gameに設定する
    /// </summary>
    public void SetNextGameAction() {
        this.action = Action.NextGame;
    }

    /// <summary>
    /// 選択されたアクションを取得する
    /// </summary>
    /// <returns>選択されたアクション</returns>
    public Action? GetAction() {
        return this.action;
    }

    /// <summary>
    /// 選択されたアクションをリセットする
    /// </summary>
    /// <remarks>選択されたアクションをnullに設定する</remarks>
    public void ResetAction() {
        this.action = null;
    }
}
