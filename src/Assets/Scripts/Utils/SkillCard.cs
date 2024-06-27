using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCard : MonoBehaviour {
    public enum Skill {
        BurnCard,
        FreeDoubleDown,
        HighCard,
        LookHoleCard,
        LowCard,
        MiddleCard,
        QuintupleDown,
        Split,
    }

    private Skill skill;
    private SkillCardManager skillCardManager;
    private int index;

    private void Awake() {
        this.skill = Skill.BurnCard;
    }

    public void SetSkill(Skill skill) {
        this.skill = skill;
    }

    public void SetSkillCardManager(SkillCardManager skillCardManager) {
        this.skillCardManager = skillCardManager;
    }

    public Skill GetSkill() {
        return this.skill;
    }

    public void OnClick() {
        Debug.Log(this.skill.ToString() + " is clicked.");
        this.skillCardManager.SetSkillCard(this);
    }

    public static string AssetReferenceName(Skill skill) {
        return skill.ToString() + "Prefab";
    }

    public static Skill GetSkillFromIndex(int index) {
        return (Skill)Enum.ToObject(typeof(Skill), index);
    }

    public static int NumberOfSkills() {
        return Enum.GetValues(typeof(Skill)).Length;
    }
}
