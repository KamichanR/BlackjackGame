using System.Collections.Generic;
using UnityEngine;

public class Shoe {
    private List<Card> initialCards;
    private List<Card> cards;
    private Dictionary<Card.Rank, int> numberOfRankCards;

    public Shoe(int numberOfDecks = 8, bool isIncludedSpecialCards = false) {
        this.initialCards = new List<Card>();
        this.cards = new List<Card>();
        this.numberOfRankCards = new Dictionary<Card.Rank, int>();

        foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) {
            this.numberOfRankCards.Add(rank, numberOfDecks * System.Enum.GetValues(typeof(Card.Suit)).Length);
        }

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

    public int NumberOfCards() {
        return this.cards.Count;
    }

    public int NumberOfCards(Card.Rank rank) {
        Debug.Log(rank.ToString() + ": " + this.numberOfRankCards[rank].ToString());
        return this.numberOfRankCards[rank];
    }

    public double Probability(Card.Rank rank) {
        return (double)this.NumberOfCards(rank) / this.NumberOfCards();
    }
}
