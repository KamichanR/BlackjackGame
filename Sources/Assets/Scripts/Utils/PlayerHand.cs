using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのハンド
/// </summary>
public class PlayerHand : Hand {
    protected int bet;            // ハンドに賭けられている金額
    protected bool isActive;      // ハンドがアクティブか
    public GameObject baseObject; // カードのプレハブ
    public Material activeMaterial; // アクティブ時のマテリアル
    public Material deactiveMaterial;  // 非アクティブ時のマテリアル

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void Awake() {
        this.Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cards">ハンドに含まれるカード</param>
    /// <param name="limitPoint">ハンドが取ることができる最大の点数</param>
    /// <param name="bet">ハンドに賭けられている金額</param>
    /// <remarks>デフォルトの<c>cards</c>は<c>null</c>。</remarks>
    /// <remarks>デフォルトの<c>limitPoint</c>は21。</remarks>
    /// <remarks>デフォルトの<c>bet</c>は0。</remarks>
    public void Initialize(List<Card> cards = null, int limitPoint = 21, int bet = 0) {
        base.Initialize(cards, limitPoint);
        this.bet = bet;
    }

    /// <summary>
    /// ハンドにカードを追加する。
    /// </summary>
    /// <param name="card">追加するカード</param>
    /// <param name="cardPrefab">カードのプレハブ</param>
    /// <remarks>点数も更新する。</remarks>
    /// <remarks>バーストしている場合は終了フラグを立てる。</remarks>
    new public GameObject AddCard(Card card, GameObject cardPrefab) {
        GameObject cardObject = base.AddCard(card, cardPrefab);

        cardObject.transform.localPosition = this.GetCardPosition();

        return cardObject;
    }

    /// <summary>
    /// ハンドの最後のカードを削除する。
    /// </summary>
    public void RemoveLastCard() {
        int numberOfCards = this.GetNumberOfCards();

        if (numberOfCards > 0) {
            this.RemoveCard(numberOfCards - 1);
        }
    }

    /// <summary>
    /// カードのポジションを取得する。
    /// </summary>
    /// <returns>カードのポジション</returns>
    /// <remarks>カードのポジションは手札の枚数によって変化する。</remarks>
    private Vector3 GetCardPosition() {
        float numberOfCards = (float)(this.GetNumberOfCards() - 1);
        float x = numberOfCards * 0.4f;
        float y = numberOfCards * 0.0001f;
        float z = - numberOfCards * 0.4f;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 賭け金を設定する。
    /// </summary>
    /// <param name="bet">賭け金</param>
    public void SetBet(int bet) {
        this.bet = bet;
    }

    /// <summary>
    /// ハンドがアクティブかを設定する。
    /// </summary>
    /// <param name="isActive">アクティブであるか</param>
    public void SetIsActive(bool isActive) {
        this.isActive = isActive;

        if (this.isActive) {
            this.baseObject.GetComponent<Renderer>().material = this.activeMaterial;
        } else {
            this.baseObject.GetComponent<Renderer>().material = this.deactiveMaterial;
        }
    }

    /// <summary>
    /// ハンドを終了する。
    /// </summary>
    /// <param name="isFinished">終了しているか</param>
    new public void SetIsFinished(bool isFinished) {
        base.SetIsFinished(isFinished);

        this.SetIsActive(false);
    }

    /// <summary>
    /// 賭け金を取得する。
    /// </summary>
    public int GetBet() {
        return bet;
    }
}
