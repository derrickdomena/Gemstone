using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateLevel : MonoBehaviour
{
    [SerializeField] int maxRooms;
    [SerializeField] List<GameObject> spawnRooms = new List<GameObject>();
    private List<GameObject> qualifiedRooms = new List<GameObject>();
    private List<GameObject> nodes = new List<GameObject>();
    private List<GameObject> neighborNodes = new List<GameObject>();
    private List<string> noNeighbors = new List<string>();
    public  List<GameObject> bobShop = new List<GameObject>();

    //Nodes to find
    private string nodeN = "NodeN";
    private string nodeE = "NodeE";
    private string nodeS = "NodeS";
    private string nodeW = "NodeW";

    //check neighbors for doors
    public bool hasDoorN = false;
    public bool hasDoorE = false;
    public bool hasDoorS = false;
    public bool hasDoorW = false;

    private GameObject currentNode;
    GameObject neighbor;
    int roomCount;

    private Vector2Int index;
    bool generating = false;
    bool roomCountSet = false;
    bool bossRoom = false;
    private void Awake()
    {
    }
    private void Start()
    {
        StartCoroutine(SpawnSpawn());

    }
    private void Update()
    {
        if(gameManager.instance.generated == true && !roomCountSet)
        {
            roomCountSet = true;
            gameManager.instance.roomCount = roomCount;
        }

        if (nodes.Count > 0 && !generating)
        {
            StartCoroutine(Generate());
        }
        else if (nodes.Count <= 0 && !generating)
        {
           // Debug.Log("No nodes");
        }
        else if (nodes.Count <= 0 && generating)
        {
            gameManager.instance.generated = true;
        }
    }

    private IEnumerator SpawnSpawn()
    {
        generating = true;
        int randomRoom = UnityEngine.Random.Range(0, spawnRooms.Count);

        // Instantiate the first room at the center of the grid
        Vector2Int center = new Vector2Int(gameManager.instance.level.GetLength(0) / 2, gameManager.instance.level.GetLength(1) / 2);


        if (spawnRooms.Count > 0)
        {
            GameObject room = Instantiate(spawnRooms[randomRoom], transform.position, transform.rotation);
            getNodes(room);
            // Place the first room in the center of the level grid
            gameManager.instance.level[center.x, center.y] = room;
            roomCount++;
        }
        else
        {
            //Debug.LogWarning("No rooms to instantiate.");
        }
        yield return new WaitForSeconds(.3f);
        generating = false;

    }
    private IEnumerator Generate()
    {
        generating = true;

        if (nodes.Count > 0)
        {
            currentNode = nodes[0];
            nodes.RemoveAt(0);
        }

        CalculateIndex(currentNode);

        roomCheck();
        if(qualifiedRooms.Count <= 0)
        {
            //Debug.Log(" no qualified rooms");
        }

        if (qualifiedRooms.Count > 0)
        {
            GameObject room = Instantiate(qualifiedRooms[UnityEngine.Random.Range(0, qualifiedRooms.Count)], currentNode.transform.position, transform.rotation);
            getNodes(room);

            gameManager.instance.level[index.x, index.y] = room;
            roomCount++;
        }
        else
        {
           // Debug.Log("No qualified rooms to instantiate.");
        }

        yield return new WaitForSeconds(.3f);
        generating = false;
    }


    private void roomCheck()
    {
        int width = gameManager.instance.level.GetLength(0);
        int height = gameManager.instance.level.GetLength(1);

        Vector2Int currentPosition = index;
        Vector2Int direction = Vector2Int.zero;

        if (currentNode.name == nodeN) direction = Vector2Int.up;
        else if (currentNode.name == nodeE) direction = Vector2Int.right;
        else if (currentNode.name == nodeS) direction = Vector2Int.down;
        else if (currentNode.name == nodeW) direction = Vector2Int.left;

        index += direction;

        Vector2Int newPosition = currentPosition + direction;

        if (newPosition.x >= 0 && newPosition.x < width && newPosition.y >= 0 && newPosition.y < height)
        {
            List<Vector2Int> directionsToCheck = new List<Vector2Int>() { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            directionsToCheck.Remove(GetOppositeDirection(direction));

            // Create a List to store needed nodes for the new room
            List<string> neededNodes = new List<string>();

            neededNodes.Add(GetOppositeNode(currentNode.name));
            noNeighbors.Clear();
            List<string> neighborsWithoutNode = new List<string>();

            foreach (Vector2Int dir in directionsToCheck)
            {
                Vector2Int neighborPosition = newPosition + dir;

                if (neighborPosition.x >= 0 && neighborPosition.x < width && neighborPosition.y >= 0 && neighborPosition.y < height)
                {
                    neighbor = gameManager.instance.level[neighborPosition.x, neighborPosition.y];

                    if (neighbor != null)
                    {
                        getNodesInNeighbor(neighbor);

                        // Create a flag to track whether the neighbor has a corresponding node
                        bool neighborHasNode = false;

                        foreach (GameObject node in neighborNodes)
                        {
                            if (dir == Vector2Int.up && node.name == nodeS)
                            {
                                neededNodes.Add(GetOppositeNode(node.name));
                                neighborHasNode = true;
                            }
                            else if (dir == Vector2Int.right && node.name == nodeW)
                            {
                                neededNodes.Add(GetOppositeNode(node.name));
                                neighborHasNode = true;
                            }
                            else if (dir == Vector2Int.down && node.name == nodeN)
                            {
                                neededNodes.Add(GetOppositeNode(node.name));
                                neighborHasNode = true;
                            }
                            else if (dir == Vector2Int.left && node.name == nodeE)
                            {
                                neededNodes.Add(GetOppositeNode(node.name));
                                neighborHasNode = true;
                            }
                        }

                        // If the neighbor exists but has no corresponding node, add the node facing it to the list
                        if (!neighborHasNode)
                        {
                            if (dir == Vector2Int.up)
                            {
                                neighborsWithoutNode.Add(nodeN);
                            }
                            else if (dir == Vector2Int.right)
                            {
                                neighborsWithoutNode.Add(nodeE);
                            }
                            else if (dir == Vector2Int.down)
                            {
                                neighborsWithoutNode.Add(nodeS);
                            }
                            else if (dir == Vector2Int.left)
                            {
                                neighborsWithoutNode.Add(nodeW);
                            }
                        }
                    }
                    if (neighbor == null)
                    {
                        if (dir == Vector2Int.up)
                        {
                            noNeighbors.Add(nodeN);
                        }
                        else if (dir == Vector2Int.right)
                        {
                            noNeighbors.Add(nodeE);
                        }
                        else if (dir == Vector2Int.down)
                        {
                            noNeighbors.Add(nodeS);
                        }
                        else if (dir == Vector2Int.left)
                        {
                            noNeighbors.Add(nodeW);
                        }
                        else
                        {
                            continue;
                        }                       
                    }
                }
            }

            // Clear qualifiedRooms to prepare for new calculation
            qualifiedRooms.Clear();
            bool roomQualifies = false;

            if(roomCount == 1)
            {
                qualifiedRooms = bobShop;
            }
            else if (roomCount == 2)
            {
                foreach (GameObject room in gameManager.instance.BossRooms)
                {
                    // Check if the new position is empty in the level array
                    if (gameManager.instance.level[newPosition.x, newPosition.y] == null)
                    {
                        List<string> nodesInRoom = new List<string>();
                        List<bool> qualifies = new List<bool>();

                        foreach (Transform child in room.transform)
                        {

                            if (child.name == nodeN || child.name == nodeE || child.name == nodeS || child.name == nodeW)
                            {
                                nodesInRoom.Add(child.name);
                            }
                        }
                        for (int i = 0; i < neededNodes.Count; i++)
                        {
                            roomQualifies = nodesInRoom.Contains(neededNodes[i]);
                            qualifies.Add(roomQualifies);
                        }

                        roomQualifies = qualifies.All(q => q);

                        foreach (string nodeInRoom in nodesInRoom)
                        {
                            if (neighborsWithoutNode.Contains(nodeInRoom))
                            {
                                roomQualifies = false;
                                break;
                            }
                        }
                        // Create copies and sort them
                        List<string> sortedNeededNodes = new List<string>(neededNodes);
                        sortedNeededNodes.Sort();
                        List<string> sortedNodesInRoom = new List<string>(nodesInRoom);
                        sortedNodesInRoom.Sort();

                        // Compare sorted lists
                        roomQualifies = sortedNeededNodes.SequenceEqual(sortedNodesInRoom);
                    }
                    if (roomQualifies)
                    {
                        qualifiedRooms.Add(room);
                    }
                }
            }
            else if (roomCount < maxRooms)
            {

                foreach (GameObject room in gameManager.instance.rooms)
                {
                    // Check if the new position is empty in the level array
                    if (gameManager.instance.level[newPosition.x, newPosition.y] == null)
                    {
                        List<string> nodesInRoom = new List<string>();

                        foreach (Transform child in room.transform)
                        {
                            if (child.name == nodeN || child.name == nodeE || child.name == nodeS || child.name == nodeW)
                            {
                                nodesInRoom.Add(child.name);
                            }
                        }

                        // The room qualifies if all needed nodes are present in the room
                        roomQualifies = neededNodes.All(node => nodesInRoom.Contains(node));

                        // The room does not qualify if it has a node where a neighbor exists but has no node facing the room
                        roomQualifies &= !neighborsWithoutNode.Any(node => nodesInRoom.Contains(node));
                    }

                    if (roomQualifies)
                    {
                        qualifiedRooms.Add(room);
                    }
                }

            }
            else
            {
                foreach (GameObject room in gameManager.instance.capRooms)
                {
                    // Check if the new position is empty in the level array
                    if (gameManager.instance.level[newPosition.x, newPosition.y] == null)
                    {
                        List<string> nodesInRoom = new List<String>();
                        List<bool> qualifies = new List<bool>();

                        foreach (Transform child in room.transform)
                        {

                            if (child.name == nodeN || child.name == nodeE || child.name == nodeS || child.name == nodeW)
                            {
                                nodesInRoom.Add(child.name);
                            }
                        }
                        for (int i = 0; i < neededNodes.Count; i++)
                        {
                            roomQualifies = nodesInRoom.Contains(neededNodes[i]);
                            qualifies.Add(roomQualifies);
                        }

                        roomQualifies = qualifies.All(q => q);

                        foreach (string nodeInRoom in nodesInRoom)
                        {
                            if (neighborsWithoutNode.Contains(nodeInRoom))
                            {
                                roomQualifies = false;
                                break;
                            }
                        }
                        // Create copies and sort them
                        List<string> sortedNeededNodes = new List<string>(neededNodes);
                        sortedNeededNodes.Sort();
                        List<string> sortedNodesInRoom = new List<string>(nodesInRoom);
                        sortedNodesInRoom.Sort();

                        // Compare sorted lists
                        roomQualifies = sortedNeededNodes.SequenceEqual(sortedNodesInRoom);
                    }
                    if (roomQualifies)
                    {
                        qualifiedRooms.Add(room);
                    }
                }
            }         
        }
    }

    private void CalculateIndex(GameObject child)
    {
        // Get the parent of the child
        GameObject parent = child.transform.parent.gameObject;

        // Look for the parent in the level array
        for (int i = 0; i < gameManager.instance.level.GetLength(0); i++)
        {
            for (int j = 0; j < gameManager.instance.level.GetLength(1); j++)
            {
                if (gameManager.instance.level[i, j] == parent)
                {
                    index = new Vector2Int(i, j);
                }
            }
        }
    }


    private Vector2Int GetOppositeDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Vector2Int.down;
        else if (direction == Vector2Int.right) return Vector2Int.left;
        else if (direction == Vector2Int.down) return Vector2Int.up;
        else if (direction == Vector2Int.left) return Vector2Int.right;
        else return Vector2Int.zero;

    }

    private string GetOppositeNode(string node)
    {
        if (node == nodeN) return nodeS;
        else if (node == nodeE) return nodeW;
        else if (node == nodeS) return nodeN;
        else if (node == nodeW) return nodeE;
        else return null;
    }


    private void getNodesInNeighbor(GameObject neighbor)
    {
        neighborNodes.Clear();

        // Grab nodes that we need to generate a room for
        foreach (Transform child in neighbor.transform)
        {
            if (child.name == nodeN)
            {
                neighborNodes.Add(child.gameObject);
            }
            if (child.name == nodeE)
            {
                neighborNodes.Add(child.gameObject);
            }
            if (child.name == nodeS)
            {
                neighborNodes.Add(child.gameObject);
            }
            if (child.name == nodeW)
            {
                neighborNodes.Add(child.gameObject);
            }
            else
            {
                continue;
            }
        }
    }

    private void getNodes(GameObject current)
    {
        // Grab nodes that we need to generate a room for
        foreach (Transform child in current.transform)
        {
            if (child.name == nodeN)
            {
                nodes.Add(child.gameObject);
            }
            if (child.name == nodeE)
            {
                nodes.Add(child.gameObject);
            }
            if (child.name == nodeS)
            {
                nodes.Add(child.gameObject);
            }
            if (child.name == nodeW)
            {
                nodes.Add(child.gameObject);
            }
            else
            {
                continue;
            }
        }
    }
}