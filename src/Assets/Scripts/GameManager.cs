// using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    private Shoe shoe;
    private Dictionary<string, GameObject> prefabs;
    public Player player;
    public Dealer dealer;
    public ButtonManager buttonManager;
    public ChipManager chipManager;
    public SkillCardManager skillCardManager;
    public InformationManager informationManager;

    private void Awake() {
        this.shoe = new Shoe(numberOfDecks : 8, isIncludedSpecialCards : true);
        this.informationManager.SetShoe(this.shoe);
        this.prefabs = new Dictionary<string, GameObject>();
    }

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

    private IEnumerator LoadAssets() {
        Addressables.LoadAssetsAsync<GameObject>("Default", RegisterCardPrefab);

        yield return null;
    }

    private GameObject GetPrefab(string name) {
        if (!this.prefabs.ContainsKey(name)) {
            Debug.LogError("Prefab not found.");
            return null;
        }

        return this.prefabs[name];
    }

    private void RegisterCardPrefab(GameObject prefab) {
        this.prefabs.Add(prefab.name, prefab);
    }

    private IEnumerator ResetTurn() {
        this.player.ResetHands();
        this.dealer.ResetHand();
        this.chipManager.Reset();
        this.skillCardManager.Reset();
        this.UpdateInformation();
        this.informationManager.ResetProbability();
        yield return null;
    }

    private IEnumerator BetTurn() {
        yield return new WaitUntil(() => chipManager.GetIsPressedDealButton());
        chipManager.SetIsPressedDealButton(false);
        chipManager.Deactivate();
        yield return null;
    }

    /// <summary>
    /// ゲームを開始して、初めにプレイヤーとディーラーに2枚ずつカードを配る。
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

    private IEnumerator AddCard(PlayerHand hand, Card card = null) {
        if (card is null) {
            card = shoe.DrawCard();
        }
        GameObject cardPrefab = GetPrefab(card.AssetReferenceName());
        hand.AddCard(card, cardPrefab);
        this.UpdateInformation();
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator AddCard(DealerHand hand, bool isFaceUp = true) {
        Card card = shoe.DrawCard();
        card.SetIsFaceUp(isFaceUp);
        GameObject cardPrefab = GetPrefab(card.AssetReferenceName());
        hand.AddCard(card, cardPrefab);
        if (!isFaceUp) {
            this.informationManager.SetHoleCardRank(hand.GetHoleCard().GetRank());
        }
        this.UpdateInformation();
        yield return new WaitForSeconds(0.5f);
    }

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

    private IEnumerator PlayerTurn() {
        while (true) {
            buttonManager.Activate(player);
            yield return new WaitUntil(() => buttonManager.GetAction() is not null || skillCardManager.GetSelectedSkillCard() is not null);
            buttonManager.Deactivate();

            // アクションの処理
            if (buttonManager.GetAction() is not null) {
                Action action = buttonManager.GetAction() ?? Action.Stand;
                yield return new WaitForSeconds(0.5f);
                buttonManager.ResetAction();

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

            if (this.player.GetHand().GetIsFinished()) {
                this.player.NextHand();
            }

            if (this.player.GetHand() is null) {
                break;
            }
        }

        yield return null;
    }

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

    private IEnumerator JudgeTurn() {
        DealerHand dealerHand = dealer.GetHand();
        foreach (PlayerHand playerHand in player.GetDoneHands()) {
            this.Judge(playerHand, dealerHand);
        }

        yield return null;
    }

    private void Judge(PlayerHand playerHand, DealerHand dealerHand) {
        int playerPoint = playerHand.GetPoint();
        int dealerPoint = dealerHand.GetPoint();
        double payoutMultiplier = 1.0;

        if (playerHand.IsBusted()) {
            Debug.Log("Player Burst. Player Lose.");
            payoutMultiplier = 0.0;
        } else if (dealerHand.IsBusted()) {
            Debug.Log("Dealer Burst. Player Win.");
            payoutMultiplier = 2.0;
        } else if (playerHand.IsBlackjack() && dealerHand.IsBlackjack()) {
            Debug.Log("Draw.");
            payoutMultiplier = 1.0;
        } else if (playerHand.IsBlackjack()) {
            Debug.Log("Player Win.");
            payoutMultiplier = 2.5;
        } else if (dealerHand.IsBlackjack()) {
            Debug.Log("Player Lose.");
            payoutMultiplier = 0.0;
        } else if (playerPoint > dealerPoint) {
            Debug.Log("Player Win.");
            payoutMultiplier = 2.0;
        } else if (playerPoint < dealerPoint) {
            Debug.Log("Player Lose.");
            payoutMultiplier = 0.0;
        } else {
            Debug.Log("Draw.");
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

        int payout = (int)(playerHand.GetBet() * payoutMultiplier);
        player.AddPoint(payout);
    }

    private IEnumerator NextGameTurn() {
        buttonManager.ActivateNextGame();
        yield return new WaitUntil(() => buttonManager.GetAction() == Action.NextGame);
        buttonManager.Deactivate();

        yield return null;
    }

    public void OnClickChip(int value) {
        if (player.GetPoint() < value) {
            value = player.GetPoint();
        }
        player.AddPoint(-value);
        chipManager.AddBet(value);
    }

    public void OnClickResetBet() {
        player.AddPoint(chipManager.GetBet());
        this.chipManager.Reset();
    }

    public IEnumerator DealSkillCard() {
        for (int i = 0; i < Random.Range(1, 4); i++) {
            int index = Random.Range(0, SkillCard.NumberOfSkills());
            SkillCard.Skill skill = SkillCard.GetSkillFromIndex(index);
            this.AddSkillCard(skill);
        }

        yield return null;
    }

    public void AddSkillCard(SkillCard.Skill skill) {
        GameObject skillCardPrefab = GetPrefab(SkillCard.AssetReferenceName(skill));
        this.skillCardManager.AddSkillCard(skill, skillCardPrefab);
    }

    public void UpdateInformation() {
        this.informationManager.UpdateLabel();
        if (this.player.GetHand() is not null) {
            this.informationManager.UpdateProbability(this.player.GetHand());
        }
    }
}
