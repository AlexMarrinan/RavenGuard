using System.Collections;
using System.Collections.Generic;
using Game.Dialogue;
using Unity.VisualScripting;
using UnityEngine;

// Manages node logic for the Overworld Map
public class OverworldMapManager : MonoBehaviour
{
    public MapNode currentNode; // Player's current node
    public static OverworldMapManager instance;
    public OverworldMap overworldMap; // Reference to Scene's Menu(?) Manager
    public int mapSeed; // Seed for generating node types; Set to 0 for random.
    public GameObject nodeMap;

    private void Awake() {
        instance = this; // Why are we doing this?

        // Get list of node buttons for current node
        List<MenuButton> nodeButtons = new();
        foreach (MapNode node in currentNode.nodePaths) {
            nodeButtons.Add(node.button);
        }

        overworldMap.buttons = nodeButtons;
        // TODO: Update nodeButtons each time the currentNode changes

        InitializeNodes(); // Initialize all nodes

        PositionNodeMap();
    }

    public void Move(Vector2 direction){
        AudioManager.instance.PlayMoveUI();
        overworldMap.Move(direction);
    }
    public void Select(){
        AudioManager.instance.PlayConfirm();
        overworldMap.Select();
    }

    // Moves the player's current node to the given next node
    public void MoveNode(int path) {
        currentNode = currentNode.nodePaths[path];
    }

    // Set types of each node & update appearance
    private void InitializeNodes() {
        if (mapSeed != 0) {
            Random.InitState(mapSeed); // Use seed to determine node types
        }
        Debug.Log("Seed: " + Random.seed); // Logs seed; the warning isn't an issue

        currentNode.Initialize();

        // Optional clause: make sure multiple shops don't appear in a row
        PreventConsecutiveShops();
    }

    // Optional clause: make sure multiple shops don't appear in a row
    private void PreventConsecutiveShops() {
        currentNode.PreventConsecutiveShops();
    }

    // Positions the node map to focus on the current node's node paths
    private void PositionNodeMap() {
        List<Vector3> nodePositions = new();
        foreach (MapNode node in currentNode.nodePaths) {
            nodePositions.Add(node.transform.position);
        }
        Debug.Log("Node Positions:" + nodePositions.ToString());

        Vector3 finalPosition = nodePositions[0];
        if (nodePositions.Count == 1) {
            
        }
        else if (nodePositions.Count == 2) {
            finalPosition = Vector3.Lerp(nodePositions[0], nodePositions[1], 0.5f);
        }
        else if (nodePositions.Count == 3) {
            finalPosition = Vector3.Lerp(nodePositions[0], nodePositions[1], 0.5f) + Vector3.Lerp(nodePositions[1], nodePositions[2], 0.5f);
        }
        Debug.Log("Final Position Determined: " + finalPosition);
        // for (int x = 0 ; x < nodePositions.Count - 1 ; x++) {
        //     finalPosition = Vector3.Lerp(nodePositions[x], nodePositions[x + 1], 0.5f); // Find 50% distance between both nodes
        // }

        nodeMap.transform.position = finalPosition;
    }
}