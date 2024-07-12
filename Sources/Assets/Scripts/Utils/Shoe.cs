using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 山札
/// </summary>
public class Shoe {
    private List<Card> initialCards; // 初期状態のカード
    private List<Card> cards; // 山札のカード
    private Dictionary<Card.Rank, int> numberOfRankCards; // ランクごとのカードの数

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="numberOfDecks">デッキの数</param>
    /// <param name="isIncludedSpecialCards">特殊カードを含めるか</param>
    /// <remarks>特殊カードを含める場合、スペードのAce, Seven, Jackで、ゴールド効果を持つカードを半分追加する。</remarks>
    public Shoe(int numberOfDecks = 8, bool isIncludedSpecialCards = false) {
        this.initialCards = new List<Card>();
        this.cards = new List<Card>();
        this.numberOfRankCards = new Dictionary<Card.Rank, int>();

        // ランクごとのカードの枚数を更新する
        foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) {
            this.numberOfRankCards.Add(rank, numberOfDecks * System.Enum.GetValues(typeof(Card.Suit)).Length);
        }

        // 山札を生成する
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit))) {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) {
                int n = isIncludedSpecialCards && suit == Card.Suit.Spade && (rank == Card.Rank.Ace || rank == Card.Rank.Seven || rank == Card.Rank.Jack) ? 2 : 1;
                for (int i = 0; i < numberOfDecks / n; i++) {
                    this.cards.Add(new Card(suit, rank));
                }
                for (int i = 0; i < numberOfDecks - numberOfDecks / n; i++) {
                    this.cards.Add(new Card(suit, rank, Card.Effect.Gold));
                }
            }
        }

        // シャッフルする
        this.Shuffle();
    }

    /// <summary>
    /// カードを引く。
    /// </summary>
    public Card DrawCard() {
        // エラー回避のための処理
        // カードがない場合はスペードのエースを返す。
        if (this.cards.Count == 0) {
            return new Card(Card.Suit.Spade, Card.Rank.Ace);
        }

        Card card = this.cards[0];
        this.numberOfRankCards[card.GetRank()]--;
        this.cards.RemoveAt(0);

        return card;
    }

    /// <summary>
    /// 低ランクのカードを引く。
    /// </summary>
    /// <remarks>低ランクのカードは2, 3, 4, 5, 6。</remarks>
    /// <remarks>低ランクのカードがない場合はランダムにカードを引く。</remarks>
    /// <returns>引いたカード</returns>
    public Card DrawLowCard() {
        // エラー回避のための処理
        // カードがない場合はスペードのエースを返す。
        if (this.cards.Count == 0) {
            return new Card(Card.Suit.Spade, Card.Rank.Ace);
        }

        int index = 0;
        while (index < this.NumberOfCards()) {
            Card card = this.cards[index];
            if (card.GetRank() == Card.Rank.Two || card.GetRank() == Card.Rank.Three ||
                card.GetRank() == Card.Rank.Four || card.GetRank() == Card.Rank.Five || card.GetRank() == Card.Rank.Six) {
                this.numberOfRankCards[card.GetRank()]--;
                this.cards.RemoveAt(index);
                return card;
            }
            index++;
        }

        return this.DrawCard();
    }

    /// <summary>
    /// 中ランクのカードを引く。
    /// </summary>
    /// <remarks>中ランクのカードは7, 8, 9。</remarks>
    /// <remarks>中ランクのカードがない場合はランダムにカードを引く。</remarks>
    /// <returns>引いたカード</returns>
    public Card DrawMiddleCard() {
        // エラー回避のための処理
        // カードがない場合はスペードのエースを返す。
        if (this.cards.Count == 0) {
            return new Card(Card.Suit.Spade, Card.Rank.Ace);
        }

        int index = 0;
        while (index < this.NumberOfCards()) {
            Card card = this.cards[index];
            if (card.GetRank() == Card.Rank.Seven || card.GetRank() == Card.Rank.Eight || card.GetRank() == Card.Rank.Nine) {
                this.numberOfRankCards[card.GetRank()]--;
                this.cards.RemoveAt(index);
                return card;
            }
            index++;
        }

        return this.DrawCard();
    }

    /// <summary>
    /// 高ランクのカードを引く。
    /// </summary>
    /// <remarks>高ランクのカードは10, J, Q, K, A。</remarks>
    /// <remarks>高ランクのカードがない場合はランダムにカードを引く。</remarks>
    /// <returns>引いたカード</returns>
    public Card DrawHighCard() {
        // エラー回避のための処理
        // カードがない場合はスペードのエースを返す。
        if (this.cards.Count == 0) {
            return new Card(Card.Suit.Spade, Card.Rank.Ace);
        }

        int index = 0;
        while (index < this.NumberOfCards()) {
            Card card = this.cards[index];
            if (card.GetRank() == Card.Rank.Ace || card.GetRank() == Card.Rank.Ten ||
                card.GetRank() == Card.Rank.Jack || card.GetRank() == Card.Rank.Queen ||
                card.GetRank() == Card.Rank.King) {
                this.numberOfRankCards[card.GetRank()]--;
                this.cards.RemoveAt(index);
                return card;
            }
            index++;
        }

        return this.DrawCard();
    }

    /// <summary>
    /// シューを初期状態のカードにして、シャッフルする。
    /// </summary>
    /// <remarks>シャッフルはFisher-Yatesアルゴリズムを使用する。</remarks>
    public void Reset() {
        this.cards = new List<Card>(this.initialCards);
        this.Shuffle();
    }

    /// <summary>
    /// シューをシャッフルする。
    /// </summary>
    /// <remarks>シャッフルはFisher-Yatesアルゴリズムを使用する。</remarks>
    public void Shuffle() {
        int numberOfCards = this.cards.Count;
        for (int i = 0; i < numberOfCards; i++)
        {
            int j = Random.Range(i, numberOfCards);
            Card temporaryCard = this.cards[i];
            this.cards[i] = this.cards[j];
            this.cards[j] = temporaryCard;
        }
    }

    /// <summary>
    /// 山札のカードの枚数を取得する。
    /// </summary>
    /// <returns>山札のカードの枚数</returns>
    public int NumberOfCards() {
        return this.cards.Count;
    }

    /// <summary>
    /// ランクごとのカードの枚数を取得する。
    /// </summary>
    /// <param name="rank">ランク</param>
    /// <returns>ランクごとのカードの枚数</returns>
    public int NumberOfCards(Card.Rank rank) {
        Debug.Log(rank.ToString() + ": " + this.numberOfRankCards[rank].ToString());
        return this.numberOfRankCards[rank];
    }

    /// <summary>
    /// ランクごとのカードの確率を取得する。
    /// </summary>
    /// <param name="rank">ランク</param>
    /// <returns>ランクごとのカードの確率</returns>
    public double Probability(Card.Rank rank) {
        return (double)this.NumberOfCards(rank) / this.NumberOfCards();
    }
}
