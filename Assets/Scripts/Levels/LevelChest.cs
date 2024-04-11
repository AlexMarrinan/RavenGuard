using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelChest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer, itemSpriteRenderer, itemBGRSprite;
    public Sprite openSprite;
    public Sprite closedSprite;
    private BaseTile attachedTile;
    private bool open = false;
    private ParticleSystem chestOpenParticles;

    private void Awake() {
        chestOpenParticles = GetComponentInChildren<ParticleSystem>();
    }

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
        StartCoroutine(OpenChestAnimation());
    }
    IEnumerator OpenChestAnimation(){
        spriteRenderer.sprite = openSprite;
        open = true;

         //TODO: GIVE PLAYER ITEMS FROM CHEST
        yield return new WaitForSeconds(0.5f);
        BaseSkill[] allSkills = Resources.LoadAll<BaseSkill>("Skills/");
        int index = Random.Range(0, allSkills.Count());
        BaseSkill randomSkill = allSkills[index];

        itemSpriteRenderer.gameObject.SetActive(true);
        itemBGRSprite.gameObject.SetActive(true);
        itemSpriteRenderer.sprite = randomSkill.sprite;

        if (randomSkill is ActiveSkill){
            itemBGRSprite.color = SkillManager.instance.activeSkillColor;
        }else{
            itemBGRSprite.color = SkillManager.instance.passiveSkillColor;
        }
        chestOpenParticles.Play();
        yield return new WaitForSeconds(1.5f);
        InventoryManager.instance.AddItem(randomSkill);
        yield return new WaitForSeconds(0.25f);

        DeleteChest();
    }
    public void DeleteChest(){
//        Debug.Log("Deleting Chest!");
        attachedTile.attachedChest = null;
        attachedTile = null;
        Destroy(transform.gameObject);
    }
}
