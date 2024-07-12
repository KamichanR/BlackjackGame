using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルカード
/// </summary>
public class SkillCard : MonoBehaviour {
    /// <summary>
    /// スキルの列挙体
    /// </summary>
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

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void Awake() {
        this.skill = Skill.BurnCard;
    }

    /// <summary>
    /// スキルを設定する。
    /// </summary>
    /// <param name="skill">設定するスキル</param>
    public void SetSkill(Skill skill) {
        this.skill = skill;
    }

    /// <summary>
    /// スキルカードマネージャーを設定する。
    /// </summary>
    /// <param name="skillCardManager">設定するスキルカードマネージャー</param>
    public void SetSkillCardManager(SkillCardManager skillCardManager) {
        this.skillCardManager = skillCardManager;
    }

    /// <summary>
    /// スキルを取得する。
    /// </summary>
    /// <returns>スキル</returns>
    public Skill GetSkill() {
        return this.skill;
    }

    /// <summary>
    /// クリックされたときに呼び出されるメソッド
    /// </summary>
    public void OnClick() {
        this.skillCardManager.SetSkillCard(this);
    }

    /// <summary>
    /// スキルからアセット名を取得する
    /// </summary>
    /// <param name="skill">スキル</param>
    /// <returns>アセット名</returns>
    public static string AssetReferenceName(Skill skill) {
        return skill.ToString() + "Prefab";
    }

    /// <summary>
    /// インデックスからスキルを取得する。
    /// </summary>
    /// <param name="index">インデックス</param>
    /// <returns>スキル</returns>
    public static Skill GetSkillFromIndex(int index) {
        return (Skill)Enum.ToObject(typeof(Skill), index);
    }

    /// <summary>
    /// スキルの種類数を取得する。
    /// </summary>
    /// <returns>スキルの種類数</returns>
    public static int NumberOfSkills() {
        return Enum.GetValues(typeof(Skill)).Length;
    }
}
