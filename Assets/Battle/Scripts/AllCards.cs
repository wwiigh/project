using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCards : MonoBehaviour
{
    [SerializeField] List<Card> allCards;
    List<Card> allSkillCards = new List<Card>();
    List<Card> allAttackCards = new List<Card>();

    public int GetCount(){
        return allCards.Count;
    }

    public List<Card> GetAllCards(){
        return allCards;
    }

    public Card GetRandomSkillCard(){
        if (allSkillCards.Count == 0){
            foreach(Card c in allCards){
                if (c.type == Card.Type.skill) allSkillCards.Add(c);
            }
        }

        return allSkillCards[Random.Range(0, allSkillCards.Count)];
    }

    public Card GetRandomAttackCard(){
        if (allAttackCards.Count == 0){
            foreach(Card c in allCards){
                if (c.type == Card.Type.attack) allAttackCards.Add(c);
            }
        }

        return allAttackCards[Random.Range(0, allAttackCards.Count)];
    }
}
