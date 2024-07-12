using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルカードを管理するクラス
/// </summary>
public class SkillCardManager : MonoBehaviour
{
    private List<SkillCard> skillCards; // スキルカードのリスト
    private SkillCard? selectedSkillCard; // 選択されたスキルカード

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void Awake() {
        this.skillCards = new List<SkillCard>();
        this.selectedSkillCard = null;
    }

    /// <summary>
    /// スキルカードをリセットする
    /// </summary>
    public void Reset() {
        foreach (SkillCard skillCard in this.skillCards) {
            Destroy(skillCard.transform.gameObject);
        }
        this.skillCards.Clear();
        this.selectedSkillCard = null;
    }

    /// <summary>
    /// スキルカードを生成し、リストに追加する。
    /// スキルカードの位置を更新する。
    /// </summary>
    /// <param name="skill">スキル</param>
    /// <param name="skillCardPrefab">スキルカードのプレハブ</param>
    public void AddSkillCard(SkillCard.Skill skill, GameObject skillCardPrefab) {
        SkillCard skillCard = Instantiate(skillCardPrefab, this.transform).GetComponent<SkillCard>();
        skillCard.SetSkill(skill);
        skillCard.SetSkillCardManager(this);
        this.skillCards.Add(skillCard);
        this.UpdatePositions();
    }

    /// <summary>
    /// 選択されたスキルカードを設定する
    /// </summary>
    /// <param name="skillCard">スキルカード</param>
    public void SetSkillCard(SkillCard skillCard) {
        this.selectedSkillCard = skillCard;
    }

    /// <summary>
    /// 選択されたスキルカードを取得する
    /// </summary>
    /// <returns>選択されたスキルカード</returns>
    public SkillCard? GetSelectedSkillCard() {
        return this.selectedSkillCard;
    }

    /// <summary>
    /// 選択されたスキルカードがnullでない場合、リストから削除し、オブジェクトを破棄する。
    /// その後、スキルカードの位置を更新する。
    /// </summary>
    public void RemoveSelectedSkillCard() {
        if (this.selectedSkillCard is not null) {
            this.skillCards.Remove(this.selectedSkillCard);
            Destroy(this.selectedSkillCard.transform.gameObject);
            this.selectedSkillCard = null;
            UpdatePositions();
        }
    }

    /// <summary>
    /// スキルカードの位置を更新する。
    /// </summary>
    private void UpdatePositions() {
        int count = 0;
        foreach (SkillCard skillCard in this.skillCards) {
            skillCard.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f * count);
            count++;
        }
    }
}
