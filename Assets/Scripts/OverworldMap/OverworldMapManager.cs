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

    private Vector2 inputDirection = new();
    private bool selectActive = false;

    private void Awake() {
        // Singleton enforcement
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }

    // Setup nodes, node selector, & position node map
    private void Start() {
        InitializeNodes(); // Initialize all nodes
        NodeSelector.instance.Initialize(currentNode); // Initialize node selector

        PositionNodeMap();
    }

    // Change which direction the node selector will move depending on pressed keys
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            selectActive = true;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            inputDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            inputDirection = Vector2.down;
        }
    }

    // Select the currently highlighted node if space is pressed, or move node selector if a direction was given.
    private void FixedUpdate() {
        if (selectActive) {
            Select();
            selectActive = false;
            inputDirection = Vector2.zero; // Reset direction
        }

        if (inputDirection != Vector2.zero) {
            Move();
            inputDirection = Vector2.zero; // Reset direction
        }
    }

    // Move node selector to a different node
    private void Move() {
        AudioManager.instance.PlayMoveUI();

        NodeSelector.instance.ChangeSelection(inputDirection); // move node selector up or down
    }

    // Select the node currently highlighted by the node selector
    private void Select() { // TODO: Move player to level scene and so on.
        AudioManager.instance.PlayConfirm();

        currentNode = NodeSelector.instance.GetCurrentNode(); // Progress to next node

        NodeSelector.instance.Initialize(currentNode); // Progress node selector to next node
        currentNode.ClearNode(); // Mark node as cleared (as if the level that was just selected was finished)

        PositionNodeMap();
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
    private void PositionNodeMap() {
        float finalPosition = 0;

        // Find the average x-coordinate of each node directly ahead of the current one
        foreach (MapNode node in currentNode.nodePaths) {
            finalPosition += node.transform.position.x;
        }
        finalPosition /= currentNode.nodePaths.Count;

        nodeMap.transform.position = new(nodeMap.transform.position.x - finalPosition, 0); // Move nodemap left to center view on next nodes
    }
}