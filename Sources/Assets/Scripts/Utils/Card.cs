/// <summary>
/// カードクラス
/// </summary>
public class Card {
    /// <summary>
    /// スートの列挙型
    /// </summary>
    public enum Suit {
        Spade,
        Heart,
        Diamond,
        Club,
    }

    /// <summary>
    /// ランクの列挙型
    /// </summary>
    public enum Rank {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
    }

    /// <summary>
    /// 効果の列挙型
    /// </summary>
    public enum Effect {
        Normal,
        Gold,
    }

    private Suit suit;
    private Rank rank;
    private Effect effect;
    private bool isFaceUp;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="suit">スート</param>
    /// <param name="rank">ランク</param>
    /// <param name="effect">効果</param>
    /// <param name="isFaceUp">表向きであるか</param>
    public Card(Suit suit, Rank rank, Effect effect = Effect.Normal, bool isFaceUp = true) {
        this.suit = suit;
        this.rank = rank;
        this.effect = effect;
        this.isFaceUp = isFaceUp;
    }

    /// <summary>
    /// カードのポイントを取得する。
    /// </summary>
    /// <param name="isSoft"></param>
    /// <returns>カードのポイント</returns>
    /// <remarks>isSoftがtrueの場合、Aceは11として扱う。</remarks>
    /// <remarks>isSoftがfalseの場合、Aceは1として扱う。</remarks>
    /// <remarks>Jack, Queen, Kingは10として扱う。</remarks>
    /// <remarks>それ以外のカードはそのランクの値として扱う。</remarks>
    public int Point(bool isSoft = false) {
        if (rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King) {
            return 10;
        }

        if (rank == Rank.Ace) {
            return isSoft ? 11 : 1;
        }

        return (int)rank;
    }

    /// <summary>
    /// カードのアセット名を取得する。
    /// </summary>
    public string AssetReferenceName() {
        return effect.ToString() + suit.ToString() + ((int)rank).ToString("D2") + "Prefab";
    }

    /// <summary>
    /// カードが表向きであるかを取得する。
    /// </summary>
    /// <returns>カードが表向きであるか</returns>
    public bool GetIsFaceUp() {
        return isFaceUp;
    }

    /// <summary>
    /// 表向きであるかを設定する。
    /// </summary>
    /// <param name="isFaceUp">表向きであるか</param>
    public void SetIsFaceUp(bool isFaceUp) {
        this.isFaceUp = isFaceUp;
    }

    /// <summary>
    /// カードの効果を取得する。
    /// </summary>
    /// <returns>カードの効果</returns>
    public Effect GetEffect() {
        return effect;
    }

    /// <summary>
    /// カードのスートを取得する。
    /// </summary>
    /// <returns>カードのスート</returns>
    public Rank GetRank() {
        return rank;
    }
}
