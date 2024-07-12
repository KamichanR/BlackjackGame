using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// ゲーム全体を管理するクラス。
/// ゲームの進行を管理する。
/// </summary>
public class GameManager : MonoBehaviour
{
    private Shoe shoe;                              // 山札
    private Dictionary<string, GameObject> prefabs; // 読み込んだプレハブを管理する辞書
    public Player player;                           // プレイヤー
    public Dealer dealer;                           // ディーラー
    public ButtonManager buttonManager;             // ボタン管理クラス
    public ChipManager chipManager;                 // チップ管理クラス
    public SkillCardManager skillCardManager;       // スキルカード管理クラス
    public InformationManager informationManager;   // 情報管理クラス

    /// <summary>
    /// ゲーム開始時に呼び出される。
    /// 山札を生成し、情報管理クラスに山札をセットする。
    /// </summary>
    private void Awake() {
        this.shoe = new Shoe(numberOfDecks : 8, isIncludedSpecialCards : true);
        this.informationManager.SetShoe(this.shoe);
        this.prefabs = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// 起動時、アセットを読み込む。
    /// 次に、ゲームの進行を管理するコルーチンを開始する。
    /// </summary>
    private IEnumerator Start() {
        yield return StartCoroutine(this.LoadAssets());

        while (true) {
            yield return StartCoroutine(this.ResetTurn());
            yield return StartCoroutine(this.BetTurn());
            yield return StartCoroutine(this.DealTurn());
            yield return StartCoroutine(this.PlayerTurn());
            yield return StartCoroutine(this.DealerTurn());
            yield return StartCoroutine(this.JudgeTurn());
            yield return StartCoroutine(this.NextGameTurn());
        }
    }

    /// <summary>
    /// アセットを読み込む。
    /// </summary>
    private IEnumerator LoadAssets() {
        Addressables.LoadAssetsAsync<GameObject>("Default", RegisterCardPrefab);

        yield return null;
    }

    /// <summary>
    /// プレハブを取得する。
    /// </summary>
    /// <param name="name">プレハブの名前</param>
    /// <returns>プレハブ</returns>
    /// <remarks>プレハブが存在しない場合はnullを返す。</remarks>
    private GameObject GetPrefab(string name) {
        if (!this.prefabs.ContainsKey(name)) {
            return null;
        }

        return this.prefabs[name];
    }

    /// <summary>
    /// プレハブを登録する。
    /// </summary>
    /// <param name="prefab">プレハブ</param>
    /// <remarks>プレハブの名前をキーとして登録する。</remarks>
    private void RegisterCardPrefab(GameObject prefab) {
        this.prefabs.Add(prefab.name, prefab);
    }

    /// <summary>
    /// ターンをリセットする。
    /// プレイヤーとディーラーのハンドをリセットし、チップをリセットする。
    /// その後、情報を更新する。
    /// </summary>
    private IEnumerator ResetTurn() {
        this.player.ResetHands();
        this.dealer.ResetHand();
        this.chipManager.Reset();
        this.skillCardManager.Reset();
        this.UpdateInformation();
        this.informationManager.ResetProbability();
        yield return null;
    }

    /// <summary>
    /// 賭けるチップを考えるシーン。
    /// チップを賭けるボタンを押すまで待機する。
    /// </summary>
    private IEnumerator BetTurn() {
        yield return new WaitUntil(() => chipManager.GetIsPressedDealButton());
        chipManager.SetIsPressedDealButton(false);
        chipManager.Deactivate();
        yield return null;
    }

    /// <summary>
    /// ゲームを開始して、初めにプレイヤーとディーラーに2枚ずつカードを配る。
    /// その後、スキルカードを配る。
    /// </summary>
    private IEnumerator DealTurn() {
        // プレイヤーにハンドを追加
        GameObject playerHandPrefab = GetPrefab("PlayerHandPrefab");
        player.AddHand(playerHandPrefab, this.chipManager.GetBet());
        PlayerHand playerHand = player.GetHand();

        // ディーラーにハンドを追加
        GameObject dealerHandPrefab = GetPrefab("DealerHandPrefab");
        dealer.AddHand(dealerHandPrefab);
        DealerHand dealerHand = dealer.GetHand();

        // カードを配る
        yield return this.AddCard(playerHand);
        yield return this.AddCard(dealerHand);
        yield return this.AddCard(playerHand);
        yield return this.AddCard(dealerHand, isFaceUp : false);

        // スキルカードを配る
        yield return this.DealSkillCard();

        yield return null;
    }

    /// <summary>
    /// プレイヤーにカードを配る。
    /// 情報も更新する。
    /// </summary>
    /// <param name="hand">プレイヤーのハンド</param>
    /// <param name="card">カード</param>
    /// <remarks>カードが指定されていない場合は山札から引く。</remarks>
    private IEnumerator AddCard(PlayerHand hand, Card card = null) {
        if (card is null) {
            card = shoe.DrawCard();
        }
        GameObject cardPrefab = GetPrefab(card.AssetReferenceName());
        hand.AddCard(card, cardPrefab);
        this.UpdateInformation();
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// ディーラーにカードを配る。
    /// 情報も更新する。
    /// </summary>
    /// <param name="hand">ディーラーのハンド</param>
    /// <param name="isFaceUp">表向きかどうか</param>
    /// <param name="card">カード</param>
    /// <remarks>カードが指定されていない場合は山札から引く。</remarks>
    /// <remarks>表向きかどうかが指定されていない場合はtrueを指定する。</remarks>
    /// <remarks>表向きでない場合は、情報管理クラスに裏のカードのランクをセットする。</remarks>
    private IEnumerator AddCard(DealerHand hand, bool isFaceUp = true, Card card = null) {
        if (card is null) {
            card = shoe.DrawCard();
        }
        card.SetIsFaceUp(isFaceUp);
        GameObject cardPrefab = GetPrefab(card.AssetReferenceName());
        hand.AddCard(card, cardPrefab);
        if (!isFaceUp) {
            this.informationManager.SetHoleCardRank(hand.GetHoleCard().GetRank());
        }
        this.UpdateInformation();
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// マルチダウンを行う。
    /// プレイヤーのハンドのベットを増やし、カードを引く。
    /// </summary>
    /// <param name="multiplier">倍率</param>
    /// <remarks>倍率が1未満の場合は1に設定する。</remarks>
    private IEnumerator MultipleDown(int multiplier) {
        if (multiplier < 1) {
            multiplier = 1;
        }

        PlayerHand hand = this.player.GetHand();
        int bet = hand.GetBet();
        this.chipManager.AddBet(bet * (multiplier - 1));
        this.player.AddPoint(- bet * (multiplier - 1));
        hand.SetBet(bet * multiplier);
        yield return this.AddCard(hand);
        this.player.Stand();
        yield return null;
    }

    /// <summary>
    /// ハンドを分割する。
    /// プレイヤーにハンドを追加し、カードを移動する。
    /// </summary>
    /// <remarks>分割できない場合は何もしない。</remarks>
    /// <remarks>分割したハンドには、元のハンドのベットと同じベットを設定する。</remarks>
    /// <remarks>分割したハンドには、元のハンドのカードのうち1枚を移動する。</remarks>
    /// <remarks>分割した・されたハンドで、山札からカードを引く。</remarks>
    private IEnumerator SplitHand() {
        // プレイヤーにハンドを追加
        GameObject playerHandPrefab = GetPrefab("PlayerHandPrefab");
        player.AddHand(playerHandPrefab, bet : player.GetHand().GetBet());
        PlayerHand newPlayerHand = player.GetUndoneHands().LastOrDefault();

        // ポイントの処理
        this.player.AddPoint(- newPlayerHand.GetBet());
        this.chipManager.AddBet(newPlayerHand.GetBet());

        // カードを移動
        Card card = player.GetHand().RemoveCard(1);
        GameObject cardPrefab = GetPrefab(card.AssetReferenceName());
        newPlayerHand.AddCard(card, cardPrefab);

        // カードを追加
        yield return this.AddCard(player.GetHand());
        yield return this.AddCard(newPlayerHand);
    }

    /// <summary>
    /// プレイヤーのターン。
    /// プレイヤーのアクションを待機し、アクションに応じて処理を行う。
    /// プレイヤーのハンドが終了した場合は次のハンドに移る。
    /// プレイヤーのハンドがなくなった場合は終了する。
    /// スキルカードが選択された場合は、スキルカードの効果を適用する。
    /// </summary>
    private IEnumerator PlayerTurn() {
        while (true) {
            buttonManager.Activate(player);
            yield return new WaitUntil(() => buttonManager.GetAction() is not null || skillCardManager.GetSelectedSkillCard() is not null); // ボタンが押されるまで待機
            buttonManager.Deactivate(); // ボタンを非アクティブ化

            // アクションの処理
            if (buttonManager.GetAction() is not null) {
                Action action = buttonManager.GetAction() ?? Action.Stand;  // アクションを取得（nullの場合はStand）
                yield return new WaitForSeconds(0.5f);
                buttonManager.ResetAction();    // アクションをリセット

                switch (action) {
                    case Action.Hit:
                        yield return this.AddCard(this.player.GetHand());
                        break;
                    case Action.Stand:
                        this.player.Stand();
                        break;
                    case Action.Double:
                        yield return this.MultipleDown(2);
                        break;
                    case Action.Triple:
                        yield return this.MultipleDown(3);
                        break;
                    case Action.Quadruple:
                        yield return this.MultipleDown(4);
                        break;
                    case Action.Split:
                        yield return this.SplitHand();
                        break;
                    default:
                        break;
                }
            }

            // スキルカードの処理
            if (skillCardManager.GetSelectedSkillCard() is not null) {
                SkillCard skillCard = skillCardManager.GetSelectedSkillCard();
                SkillCard.Skill skill = skillCard.GetSkill();

                switch (skillCard.GetSkill()) {
                    case SkillCard.Skill.BurnCard:
                        this.player.GetHand().RemoveLastCard();
                        break;
                    case SkillCard.Skill.FreeDoubleDown:
                        this.player.AddPoint(this.player.GetHand().GetBet());
                        yield return this.MultipleDown(2);
                        break;
                    case SkillCard.Skill.HighCard:
                        yield return this.AddCard(this.player.GetHand(), this.shoe.DrawHighCard());
                        break;
                    case SkillCard.Skill.LookHoleCard:
                        this.dealer.GetHand().RevealHoleCard();
                        break;
                    case SkillCard.Skill.LowCard:
                        yield return this.AddCard(this.player.GetHand(), this.shoe.DrawLowCard());
                        break;
                    case SkillCard.Skill.MiddleCard:
                        yield return this.AddCard(this.player.GetHand(), this.shoe.DrawMiddleCard());
                        break;
                    case SkillCard.Skill.QuintupleDown:
                        if (this.player.IsMultipleable(5)) {
                            yield return this.MultipleDown(5);
                        }
                        break;
                    case SkillCard.Skill.Split:
                        if (this.player.GetNumberOfHands() == 1 && this.player.GetHand().GetNumberOfCards() == 2) {
                            yield return this.SplitHand();
                        }
                        break;
                    default:
                        break;
                }

                skillCardManager.RemoveSelectedSkillCard();
            }

            // 現在のハンドが終了している場合、次のハンドに移る
            if (this.player.GetHand().GetIsFinished()) {
                this.player.NextHand();
            }

            // プレイヤーのハンドがなくなった場合、終了
            if (this.player.GetHand() is null) {
                break;
            }
        }

        yield return null;
    }

    /// <summary>
    /// ディーラーのターン。
    /// ディーラーのハンドが終了するまでカードを引く。
    /// </summary>
    private IEnumerator DealerTurn() {
        yield return new WaitForSeconds(1.0f);
        DealerHand hand = this.dealer.GetHand();
        hand.RevealHoleCard();
        this.informationManager.ResetHoleCardRank();
        this.UpdateInformation();

        while (!hand.GetIsFinished()) {
            yield return this.AddCard(hand);
        }

        yield return null;
    }

    /// <summary>
    /// プレイヤーの各ハンドに対して、勝敗を判定する。
    /// </summary>
    private IEnumerator JudgeTurn() {
        DealerHand dealerHand = dealer.GetHand();
        foreach (PlayerHand playerHand in player.GetDoneHands()) {
            this.Judge(playerHand, dealerHand);
        }

        yield return null;
    }

    /// <summary>
    /// プレイヤーのハンドとディーラーのハンドを比較し、勝敗を判定する。
    /// </summary>
    /// <param name="playerHand">プレイヤーのハンド</param>
    /// <param name="dealerHand">ディーラーのハンド</param>
    private void Judge(PlayerHand playerHand, DealerHand dealerHand) {
        int playerPoint = playerHand.GetPoint();
        int dealerPoint = dealerHand.GetPoint();
        double payoutMultiplier = 1.0;

        if (playerHand.IsBusted()) { // プレイヤーがバーストしている場合
            payoutMultiplier = 0.0;
        } else if (dealerHand.IsBusted()) { // ディーラーがバーストしている場合
            payoutMultiplier = 2.0;
        } else if (playerHand.IsBlackjack() && dealerHand.IsBlackjack()) { // プレイヤーとディーラーがナチュラルブラックジャックの場合
            payoutMultiplier = 1.0;
        } else if (playerHand.IsBlackjack()) { // プレイヤーがナチュラルブラックジャックかつディーラーがナチュラルブラックジャックでない場合
            payoutMultiplier = 2.5;
        } else if (dealerHand.IsBlackjack()) { // ディーラーがナチュラルブラックジャックかつプレイヤーがナチュラルブラックジャックでない場合
            payoutMultiplier = 0.0;
        } else if (playerPoint > dealerPoint) { // プレイヤーがディーラーよりポイントが高い場合
            payoutMultiplier = 2.0;
        } else if (playerPoint < dealerPoint) { // プレイヤーがディーラーよりポイントが低い場合
            payoutMultiplier = 0.0;
        } else { // プレイヤーとディーラーのポイントが同じ場合
            payoutMultiplier = 1.0;
        }

        // 効果がGoldであるカードの枚数に応じて、配当を増やす
        if (payoutMultiplier > 1.0) {
            int numberOfGoldCards = playerHand.NumberOfEffectCards(Card.Effect.Gold);
            if (playerHand.IsBlackjack()) {
                payoutMultiplier -= 1;
                payoutMultiplier *= System.Math.Pow(2, numberOfGoldCards);
                payoutMultiplier += 1;
            } else {
                payoutMultiplier += numberOfGoldCards;
            }
        }

        // プレイヤーに配当を支払う
        int payout = (int)(playerHand.GetBet() * payoutMultiplier);
        player.AddPoint(payout);
    }

    /// <summary>
    /// 次のゲームに進む。
    /// </summary>
    private IEnumerator NextGameTurn() {
        buttonManager.ActivateNextGame();
        yield return new WaitUntil(() => buttonManager.GetAction() == Action.NextGame);
        buttonManager.Deactivate();

        yield return null;
    }

    /// <summary>
    /// チップをクリックしたときの処理。
    /// プレイヤーのポイントがチップの値より少ない場合は、ポイントを全て賭ける。
    /// プレイヤーのポイントからチップの値を引く。
    /// チップの値をベットに追加する。
    /// </summary>
    public void OnClickChip(int value) {
        if (player.GetPoint() < value) {
            value = player.GetPoint();
        }
        player.AddPoint(-value);
        chipManager.AddBet(value);
    }

    /// <summary>
    /// ベットリセットボタンをクリックしたときの処理。
    /// プレイヤーのポイントにベットを追加する。
    /// </summary>
    public void OnClickResetBet() {
        player.AddPoint(chipManager.GetBet());
        this.chipManager.Reset();
    }

    /// <summary>
    /// スキルカードを配布する。
    /// スキルカードの枚数は0枚から3枚の間でランダムに決定する。
    /// スキルカードの種類もランダムに選び、プレイヤーに配布する。
    /// </summary>
    public IEnumerator DealSkillCard() {
        for (int i = 0; i < Random.Range(0, 4); i++) {
            int index = Random.Range(0, SkillCard.NumberOfSkills());
            SkillCard.Skill skill = SkillCard.GetSkillFromIndex(index);
            this.AddSkillCard(skill);
        }

        yield return null;
    }

    /// <summary>
    /// スキルカードを追加する。
    /// </summary>
    /// <param name="skill">スキル</param>
    public void AddSkillCard(SkillCard.Skill skill) {
        GameObject skillCardPrefab = GetPrefab(SkillCard.AssetReferenceName(skill));
        this.skillCardManager.AddSkillCard(skill, skillCardPrefab);
    }

    /// <summary>
    /// 情報を更新する。
    /// </summary>
    /// <remarks>情報管理クラスでラベルを更新する。</remarks>
    /// <remarks>プレイヤーのハンドが存在する場合、確率を更新する。</remarks>
    public void UpdateInformation() {
        this.informationManager.UpdateLabel();
        if (this.player.GetHand() is not null) {
            this.informationManager.UpdateProbability(this.player.GetHand());
        }
    }
}
