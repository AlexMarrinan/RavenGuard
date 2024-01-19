using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelChest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closedSprite;
    private BaseTile attachedTile;
    private bool open = false;
    public void PlaceChest(BaseTile tile){
        tile.attachedChest = this;
        open = false;
        attachedTile = tile;
        this.transform.position = tile.transform.position;
        spriteRenderer.sprite = closedSprite;
    }
    public void OpenChest(){
        if (open){
            return;
        }
        spriteRenderer.sprite = openSprite;
        open = true;
        //TODO: GIVE PLAYER ITEMS FROM CHEST

        //TODO: DELETE CHEST AFTER OPENING
    }
    public void DeleteChest(){
        attachedTile.attachedChest = null;
        attachedTile = null;
        Destroy(this);
    }
}
