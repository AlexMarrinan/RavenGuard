using System.Collections;
using System.Collections.Generic;
using Game.Dialogue;
using Unity.VisualScripting;
using UnityEditor.Animations;
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

    // Change which direction the node selector will move depending on pressed keys
    private void Update() {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     selectActive = true;
        // }
        // if (Input.GetKeyDown(KeyCode.W)) {
        //     inputDirection = Vector2.up;
        // }
        // else if (Input.GetKeyDown(KeyCode.S)) {
        //     inputDirection = Vector2.down;
        // }
    }

    // Select the currently highlighted node if space is pressed, or move node selector if a direction was given.
    private void FixedUpdate() {
        // if (selectActive) {
        //     Select();
        //     selectActive = false;
        //     inputDirection = Vector2.zero; // Reset direction
        // }

        // if (inputDirection != Vector2.zero) {
        //     Move();
        //     inputDirection = Vector2.zero; // Reset direction
        // }
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

        NodeSelector.instance.Initialize(currentNode); // Progress node selector to next node
        currentNode.ClearNode(); // Mark node as cleared (as if the level that was just selected was finished)
        
        //TODO: LOAD NEW LEVEL HERE !!!
        //TODO: ADD SHOP LEVELS !!!
        if (true){
            PositionNodeMap();
            animator.SetBool("idle", true);
            GameManager.instance.LoadCombatLevel();
        }else{

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
        float finalOffset = 0;

        // Find the average x-coordinate of each node directly ahead of the current one
        foreach (MapNode node in currentNode.nodePaths) {
            finalOffset += (node.transform.localPosition.x - currentNode.transform.localPosition.x);
        }
        finalOffset /= currentNode.nodePaths.Count;

        Debug.Log(finalOffset);
        //TODO: MAKE MAP SCROLL BETTER
        nodeMap.transform.localPosition -= new Vector3(0.1f, 0); // Move nodemap left to center view on next nodes
    }
}