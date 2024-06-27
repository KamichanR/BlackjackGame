using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject hitButton;
    public GameObject standButton;
    public GameObject doubleButton;
    public GameObject tripleButton;
    public GameObject quadrupleButton;
    public GameObject splitButton;
    public GameObject nextGameButton;
    private Action? action;

    private void Awake() {
        this.Deactivate();
    }

    public void Deactivate() {
        this.hitButton.SetActive(false);
        this.standButton.SetActive(false);
        this.doubleButton.SetActive(false);
        this.tripleButton.SetActive(false);
        this.quadrupleButton.SetActive(false);
        this.splitButton.SetActive(false);
        this.nextGameButton.SetActive(false);
    }

    public void Activate(Player player) {
        this.hitButton.SetActive(true);
        this.standButton.SetActive(true);

        if (player.IsSplittable()) {
            this.splitButton.SetActive(true);
        }

        if (player.IsMultipleable(2)) {
            this.doubleButton.SetActive(true);
        }

        if (player.IsMultipleable(3)) {
            this.tripleButton.SetActive(true);
        }

        if (player.IsMultipleable(4)) {
            this.quadrupleButton.SetActive(true);
        }
    }

    public void ActivateNextGame() {
        this.nextGameButton.SetActive(true);
    }

    public void SetHitAction() {
        this.action = Action.Hit;
    }

    public void SetStandAction() {
        this.action = Action.Stand;
    }

    public void SetDoubleAction() {
        this.action = Action.Double;
    }

    public void SetTripleAction() {
        this.action = Action.Triple;
    }

    public void SetQuadrupleAction() {
        this.action = Action.Quadruple;
    }

    public void SetSplitAction() {
        this.action = Action.Split;
    }

    public void SetInsuranceAction() {
        this.action = Action.Insurance;
    }

    public void SetNextGameAction() {
        this.action = Action.NextGame;
    }

    public Action? GetAction() {
        return this.action;
    }

    public void ResetAction() {
        this.action = null;
    }
}
