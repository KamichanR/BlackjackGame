using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ゲームの情報を管理するクラス
/// </summary>
public class InformationManager : MonoBehaviour
{
    public TMP_Text aceLabel; // Aceの枚数を表示するためのラベル
    public TMP_Text twoLabel; // Twoの枚数を表示するためのラベル
    public TMP_Text threeLabel; // Threeの枚数を表示するためのラベル
    public TMP_Text fourLabel; // Fourの枚数を表示するためのラベル
    public TMP_Text fiveLabel; // Fiveの枚数を表示するためのラベル
    public TMP_Text sixLabel; // Sixの枚数を表示するためのラベル
    public TMP_Text sevenLabel; // Sevenの枚数を表示するためのラベル
    public TMP_Text eightLabel; // Eightの枚数を表示するためのラベル
    public TMP_Text nineLabel; // Nineの枚数を表示するためのラベル
    public TMP_Text tenLabel; // Tenの枚数を表示するためのラベル
    public TMP_Text jackLabel; // Jackの枚数を表示するためのラベル
    public TMP_Text queenLabel; // Queenの枚数を表示するためのラベル
    public TMP_Text kingLabel; // Kingの枚数を表示するためのラベル
    public TMP_Text ProbabilityUnder17Label; // 17未満になる確率を表示するためのラベル
    public TMP_Text ProbabilityOver16Label; // 17以上になる確率を表示するためのラベル
    public TMP_Text ProbabilityOver17Label; // 18以上になる確率を表示するためのラベル
    public TMP_Text ProbabilityOver18Label; // 19以上になる確率を表示するためのラベル
    public TMP_Text ProbabilityOver19Label; // 20以上になる確率を表示するためのラベル
    public TMP_Text ProbabilityOver20Label; // 21になる確率を表示するためのラベル
    public TMP_Text ProbabilityBustLabel; // バーストする確率を表示するためのラベル
    private Shoe shoe; // 山札
    private Card.Rank? holeCardRank; // ホールカードのランク

    /// <summary>
    /// 山札を設定する
    /// </summary>
    /// <param name="shoe">山札</param>
    public void SetShoe(Shoe shoe) {
        this.shoe = shoe;
    }

    /// <summary>
    /// ホールカードのランクを設定する
    /// </summary>
    /// <param name="holeCardRank">ホールカードのランク</param>
    public void SetHoleCardRank(Card.Rank holeCardRank) {
        this.holeCardRank = holeCardRank;
    }

    /// <summary>
    /// ホールカードのランクをリセットする
    /// </summary>
    /// <remarks>ホールカードのランクをnullに設定する</remarks>
    public void ResetHoleCardRank() {
        this.holeCardRank = null;
    }

    /// <summary>
    /// 各カードの枚数を更新する
    /// </summary>
    public void UpdateLabel() {
        aceLabel.text = (shoe.NumberOfCards(Card.Rank.Ace) + (this.holeCardRank == Card.Rank.Ace ? 1 : 0)).ToString();
        twoLabel.text = (shoe.NumberOfCards(Card.Rank.Two) + (this.holeCardRank == Card.Rank.Two ? 1 : 0)).ToString();
        threeLabel.text = (shoe.NumberOfCards(Card.Rank.Three) + (this.holeCardRank == Card.Rank.Three ? 1 : 0)).ToString();
        fourLabel.text = (shoe.NumberOfCards(Card.Rank.Four) + (this.holeCardRank == Card.Rank.Four ? 1 : 0)).ToString();
        fiveLabel.text = (shoe.NumberOfCards(Card.Rank.Five) + (this.holeCardRank == Card.Rank.Five ? 1 : 0)).ToString();
        sixLabel.text = (shoe.NumberOfCards(Card.Rank.Six) + (this.holeCardRank == Card.Rank.Six ? 1 : 0)).ToString();
        sevenLabel.text = (shoe.NumberOfCards(Card.Rank.Seven) + (this.holeCardRank == Card.Rank.Seven ? 1 : 0)).ToString();
        eightLabel.text = (shoe.NumberOfCards(Card.Rank.Eight) + (this.holeCardRank == Card.Rank.Eight ? 1 : 0)).ToString();
        nineLabel.text = (shoe.NumberOfCards(Card.Rank.Nine) + (this.holeCardRank == Card.Rank.Nine ? 1 : 0)).ToString();
        tenLabel.text = (shoe.NumberOfCards(Card.Rank.Ten) + (this.holeCardRank == Card.Rank.Ten ? 1 : 0)).ToString();
        jackLabel.text = (shoe.NumberOfCards(Card.Rank.Jack) + (this.holeCardRank == Card.Rank.Jack ? 1 : 0)).ToString();
        queenLabel.text = (shoe.NumberOfCards(Card.Rank.Queen) + (this.holeCardRank == Card.Rank.Queen ? 1 : 0)).ToString();
        kingLabel.text = (shoe.NumberOfCards(Card.Rank.King) + (this.holeCardRank == Card.Rank.King ? 1 : 0)).ToString();
    }

    /// <summary>
    /// ヒット時のスコアの確率を更新する
    /// </summary>
    public void UpdateProbability(PlayerHand hand) {
        double probabilityUnder17 = 0.0;
        double probabilityOver16 = 0.0;
        double probabilityOver17 = 0.0;
        double probabilityOver18 = 0.0;
        double probabilityOver19 = 0.0;
        double probabilityOver20 = 0.0;
        double probabilityBust = 0.0;

        int currentPoint = hand.GetPoint();
        bool isSoft = hand.IsSoft();
        // 各カードのランクに対して確率を計算する
        foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) {
            double probability = shoe.Probability(rank);
            
            int point = currentPoint + (int)rank;
            if (rank == Card.Rank.Ace && currentPoint <= 11) {
                point += 10;
            }
            if (isSoft && point > 21) {
                point -= 10;
            }
            
            if (point > 21) {
                probabilityBust += probability;
                continue;
            }

            if (point < 17) {
                probabilityUnder17 += probability;
                continue;
            }

            if (point > 16) {
                probabilityOver16 += probability;
            }
            if (point > 17) {
                probabilityOver17 += probability;
            }
            if (point > 18) {
                probabilityOver18 += probability;
            }
            if (point > 19) {
                probabilityOver19 += probability;
            }
            if (point > 20) {
                probabilityOver20 += probability;
            }
        }

        // 確率をテキストに表示する
        ProbabilityUnder17Label.text = probabilityUnder17.ToString("P");
        ProbabilityOver16Label.text = probabilityOver16.ToString("P");
        ProbabilityOver17Label.text = probabilityOver17.ToString("P");
        ProbabilityOver18Label.text = probabilityOver18.ToString("P");
        ProbabilityOver19Label.text = probabilityOver19.ToString("P");
        ProbabilityOver20Label.text = probabilityOver20.ToString("P");
        ProbabilityBustLabel.text = probabilityBust.ToString("P");
    }

    /// <summary>
    /// 確率の表示をリセットする
    /// </summary>
    public void ResetProbability() {
        ProbabilityUnder17Label.text = "";
        ProbabilityOver16Label.text = "";
        ProbabilityOver17Label.text = "";
        ProbabilityOver18Label.text = "";
        ProbabilityOver19Label.text = "";
        ProbabilityOver20Label.text = "";
        ProbabilityBustLabel.text = "";
    }
}
