using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    BattleController battleController;
    [SerializeField] List<Card> drawPile = new List<Card>();
    [SerializeField] List<Card> trash = new List<Card>();
    [SerializeField] List<GameObject> hand = new List<GameObject>();
    [SerializeField] Card temp;
    [SerializeField] GameObject card_template;
    [SerializeField] GameObject allcards_obj;
    AllCards allcards_class;
    TMP_Text drawPileNumber;
    TMP_Text trashNumber;
    int hand_limit = 8;
    private void Start() {
        battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
        allcards_class = allcards_obj.GetComponent<AllCards>();
        drawPileNumber = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        trashNumber = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        Init();
    }

    public void Init(List<Card> playerDeck){
        for (int i = 0; i < playerDeck.Count; i++){
            drawPile.Add( Card.Copy( playerDeck[i] ) );
        }
        Shuffle(drawPile);
    }

    public void Init(){
        List<Card> allcards = allcards_class.GetAllCards();
        int totalCards = allcards_class.GetCount();
        RemoveChild();
        drawPile.Clear();
        hand.Clear();
        trash.Clear();
        for (int i = 0; i < totalCards; i++){
            drawPile.Add( Card.Copy( allcards[i] ) );
        }
        Shuffle(drawPile);
    }

    // public void Init(){
    //     List<Card> allcards = allcards_class.GetAllCards();
    //     int totalCards = allcards_class.GetCount();
    //     for (int i = 0; i < 20; i++){
    //         drawPile.Add( Card.Copy( allcards[Random.Range(0, totalCards)] ) );
    //     }
    //     Shuffle(drawPile);
    // }

    void RemoveChild(){
        List<GameObject> list = new List<GameObject>();
        foreach(Transform child in transform){
            if (child.tag == "Card") list.Add(child.gameObject);
        }
        foreach(GameObject obj in list) Destroy(obj);
    }

    public void CardUsed(GameObject card){
        hand.Remove(card);
        trash.Add(card.GetComponent<CardDisplay>().thisCard);
        Destroy(card);
        Rearrange();
    }

    public void Discard(GameObject card){
        Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        hand.Remove(card);
        trash.Add(card.GetComponent<CardDisplay>().thisCard);
        Destroy(card);
        Rearrange();
        
        int tmp = player.GetStatus(Status.status.second_weapon);
        if (tmp > 0 && !battleController.DiscardedThisTurn())
            GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>().Draw(tmp);
        
        tmp = player.GetStatus(Status.status.fast_hand);
        if (tmp > 0) battleController.ReturnRandomEnemy().GetComponent<Character>().GetHit(tmp);
        
        battleController.Discarded();
    }

    void TurnEndDiscard(GameObject card){
        hand.Remove(card);
        trash.Add(card.GetComponent<CardDisplay>().thisCard);
        Destroy(card);
        Rearrange();
    }

    public void RemoveCard(GameObject card){
        Card card_info = card.GetComponent<CardDisplay>().thisCard;
        hand.Remove(card);
        if (card_info.id == 22){
            GameObject target = battleController.GetEnemyWithLowestHP();
            Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            player.Attack(target, card_info.Args[1]);
        }
        if (card_info.id == 23){
            AddCardToHand(card_info);
        }
        if (card_info.id == 24){
            Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            player.AddStatus(Status.status.strength, card_info.Args[1]);
        }
        Destroy(card);
        Rearrange();
    }

    public void AddCardToHand(Card card){
        if (hand.Count == hand_limit) return;
        hand.Add(MakeCard(card));
        Rearrange();
    }

    public void TurnEnd(){
        // foreach(GameObject card in hand){
        //     if (!card.GetComponent<CardDisplay>().thisCard.keep && !card.GetComponent<CardDisplay>().thisCard.keepBeforeUse){
        //         Discard(card);
        //     }
        // }
        for (int i = hand.Count - 1; i >= 0; i--){
            if (!hand[i].GetComponent<CardDisplay>().thisCard.keep && !hand[i].GetComponent<CardDisplay>().thisCard.keepBeforeUse){
                TurnEndDiscard(hand[i]);
            }
        }
    }

    public void Draw(){
        if (hand.Count == hand_limit) return;
        if (drawPile.Count > 0){
            hand.Add(MakeCard(drawPile[0]));
            drawPile.RemoveAt(0);
        }
        Rearrange();
    }

    public void Draw(int n){
        for (int i = 0; i < n; i++) Draw();
    }

    public void Rearrange(){
        int gap = 200;
        UpdateDeckNumbers();
        for (int i = 0; i < hand.Count; i++){
            if (hand[i].GetComponent<CardState>().state == CardState.State.ReadyToUse)
                hand[i].GetComponent<CardMove>().Move(new Vector3(-700, 300, 0));
            else hand[i].GetComponent<CardMove>().Move(new Vector3(-gap/2 * (hand.Count-1) + gap * i, -400, 0));
            // if (hand[i].GetComponent<CardState>().state == CardState.State.ReadyToUse)
            //     hand[i].GetComponent<CardState>().state = CardState.State.Normal;
            // if (hand[i].GetComponent<CardState>().state == CardState.State.Selected)
            //     hand[i].GetComponent<CardState>().Unselect();
        }
    }

    public void ResetHand(){
        for (int i = 0; i < hand.Count; i++){
            if (hand[i].GetComponent<CardState>().state == CardState.State.ReadyToUse)
                hand[i].GetComponent<CardState>().state = CardState.State.Normal;
            if (hand[i].GetComponent<CardState>().state == CardState.State.Selected)
                hand[i].GetComponent<CardState>().Unselect();
        }
    }

    public void UpdateHand(){
        for (int i = 0; i < hand.Count; i++){
            hand[i].GetComponent<CardDisplay>().LoadCard();
        }
    }

    void Shuffle(List<Card> deck){
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            Card tmp = deck[k];
            deck[k] = deck[n];
            deck[n] = tmp;
        }
    }

    void UpdateDeckNumbers(){
        drawPileNumber.text = drawPile.Count.ToString();
        trashNumber.text = trash.Count.ToString();
    }

    GameObject MakeCard(Card c){
        Card tmp = Card.Copy(c);
        return card_template.GetComponent<CardDisplay>().Make(tmp, transform);
    }
}
