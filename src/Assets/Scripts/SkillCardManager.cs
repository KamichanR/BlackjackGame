using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardManager : MonoBehaviour
{
    private List<SkillCard> skillCards;
    private SkillCard? selectedSkillCard;

    private void Awake() {
        this.skillCards = new List<SkillCard>();
        this.selectedSkillCard = null;
    }

    public void Reset() {
        foreach (SkillCard skillCard in this.skillCards) {
            Destroy(skillCard.transform.gameObject);
        }
        this.skillCards.Clear();
        this.selectedSkillCard = null;
    }

    public void AddSkillCard(SkillCard.Skill skill, GameObject skillCardPrefab) {
        SkillCard skillCard = Instantiate(skillCardPrefab, this.transform).GetComponent<SkillCard>();
        skillCard.SetSkill(skill);
        skillCard.SetSkillCardManager(this);
        this.skillCards.Add(skillCard);
        this.UpdatePositions();
    }

    public void SetSkillCard(SkillCard skillCard) {
        this.selectedSkillCard = skillCard;
    }

    public SkillCard? GetSelectedSkillCard() {
        return this.selectedSkillCard;
    }

    public void RemoveSelectedSkillCard() {
        if (this.selectedSkillCard is not null) {
            this.skillCards.Remove(this.selectedSkillCard);
            Destroy(this.selectedSkillCard.transform.gameObject);
            this.selectedSkillCard = null;
            UpdatePositions();
        }
    }

    private void UpdatePositions() {
        int count = 0;
        foreach (SkillCard skillCard in this.skillCards) {
            skillCard.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f * count);
            count++;
        }
    }
}
