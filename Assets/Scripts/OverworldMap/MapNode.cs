using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class MapNode : MonoBehaviour {
    public MapNodeSO nodeSO; // Node visuals
    public MapNodeProbSO nodeProbSO; // Type probabilities

    public List<MapNode> nodePaths; // All paths forward from this nodee
    public MapNodeType nodeType; // Can be Town, Battle, Shop, or Boss
    public GameObject backgroundObject;
    public bool specialNode; // If true, node is unique & should not be randomized

    private bool cleared;
    private SpriteRenderer icon;
    private SpriteRenderer background;

    private void Awake() {
        cleared = false;

        icon = GetComponent<SpriteRenderer>();
        background = backgroundObject.GetComponent<SpriteRenderer>();

        nodeProbSO.FixProbabilities();
    }

    // Set type and icon of this node and all node ahead of it
    public void Initialize() {
        if (!specialNode) { // Special nodes should not be randomized
            SetRandomType();
        }
        UpdateIcon();

        foreach (MapNode node in nodePaths) { // Initialize all nodes ahead of this one
            node.Initialize();
        }
    }

    // Randomly set node's type based on probabilities given by NodeProbSO
    private void SetRandomType() {
        float typeValue = Random.value;

        for (int type = 0 ; type < nodeProbSO.typeProb.Count ; type++) {
            typeValue -= nodeProbSO.typeProb[type];
            if (typeValue < 0) {
                nodeType = (MapNodeType)type; // Set to type associated with given int
                break;
            }
        }
    }

    // Updates icon to that of the node's type. If node is cleared, the icon and background will change.
    private void UpdateIcon() {
        if (cleared) { // If node has been cleared by player
            icon.sprite = nodeSO.clearedSprites[(int)nodeType]; // Use enum value as index
            background.color = nodeSO.background[^1]; // Cleared color is last index in list
        }
        else {
            // Use enum value as index
            icon.sprite = nodeSO.unclearedSprites[(int)nodeType];
            background.color = nodeSO.background[(int)nodeType];
        }
    }

    // Marks node as cleared
    public void ClearNode() {
        cleared = true;
        UpdateIcon();
    }

    // Optional clause, prevents consecutive shop nodes
    public void PreventConsecutiveShops() {
        foreach (MapNode node in nodePaths) {
            // If this node & a node directly ahead are both shops:
            if (this.nodeType == MapNodeType.Shop && node.nodeType == MapNodeType.Shop) {
                Debug.Log("Consecutive shop detected: " + node.name);
                node.nodeType = MapNodeType.Battle; // Change node ahead to battle
                node.UpdateIcon();
            }
            node.PreventConsecutiveShops(); // Call same function on next node
        }
    }
}