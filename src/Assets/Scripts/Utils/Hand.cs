using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hand : MonoBehaviour {
    protected List<Card> cards;   // ハンドに含まれるカード
    protected int limitPoint;     // ハンドが取ることができる最大の点数
    protected int point;          // ハンドの点数
    protected int[] pointList;    // ハンドの点数計算用のリスト
    protected bool isFinished;    // ハンドが終了しているか
    public GameObject cardArea;  // カードを表示するエリア
    public TMP_Text pointLabel; // 点数を表示するラベル

    private void Awake() {
        this.Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cards">ハンドに含まれるカード</param>
    /// <param name="limitPoint">ハンドが取ることができる最大の点数</param>
    /// <remarks>デフォルトの<c>cards</c>は<c>null</c>。</remarks>
    /// <remarks>デフォルトの<c>limitPoint</c>は21。</remarks>
    public void Initialize(List<Card> cards = null, int limitPoint = 21) {
        this.cards = cards ?? new List<Card>();
        this.limitPoint = limitPoint;
        this.isFinished = false;
        RecalculatePoint();
    }

    /// <summary>
    /// ハンドにカードを追加する。
    /// </summary>
    /// <param name="card">追加するカード</param>
    /// <param name="cardPrefab">カードのプレハブ</param>
    /// <remarks>点数も更新する。</remarks>
    /// <remarks>バーストしている場合は終了フラグを立てる。</remarks>
    public GameObject AddCard(Card card, GameObject cardPrefab) {
        cards.Add(card);
        GameObject cardObject = Instantiate(cardPrefab, cardArea.transform);
        if (!card.GetIsFaceUp()) {
            cardObject.transform.Rotate(180, 0, 0);
        }
        UpdatePoint(card);

        if (IsBusted()) {
            SetIsFinished(true);
        }

        return cardObject;
    }

    /// <summary>
    /// ハンドがバーストしているかを取得する。
    /// </summary>
    /// <returns>ハンドがバーストしているか</returns>
    public bool IsBusted() {
        return point > limitPoint;
    } 

    /// <summary>
    /// インデックスに対応するカードを取得する。
    /// </summary>
    /// <param name="index">インデックス</param>
    public Card GetCard(int index) {
        return cards[index];
    }

    public Card RemoveCard(int index) {
        Card card = this.GetCard(index);
        cards.RemoveAt(index);
        GameObject.Destroy(cardArea.transform.GetChild(index).gameObject);
        this.RecalculatePoint();

        return card;
    }

    /// <summary>
    /// 点数を再計算する。
    /// </summary>
    public void RecalculatePoint() {
        point = 0;
        pointList = new int[limitPoint + 1];

        foreach (Card card in cards) {
            UpdatePoint(card);
        }
    }
    
    /// <summary>
    /// 点数を更新する。
    /// </summary>
    private void UpdatePoint(Card card) {
        if (!card.GetIsFaceUp()) {
            return;
        }

        int numberOfCards = GetNumberOfCards();
        int softPoint = card.Point(true);
        int hardPoint = card.Point(false);
        int[] newPointList = new int[limitPoint + 1];

        for (int i = 0; i < limitPoint + 1; i++) {
            if (i < hardPoint) {
                newPointList[i] = pointList[i] + hardPoint;
                continue;
            }

            if (pointList[i] + softPoint <= i) {
                newPointList[i] = pointList[i] + softPoint;
                continue;
            }

            int[] candidates = new int[3];
            candidates[0] = pointList[i] + hardPoint;
            candidates[1] = pointList[i - hardPoint] + hardPoint;
            if (i < softPoint) {
                candidates[2] = candidates[1];
            } else {
                candidates[2] = pointList[i - softPoint] + softPoint;
            }
            Array.Sort(candidates);

            int j;
            for (j = 1; j < 3; j++) {
                if (candidates[j] > i) {
                    j--;
                    break;
                }
            }
            if (j == 3) {
                j--;
            }
            newPointList[i] = candidates[j];
        }

        Array.Copy(newPointList, pointList, limitPoint + 1);
        this.point = pointList[limitPoint];
        pointLabel.text = point.ToString();
    }

    public bool IsSoft() {
        int p = 0;

        foreach (Card card in cards) {
            p += card.Point();
        }

        return p != point;
    }

    /// <summary>
    /// ハンドがブラックジャックかを取得する。
    /// </summary>
    /// <returns>ハンドがブラックジャックか</returns>
    /// <remarks>ハンドがブラックジャックの条件は以下の通り。</remarks>
    /// <remarks>1. 点数が<c>limitPoint</c>に等しい。</remarks>
    /// <remarks>2. カードが2枚である。</remarks>
    public bool IsBlackjack() {
        return point == limitPoint && GetNumberOfCards() == 2;
    }

    /// <summary>
    /// 対象の効果カードの枚数を取得する。
    /// </summary>
    /// <param name="effect">効果</param>
    /// <returns>対象の効果カードの枚数</returns>
    public int NumberOfEffectCards(Card.Effect effect) {
        int count = 0;

        foreach (Card card in cards) {
            if (card.GetEffect() == effect) {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// ハンドが終了しているかを設定する。
    /// </summary>
    public void SetIsFinished(bool isFinished) {
        this.isFinished = isFinished;
    }

    /// <summary>
    /// ハンドが取ることができる最大の点数を設定する。
    /// </summary>
    public void SetLimitPoint(int limitPoint) {
        this.limitPoint = limitPoint;
        RecalculatePoint();
    }

    /// <summary>
    /// ハンドに含まれるカードの枚数を取得する。
    /// </summary>
    /// <returns>ハンドに含まれるカードの枚数</returns>
    public int GetNumberOfCards() {
        return cards.Count;
    }

    /// <summary>
    /// ハンドの点数を取得する。
    /// </summary>
    /// <returns>ハンドの点数</returns>
    public int GetPoint() {
        return this.point;
    }

    /// <summary>
    /// ハンドが終了しているかを取得する。
    /// </summary>
    /// <returns>ハンドが終了しているか</returns>
    public bool GetIsFinished() {
        return this.isFinished;
    }
}
