using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChipManager : MonoBehaviour
{
    private int bet;
    private bool isPressedDealButton;
    public TMP_Text BetLabel;
    public GameObject Value1Chip;
    public GameObject Value5Chip;
    public GameObject Value10Chip;
    public GameObject Value25Chip;
    public GameObject Value50Chip;
    public GameObject Value100Chip;
    public GameObject Value500Chip;
    public GameObject Value1000Chip;
    public GameObject DealButton;
    public GameObject ResetBetButton;

    private void Awake() {
        this.bet = 0;
        this.UpdateBetLabel();
    }

    public void Activate() {
        this.Value1Chip.SetActive(true);
        this.Value5Chip.SetActive(true);
        this.Value10Chip.SetActive(true);
        this.Value25Chip.SetActive(true);
        this.Value50Chip.SetActive(true);
        this.Value100Chip.SetActive(true);
        this.Value500Chip.SetActive(true);
        this.Value1000Chip.SetActive(true);
        this.DealButton.SetActive(true);
        this.ResetBetButton.SetActive(true);
    }

    public void Deactivate() {
        this.Value1Chip.SetActive(false);
        this.Value5Chip.SetActive(false);
        this.Value10Chip.SetActive(false);
        this.Value25Chip.SetActive(false);
        this.Value50Chip.SetActive(false);
        this.Value100Chip.SetActive(false);
        this.Value500Chip.SetActive(false);
        this.Value1000Chip.SetActive(false);
        this.DealButton.SetActive(false);
        this.ResetBetButton.SetActive(false);
    }

    public void AddBet(int value) {
        this.bet += value;
        this.UpdateBetLabel();
    }

    public int GetBet() {
        return this.bet;
    }

    public void Reset() {
        this.bet = 0;
        this.UpdateBetLabel();
        this.Activate();
    }

    public void UpdateBetLabel() {
        this.BetLabel.text = this.bet.ToString();
    }

    public void SetIsPressedDealButton(bool value) {
        this.isPressedDealButton = value;
    }

    public bool GetIsPressedDealButton() {
        return this.isPressedDealButton;
    }
}
