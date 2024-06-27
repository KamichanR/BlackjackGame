using System.Collections.Generic;
using UnityEngine;

public class DealerHand : Hand {
    protected int hitLimitPoint;  // ディーラーがヒットできる最大の点数

    private void Awake() {
        this.Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cards">ハンドに含まれるカード</param>
    /// <param name="limitPoint">ハンドが取ることができる最大の点数</param>
    /// <param name="hitLimitPoint">ディーラーがヒットできる最大の点数</param>
    /// <remarks>デフォルトの<c>cards</c>は<c>null</c>。</remarks>
    /// <remarks>デフォルトの<c>limitPoint</c>は21。</remarks>
    /// <remarks>デフォルトの<c>hitLimitPoint</c>は16。</remarks>
    public void Initialize(List<Card> cards = null, int limitPoint = 21, int hitLimitPoint = 16) {
        base.Initialize(cards, limitPoint);
        this.hitLimitPoint = hitLimitPoint;
    }

    /// <summary>
    /// ハンドにカードを追加する。
    /// </summary>
    /// <param name="card">追加するカード</param>
    /// <remarks>点数も更新する。</remarks>
    /// <remarks>バーストしている場合は終了フラグを立てる。</remarks>
    new public GameObject AddCard(Card card, GameObject cardPrefab) {
        GameObject cardObject = base.AddCard(card, cardPrefab);
        cardObject.transform.localPosition = this.GetCardPosition();
        this.UpdateIsFinished();

        return cardObject;
    }

    /// <summary>
    /// カードのポジションを取得する。
    /// </summary>
    /// <returns>カードのポジション</returns>
    /// <remarks>カードのポジションは手札の枚数によって変化する。</remarks>
    private Vector3 GetCardPosition() {
        float numberOfCards = (float)(this.GetNumberOfCards() - 1);
        float x = 0.0f;
        float y = numberOfCards * 0.0001f;
        float z = - numberOfCards * 0.4f;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// ホールカードを公開する。
    /// </summary>
    public void RevealHoleCard() {
        if (!this.GetCard(1).GetIsFaceUp()) {
            this.GetCard(1).SetIsFaceUp(true);
            this.cardArea.transform.GetChild(1).transform.Rotate(-180, 0, 0);
            this.RecalculatePoint();
            this.UpdateIsFinished();
        }
    }

    /// <summary>
    /// ホールカードを取得する。
    /// </summary>
    /// <returns>ホールカード</returns>
    public Card GetHoleCard() {
        return this.GetCard(1);
    }

    private void UpdateIsFinished() {
        SetIsFinished(this.point > this.limitPoint - 5);
    }

    /// <summary>
    /// ディーラーがヒットできる最大の点数を設定する。
    /// </summary>
    /// <param name="hitLimitPoint">ディーラーがヒットできる最大の点数</param> 
    public void SetHitLimitPoint(int hitLimitPoint) {
        this.hitLimitPoint = hitLimitPoint;
    }
}
