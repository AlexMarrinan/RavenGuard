using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public List<MapNode> nodePaths; // All paths forward from this nodee
    public MapNodeType nodeType; // Can be Town, Battle, Shop, or Boss
    public GameObject icon;
    public GameObject background;
    public bool specialNode; // If true, node is unique & should not be randomized
    public MenuButton button; // Jury-rigging button functionality

    private bool cleared; // Represents whether this node's stage has been cleared or not
    private SpriteRenderer iconSprite;
    private SpriteRenderer backgroundSprite;

    private void Awake() {
        cleared = false;

        iconSprite = icon.GetComponent<SpriteRenderer>();
        backgroundSprite = background.GetComponent<SpriteRenderer>();
    }

    public void Initialize() {
        Awake(); // Make sure Awake has been called

        if (!specialNode) { // Special nodes should not be randomized
            SetRandomType();
        }
        UpdateIcon();
        // Initialize all nodes ahead of this one
        foreach (MapNode node in nodePaths) {
            node.Initialize();
        }
    }

    // Sets the type of the node using the Random class
    private void SetRandomType() {
        float typeValue = Random.value;

        if (typeValue < 0.2) {
            nodeType = MapNodeType.Shop;
        }
        else {
            nodeType = MapNodeType.Battle;
        }
    }

    // Changes node icon to that of the node's type. If node is cleared, the icon pool will be completed versions.
    private void UpdateIcon() {
        if (cleared) { // If node has been cleared by player
            switch (nodeType) { // Switch for completed icons
                case MapNodeType.Town: 
                    // TODO: Replace placeholder with town sprite
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapLoreCompleteIcon");
                    break;
                case MapNodeType.Battle:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapBattleCompleteIcon");
                    break;
                case MapNodeType.Shop:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapShopCompleteIcon");
                    break;
                case MapNodeType.Boss:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapBossCompleteIcon");
                    break;
            }
            backgroundSprite.color = Color.white; // White color; indicates cleared node
        }
        else {
            switch (nodeType) { // Switch for incomplete icons
                case MapNodeType.Town: 
                    // TODO: Replace placeholder with town sprite
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapLoreCompleteIcon");
                    break;
                case MapNodeType.Battle:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapBattleIcon");
                    backgroundSprite.color = Color.red; // Red color
                    break;
                case MapNodeType.Shop:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapShopIcon");
                    backgroundSprite.color = Color.yellow; // Yellow color
                    break;
                case MapNodeType.Boss:
                    iconSprite.sprite = Resources.Load<Sprite>("Sprites/OverworldMap/MapBossIcon");
                    backgroundSprite.color = new(0.7f, 0, 0); // Dark red color
                    break;
            }
        }
    }

    // Marks the node as cleared by changing the icon
    public void ClearNode() {
        cleared = true;
        UpdateIcon();
    }

    // Optional clause, prevents consecutive shop nodes
    public void PreventConsecutiveShops() {
        foreach (MapNode node in nodePaths) {
            // If this node & a node directly ahead are both shops:
            if (this.nodeType == MapNodeType.Shop && node.nodeType == MapNodeType.Shop) {
                Debug.Log("Consecutive shop detected:" + node.name);
                node.nodeType = MapNodeType.Battle; // Change node ahead to battle
                node.UpdateIcon();
            }
            node.PreventConsecutiveShops(); // Call same function on next node
        }
    }
}
