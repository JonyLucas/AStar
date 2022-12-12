using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Class that holds information about a certain position,
/// so it's used in a pathfinding algorithm.
/// </summary>
class Node
{
    // Every node may have different values, according to your game/application.
    public enum Value
    {
        Free,
        Blocked
    }

    // Nodes havve X and Y Positions
    public int posX;
    public int posY;

    // G is a basic *cost* value to go from one node to another.
    public int cost = int.MaxValue;

    // H is a heuristic that *estimates* the cost of the closest path.
    public int weight = int.MaxValue;

    // Nodes have references to other nodes so it is possible to build a path.
    public Node parent;

    public Value value;

    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        cost = int.MaxValue;
        weight = int.MaxValue;
        value = Value.Free;
    }
}

public class AStar : MonoBehaviour
{
    // Constants
    private const int MAP_SIZE = 6;

    // Variables
    private List<string> map;
    private Node[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        map= new List<string>();
        map.Add("G-----");
        map.Add("XXXXX-");
        map.Add("S-X-X-");
        map.Add("--X-X-");
        map.Add("--X-X-");
        map.Add("------");

        nodes = new Node[MAP_SIZE, MAP_SIZE];
        Node start = null;
        Node goal = null;

        // Parse the map
        for(var y = 0; y < MAP_SIZE; y++)
        {
            for(var x = 0; x < MAP_SIZE; x++)
            {
                var node = new Node(x, y);
                node.value = map[y][x] == 'X' ? Node.Value.Blocked : Node.Value.Free;

                if(map[y][x] == 'G')
                {
                    goal= node;
                }

                if (map[y][x] == 'S')
                {
                    start= node;
                }

                nodes[x,y] = node;
            }
        }

        var nodePath = ExecuteAStar(start, goal);

        foreach(var node in nodePath)
        {
            var charArray = map[node.posY].ToCharArray();
            charArray[node.posX] = '0';
            map[node.posY] = new string(charArray);
        }

        // Print the map
        var pathString = new StringBuilder();
        foreach(var line in map)
        {
            pathString.AppendLine(line);
        }
        Debug.Log(pathString.ToString());
    }

    private List<Node> ExecuteAStar(Node start, Node goal)
    {
        // This list holds potential best path nodes that should be visited. It always start with the origin.
        var openList = new List<Node>() { start };

        // This list keeps track of all the nodes that have been visited.
        var closedList = new List<Node>();

        // Initialize the start node
        start.cost = 0;
        start.weight = CalculateWeightValue(start, goal);

        // Main Algorithm
        while(openList.Count > 0)
        {
            // Get the node with the lowest estimated cost to reach the goal.
            var current = openList[0];
            foreach(var node in openList)
            {
                if(node.weight < current.weight)
                {
                    current = node;
                }
            }

            if(current == goal)
            {
                return BuildPath(current);
            }

            openList.Remove(current);
            closedList.Add(current);

            var neighbours = GetNeighbourNodes(current);
            foreach(var node in neighbours)
            {
                if (closedList.Contains(node))
                {
                    continue;
                }

                if(!openList.Contains(node))
                {
                    openList.Add(node);
                }

                // Calculates the G value of the node.
                if(node.cost > current.cost + 1)
                {
                    node.parent = current;
                    node.cost = current.cost + 1;
                    node.weight = node.cost + CalculateWeightValue(node, goal); // f = g + h
                }
            }
        }

        // Fail Condition
        return new List<Node>();
    }

    /// <summary>
    /// A simple estimation of the distance between nodes. Utilizing the Mahhattan distance.
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private int CalculateWeightValue(Node n1, Node n2)
    {
        return Mathf.Abs(n1.posX- n2.posX) + Mathf.Abs(n1.posY - n2.posY);
    }

    /// <summary>
    /// Check for all valid neighbours, considerating only vertical and horizontal movement.
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    private List<Node> GetNeighbourNodes(Node current)
    {
        var neighbours = new List<Node>();

        if(current.posX > 0)
        {
            var candidate = nodes[current.posX-1, current.posY];
            if(candidate.value != Node.Value.Blocked)
            {
                neighbours.Add(candidate);
            }
        }

        if(current.posX < MAP_SIZE - 1)
        {
            var candidate = nodes[current.posX + 1, current.posY];
            if (candidate.value != Node.Value.Blocked)
            {
                neighbours.Add(candidate);
            }
        }

        if(current.posY > 0)
        {
            var candidate = nodes[current.posX, current.posY - 1];
            if (candidate.value != Node.Value.Blocked)
            {
                neighbours.Add(candidate);
            }
        }

        if(current.posY < MAP_SIZE - 1)
        {
            var candidate = nodes[current.posX, current.posY + 1];
            if (candidate.value != Node.Value.Blocked)
            {
                neighbours.Add(candidate);
            }
        }

        return neighbours;
    }

    private List<Node> BuildPath(Node node)
    {
        var path = new List<Node>() { node };

        while(node.parent!= null)
        {
            path.Add(node.parent);
            node = node.parent;
        }

        return path;
    }
}
