using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a node selector for the overworld map
public class NodeSelector : MonoBehaviour {
    public static NodeSelector instance;

    private List<MapNode> nodePaths; // All nodes directly ahead of the player's current node
    private int currentNode; // Zero-based index of the currently selected node

    private void Awake() {
        // Singleton enforcement
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }

    // Initializes node selector using the player's current node
    public void Initialize(MapNode node) {
        nodePaths = node.nodePaths; // Nodes directly ahead of given node
        currentNode = 0; // First element in the list

        MoveToNode(); // Move selector to first node in new list
    }

    // Changes the node the selector is focused on
    public void ChangeSelection(Vector2 direction) {
        currentNode -= (int) direction.y; // direction.y is always an int. Use -= because nodes go from top to bottom.

        // Handle overflow/underflow cases
        if (currentNode >= nodePaths.Count) { // If selection went beyond last node
            currentNode = 0; // Wrap around to first node
        }
        else if (currentNode < 0) { // If selection went behind first node
            currentNode = nodePaths.Count - 1; // Wrap around to last node
        }

        MoveToNode();
    }

    // Moves node selector visual to the currently selected node
    private void MoveToNode() {
        this.transform.position = nodePaths[currentNode].transform.position; // Node Selector position equals selected node's position
    }

    // Getter for currently selected node
    public MapNode GetCurrentNode() {
        return nodePaths[currentNode];
    }
}
