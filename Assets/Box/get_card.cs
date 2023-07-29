using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
public class get_card : MonoBehaviour
{
    // Start is called before the first frame update
    public Image cardIMG;
    public Sprite[] cardsprite;
    int current_card_index = 0;
    void OnEnable()
    {
        got_cards();
    }
    public void got_cards(){
        if(cardsprite.Length > 0){
            long currentTicks = DateTime.Now.Ticks;
            current_card_index = (int)currentTicks % (cardsprite.Length);
            if(current_card_index < 0){
                current_card_index -= 2 * current_card_index;
            }
            cardIMG.sprite = cardsprite[current_card_index];
            string cardname = cardsprite[current_card_index].name;
            int id;
            if(current_card_index < 10){
                id = (int)cardname[0] - '0';
                // print(id);
            }
            else{
                id = 10 * (cardname[0] - '0') + (cardname[1] - '0');
                
            }                
            // Global.AddItemToBag(id, "card");
            
        }
    }
    
}
