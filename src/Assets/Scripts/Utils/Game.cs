public class Game {
    private Shoe shoe;
    private Player player;
    private Dealer dealer;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="numberOfDecks">デッキの数</param>
    /// <param name="playerPoint">プレイヤーの初期所持点数</param>
    public Game(Shoe shoe, Player player, Dealer dealer) {
        this.shoe = shoe;
        this.player = player;
        this.dealer = dealer;
    }

    public Shoe GetShoe() {
        return shoe;
    }

    public Player GetPlayer() {
        return player;
    }
    
    public Dealer GetDealer() {
        return dealer;
    }
}
