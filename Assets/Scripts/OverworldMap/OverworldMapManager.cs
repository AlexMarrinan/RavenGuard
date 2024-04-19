using System.Collections;
using System.Collections.Generic;
using Game.Dialogue;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages node logic for the Overworld Map
public class OverworldMapManager : MonoBehaviour {
    public static OverworldMapManager instance;
    public MapNode currentNode; // Player's current node
    public int mapSeed; // Seed for generating node types; Set to 0 for random.
    public GameObject nodeMap;
    [SerializeField] private GameObject mapAssets;
    [SerializeField] private Animator animator; 
    private bool inOverworldMap;
    private void Awake() {
        // Singleton enforcement
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }
    public void ShowMap(bool value=true){
        animator.SetBool("idle", false);
        mapAssets.SetActive(value);
        inOverworldMap = value;
        mapAssets.transform.position = GameManager.instance.mainCamera.transform.position + new Vector3(-3f,1f,10f);
        MenuManager.instance.CloseMenus();
        PositionNodeMap();
    }
    public bool InOverworldMap(){
        return inOverworldMap;
    }
    public void OnFinishOpen(){
        animator.SetBool("idle", true);
    }
    // Setup nodes, node selector, & position node map
    public void StartMap() {
        MenuManager.instance.ToggleUnitSelectionMenu();
        ShowMap();
        
        InitializeNodes(); // Initialize all nodes
        NodeSelector.instance.Initialize(currentNode); // Initialize node selector
    }

    // Move node selector to a different node
    public void Move(Vector2 direction) {
        AudioManager.instance.PlayMoveUI();
        NodeSelector.instance.ChangeSelection(direction); // move node selector up or down
    }

    // Select the node currently highlighted by the node selector
    public void Select() {
        AudioManager.instance.PlayConfirm();

        currentNode = NodeSelector.instance.GetCurrentNode(); // Progress to next node

        if (GameManager.instance.levelNumber < 7){
            NodeSelector.instance.Initialize(currentNode); // Progress node selector to next node
        }
        currentNode.ClearNode(); // Mark node as cleared (as if the level that was just selected was finished)
        
        //TODO: LOAD NEW LEVEL HERE !!!
        //TODO: ADD SHOP LEVELS !!!
        PositionNodeMap();
        animator.SetBool("idle", true);
        if (currentNode.nodeType == MapNodeType.Shop ){
            GameManager.instance.LoadShopLevel();
        }else{
            GameManager.instance.LoadCombatLevel();
        }
    }

    // Moves the player's current node to the given next node
    public void MoveNode(int path) {
        currentNode = currentNode.nodePaths[path];
    }

    // Set types of each node & update appearance
    private void InitializeNodes() {
        if (mapSeed == 0) { // If no seed was given
            mapSeed = Random.seed; // Get random seed
        }
        Random.InitState(mapSeed); // Seed will change Random.value, which will change node types
        Debug.Log("Seed: " + mapSeed);

        currentNode.Initialize(); // Goes down the node "tree", initializing everything

        // Optional clauses:
        PreventConsecutiveShops(); // Make sure multiple shops don't appear in a row
    }

    // Optional clause: make sure multiple shops don't appear in a row
    private void PreventConsecutiveShops() {
        currentNode.PreventConsecutiveShops();
    }

    // Positions the node map to focus on the current node's paths
    public void PositionNodeMap() {
        if (GameManager.instance.levelNumber >= 7){
            return;
        }
        float finalOffset = 0;

        // Find the average x-coordinate of each node directly ahead of the current one
        foreach (MapNode node in currentNode.nodePaths) {
            finalOffset += (node.transform.localPosition.x - currentNode.transform.localPosition.x);
        }
        finalOffset /= currentNode.nodePaths.Count;

        //TODO: MAKE MAP SCROLL BETTER
        nodeMap.transform.localPosition -= new Vector3(0.1f, 0); // Move nodemap left to center view on next nodes
    }
}