using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class Utils
{
    public static IEnumerable<(T, T)> Pairs<T>(this IEnumerable<T> it)
    {
        var enumerable = it.ToList();
        for (var i = 0; i < enumerable.Count(); i++)
        for (var j = i + 1; j < enumerable.Count(); j++)
            yield return (enumerable.ElementAt(i), enumerable.ElementAt(j));
    }
}

public class MapGen : MonoBehaviour
{
    public bool useSeed;
    public int seed;
    public Vector2Int mapSize = new(256, 256);
    public int reprRezolutionMultiplyer = 1;
    public Image image;
    public MapArea.DiagSplitPosition diagSplitPosition;
    public float contestedAreaScale = 0.2f;
    public Texture2D Output;
    public int MinFrontierNodes;
    public int MaxFrontierNodes;
    public int MaxNonFrontierNodes;
    public int MinNonFrontierNodes;


    public int MinFrontierSpacing;
    public int MaxFrontierSpacing;

    public float StepTimeSeconds;
    public MapElement[,] Map;
    public Random Random;


    public bool StopPreview;
    public float PathMinWidth;
    public int MaxHeight = 4;
    public int MaxRampCountPerEdge = 1;
    public List<Graph.Edge> Edges;
    public List<Graph.Node> Nodes;
    public Dictionary<Graph.Node, HashSet<Graph.Node>> FinalConnections;

    private bool Proceed
    {
        get => true;
        set { }
    }

    private WaitForSecondsRealtime Step => new(StepTimeSeconds);


    private void Start()
    {
        StartCoroutine(Generate());
        // renderer = GetComponent<Renderer>();
        //     StartCoroutine(GenerateUntilSuccess());
    }

    public IEnumerator Generate()
    {
        InitRandom();
        InitOutputTexture();

        // generate areas
        var (player1Area, player2Area, contestedArea) = MapArea.DiagonalSplit(
            diagSplitPosition,
            (int)(mapSize.x * contestedAreaScale),
            Random,
            mapSize);


        Nodes = new List<Graph.Node>();

        Map = InitGridMap();

        var frontierSize = Random.Next(MinFrontierNodes, MaxFrontierNodes + 1); // +1 due to exclusive

        for (var i = 0; i < frontierSize; i++)
        {
            if (AddFrontierNode(player1Area)) yield break;

            RenderMap();
            yield return Step;
        }

        var frontierP1 = new List<Graph.Node>(Nodes);

        var nonFrontierNodes = Random.Next(MinNonFrontierNodes, MaxNonFrontierNodes + 1);

        var maxAttempts = 100;
        var attempts = 0;

        for (var i = 0; i < nonFrontierNodes && attempts < maxAttempts; i++)
        {
            var collides = AddNonFrontierNode(player1Area, Nodes);

            if (!collides)
            {
                RenderMap();
                yield return Step;
                attempts = 0;
            }
            else
            {
                i--;
                attempts++;
            }
        }

        if (attempts >= maxAttempts)
        {
            yield break;
        }

        Edges = new List<Graph.Edge>();
        var possibleConnections = new Dictionary<Graph.Node, List<Graph.Node>>();
        var edgeConflicts = new Dictionary<Graph.Edge, HashSet<Graph.Edge>>();

        AddPotentialEdges(Nodes, edgeConflicts, possibleConnections, Edges);

        {
            RenderMap();
            yield return Step;
        }
        
        if (CheckIsolatedNodes(possibleConnections)) yield break;

        var ss = ChooseEdgeSubset(maxAttempts, edgeConflicts);
        do
        {
            yield return ss.Current;
        } while (ss.MoveNext());


        {
            RenderMap();
            yield return Step;
        }
        
        
        FinalConnections = MarkFinalEdges();
        
        var main = ComputeDistancesToFrontierAndChooseMain();
        
        var heightMap = new int[mapSize.x, mapSize.y];

        for (var x = 0; x < mapSize.x; x++)
        for (var y = 0; y < mapSize.y; y++)
            heightMap[x, y] = -1;
        
        main.Height = MaxHeight;
        if (!Visit(main, new HashSet<Graph.Node>())) yield break;
        
        
        var newNodes = new List<Graph.Node>();
        var correspondingNodes = new Dictionary<Graph.Node, Graph.Node>();
        
        foreach (var node in Nodes)
        {
            var newNode = new Graph.Node();

            newNode.DistanceFromFrontier = node.DistanceFromFrontier;
            newNode.IsFrontier = node.IsFrontier;
            newNode.Height = node.Height;
            newNode.IsMain = node.IsMain;
            newNode.Position = mapSize - Vector2Int.one - node.Position;

            newNodes.Add(newNode);
            correspondingNodes.Add(node, newNode);
            correspondingNodes.Add(newNode, node);
        }
        
        foreach (var newNode in newNodes)
        {
            if (!MarkNode(newNode.Position, MinFrontierSpacing / 2, newNode)) Debug.Log("problem");

            ;
            FinalConnections[newNode] = LinqUtility.ToHashSet(FinalConnections[correspondingNodes[newNode]]
                .Select(o => correspondingNodes[o]));

            foreach (var connectedNode in FinalConnections[newNode])
            foreach (var position in CapsulePositions(newNode.Position, connectedNode.Position, PathMinWidth,
                         mapSize))
                if (Map[position.x, position.y].Type != MapElement.EType.Node)
                    Map[position.x, position.y] = new MapElement(connectedNode, newNode);
        }
        
        Nodes.AddRange(newNodes);
        
        
        {
            RenderMap();
            yield return Step;
        }
    }
    
    bool Visit(Graph.Node n, HashSet<Graph.Node> visited)
            {
                // if (precomputedValidHeights == null)
                // {
                //     precomputedValidHeights = ValidHeights(n, visited);
                // }


                var newVisited = new HashSet<Graph.Node>(visited);
                newVisited.Add(n);

                var unvisitedNodes = Nodes.Where(on => !newVisited.Contains(on)).ToList();

                if (unvisitedNodes.Count == 0) return true;

                // Shuffle(precomputedValidHeights);

                void Shuffle(List<int> ints)
                {
                    for (var i = 0; i < ints.Count; i++)
                    {
                        var r = Random.Next(i, ints.Count);
                        (ints[i], ints[r]) = (ints[r], ints[i]);
                    }
                }


                var validHeights = unvisitedNodes
                    .Select(otherNode => (heights: ValidHeights(otherNode, newVisited, FinalConnections), otherNode)).ToList();
                validHeights.Sort((n1, n2) => n1.heights.Count.CompareTo(n2.heights.Count));


                foreach (var (heights, otherNode) in validHeights)
                {
                    Shuffle(heights);

                    var shiftedHeights = heights.Where(h =>
                            !FinalConnections[otherNode].Where(newVisited.Contains).Select(onn => onn.Height)
                                .Contains(h))
                        .ToList();

                    var nonShifedHeights = heights.Except(shiftedHeights).ToList();


                    foreach (var h in shiftedHeights)
                    {
                        otherNode.Height = h;
                        if (Visit(otherNode, newVisited)) return true;
                    }

                    foreach (var h in nonShifedHeights)
                    {
                        otherNode.Height = h;
                        if (Visit(otherNode, newVisited)) return true;
                    }
                }

                return false;
            }

    List<int> ValidHeights(Graph.Node n, HashSet<Graph.Node> visited, Dictionary<Graph.Node, HashSet<Graph.Node>> finalConnections)
    {
        var neighs = finalConnections[n].Where(visited.Contains).ToList();

        List<int> l = new();
        for (var i = 0; i <= MaxHeight; i++)
        {
            var ok = true;
            foreach (var neigh in neighs)
                if (Math.Abs(neigh.Height - i) > MaxRampCountPerEdge)
                {
                    ok = false;
                    break;
                }

            if (ok) l.Add(i);
        }

        return l;
    }
    
    private Graph.Node ComputeDistancesToFrontierAndChooseMain()
    {
        Graph.Node main;

        {
            Queue<Graph.Node> queue = new(Nodes.FindAll(n => n.IsFrontier));

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                foreach (var other in FinalConnections[node].Where(other => !other.DistanceFromFrontier.HasValue))
                {
                    other.DistanceFromFrontier = node.DistanceFromFrontier + 1;
                    queue.Enqueue(other);
                }
            }

            var mainCandidates = Nodes.FindAll(nd =>
                nd.DistanceFromFrontier == Nodes.Max(n => n.DistanceFromFrontier.Value));

            main = mainCandidates[Random.Next(mainCandidates.Count)];
            main.IsMain = true;
        }
        return main;
    }

    private Dictionary<Graph.Node, HashSet<Graph.Node>> MarkFinalEdges()
    {
        // init connection dict
        Dictionary<Graph.Node, HashSet<Graph.Node>> finalConnections = new();

        foreach (var node in Nodes) finalConnections[node] = new HashSet<Graph.Node>();

        // mark final edges
        foreach (var e in Edges)
        {
            finalConnections[e.Start].Add(e.End);
            finalConnections[e.End].Add(e.Start);
            foreach (var position in CapsulePositions(e.Start.Position, e.End.Position, PathMinWidth, mapSize))
                if (Map[position.x, position.y].Type != MapElement.EType.Node)
                    Map[position.x, position.y] = new MapElement(e);
        }
        return finalConnections; 
    }

    private IEnumerator ChooseEdgeSubset(int maxAttempts, Dictionary<Graph.Edge, HashSet<Graph.Edge>> edgeConflicts)
    {
        int attempts;
        bool allNodesConnected;

        attempts = 0;
        
        do
        {
            if (attempts++ >= maxAttempts) yield break; // yield break;


            var availableEdges = new List<Graph.Edge>(Edges);

            var chosenEdges = new List<Graph.Edge>();

            while (availableEdges.Count > 0)
            {
                var chosenIdx = Random.Next(availableEdges.Count);
                var chosenEdge = availableEdges[chosenIdx];

                var conflicts = edgeConflicts[chosenEdge];
                availableEdges.RemoveAt(chosenIdx);
                availableEdges.RemoveAll(e => conflicts.Contains(e));

                chosenEdges.Add(chosenEdge);
            }

            Debug.Log(chosenEdges.Count);

            HashSet<Graph.Node> reachableNodes = new();
            Queue<Graph.Node> front = new();

            reachableNodes.Add(Nodes[0]);
            front.Enqueue(Nodes[0]);

            while (front.Count > 0)
            {
                var node = front.Dequeue();

                foreach (var edge in chosenEdges)
                    if (edge.Contains(node, out var other))
                        if (reachableNodes.Add(other))
                            front.Enqueue(other);
            }

            Debug.Log("AAA");

            allNodesConnected = reachableNodes.Count == Nodes.Count;

            if (!allNodesConnected)
            {
                var mapCopy = Map.Clone() as MapElement[,];

                Debug.Log(chosenEdges.Count);
                foreach (var e in chosenEdges)
                foreach (var position in CapsulePositions(e.Start.Position, e.End.Position, PathMinWidth,
                             mapSize))
                    if (Map[position.x, position.y].Type != MapElement.EType.Node)
                        Map[position.x, position.y] = new MapElement(e);

                Debug.Log("BBB");

                RenderMap();
                yield return Step;
                Map = mapCopy;

                Debug.Log("Fail");
            }
            else
            {
                Debug.Log("ok");
                Edges = chosenEdges;
            }
        } while (!allNodesConnected);

        foreach (var e in Edges)
        foreach (var position in CapsulePositions(e.Start.Position, e.End.Position, PathMinWidth,
                     mapSize))
            if (Map[position.x, position.y].Type != MapElement.EType.Node)
                Map[position.x, position.y] = new MapElement(e);
    }

    private static bool CheckIsolatedNodes(Dictionary<Graph.Node, List<Graph.Node>> possibleConnections)
    {
        if (possibleConnections.Any(pair => pair.Value.Count == 0))
        {
            Debug.Log("isolated node");
            return true;
        }

        return false;
    }

    private void AddPotentialEdges(List<Graph.Node> nodes, Dictionary<Graph.Edge, HashSet<Graph.Edge>> edgeConflicts, Dictionary<Graph.Node, List<Graph.Node>> possibleConnections, List<Graph.Edge> edges)
    {
        foreach (var (node1, node2) in nodes.Pairs())
        {
            var canConnect = true;

            var edge = new Graph.Edge(node1, node2);

            var conflicts = new HashSet<Graph.Edge>();

            foreach (var elPos in CapsulePositions(node1.Position, node2.Position, PathMinWidth, mapSize))
            {
                var el = Map[elPos.x, elPos.y];

                canConnect &= el.Type != MapElement.EType.Node || el.Node == node1 || el.Node == node2;

                if (!canConnect) break;

                if (el.Type == MapElement.EType.PossibleEdgeList) conflicts.AddRange(el.Edges);
            }

            if (canConnect)
            {
                conflicts.RemoveWhere(e => e.Contains(node1) || e.Contains(node2));

                edgeConflicts[edge] = conflicts;

                foreach (var e in conflicts) edgeConflicts[e].Add(edge);

                if (!possibleConnections.ContainsKey(node1))
                    possibleConnections.Add(node1, new List<Graph.Node>());
                possibleConnections[node1].Add(node2);

                if (!possibleConnections.ContainsKey(node2))
                    possibleConnections.Add(node2, new List<Graph.Node>());
                possibleConnections[node2].Add(node1);

                edges.Add(edge);

                foreach (var elPos in CapsulePositions(node1.Position, node2.Position, PathMinWidth, mapSize))
                {
                    var el = Map[elPos.x, elPos.y];

                    if (el.Type == MapElement.EType.PossibleEdgeList)
                    {
                        el.Edges.Add(edge);
                    }
                    else if (el.Type != MapElement.EType.Node)
                    {
                        el.Type = MapElement.EType.PossibleEdgeList;
                        el.Edges = new List<Graph.Edge>();
                        el.Edges.Add(edge);
                    }
                }
            }
        }
    }

    // check if p is inside the 2d capsule defined by the centers v1 and v2 and radius r
    // i.e. p is inside the area sweeped by a disc of radius r moving in a straight line from v1 to v2
    private static bool InCapsule(Vector2 p, Vector2 v1, Vector2 v2, float radius)
    {
        if (Vector2.Distance(p, v1) < radius || Vector2.Distance(p, v2) < radius) return true;

        var dir = (v2 - v1).normalized;

        var dAlongDir = Vector2.Dot(p - v1, dir);

        if (dAlongDir < 0 || dAlongDir > (v2 - v1).magnitude) return false;

        var projection = v1 + dAlongDir * dir;

        return Vector2.Distance(p, projection) < radius;
    }
    
    private static IEnumerable<Vector2Int> CapsulePositions(Vector2Int v1, Vector2Int v2, float radius,
        Vector2Int mapBounds)
    {
        var minx = (int)Math.Floor(Math.Min(v1.x, v2.x) - radius);
        var miny = (int)Math.Floor(Math.Min(v1.y, v2.y) - radius);
        var maxx = (int)Math.Ceiling(Math.Max(v1.x, v2.x) + radius);
        var maxy = (int)Math.Ceiling(Math.Max(v1.y, v2.y) + radius);


        minx = Math.Max(minx, 0);
        miny = Math.Max(miny, 0);
        maxx = Math.Min(maxx, mapBounds.x - 1);
        maxy = Math.Min(maxy, mapBounds.y - 1);

        for (var x = minx; x <= maxx; x++)
        for (var y = miny; y <= maxy; y++)
        {
            var p = new Vector2Int(x, y);
            if (InCapsule(p, v1, v2, radius))
                yield return new Vector2Int(x, y);
        }
    }

    private bool AddNonFrontierNode(MapArea player1Area, List<Graph.Node> nodes)
    {
        // int posx, posy;

        Vector2Int pos;

        var collides = false;

        var n = new Graph.Node();

        pos = player1Area.Sample();

        foreach (var node in nodes)
            if (Vector2.Distance(pos, node.Position) <= MinFrontierSpacing)
            {
                collides = true;
                break;
            }

        if (!collides)
        {
            n.Position = pos;

            collides = !MarkNode(n.Position, MinFrontierSpacing / 2, n);
        }

        if (!collides) nodes.Add(n);

        return collides;
    }


    private bool AddFrontierNode(MapArea player1Area)
    {
        Vector2Int pos;

        var ok = true;

        var attempts = 10000;

        var n = new Graph.Node(true);

        do
        {
            ok = true;

            if (attempts-- <= 0)
            {
                RenderMap();
                return true;
            }

            // pos = random.Next(MapSize.x);

            pos = player1Area.SampleFrontier();
            foreach (var node in Nodes)
                if (Vector2.Distance(pos, node.Position) <= MinFrontierSpacing)
                {
                    ok = false;
                    break;
                }

            if (ok)
            {
                n.Position = pos;

                ok = MarkNode(n.Position, MinFrontierSpacing / 2, n);
            }
        } while (!ok);


        Nodes.Add(n);
        return false;
    }

    private void RenderMap()
    {
        for (var i = 0; i < mapSize.x; i++)
        for (var j = 0; j < mapSize.y; j++)
            Output.SetPixel(i, j, Map[i, j].Color);

        Output.Apply();
        image.sprite = Sprite.Create(Output, Rect.MinMaxRect(0, 0, mapSize.x, mapSize.y), Vector2.zero);
    }

    // mark all tiles within the node's radius on the map
    private bool MarkNode(Vector2Int position, float radius, Graph.Node node)
    {
        var cpy = Map.Clone() as MapElement[,];


        for (var i = Math.Max((int)Math.Floor(position.x - radius), 0);
             i < mapSize.x && i <= Math.Ceiling(position.x + radius);
             i++)
        for (var j = Math.Max((int)Math.Floor(position.y - radius), 0);
             j < mapSize.y && j <= Math.Ceiling(position.y + radius);
             j++)
            if (Vector2.Distance(new Vector2Int(i, j), position) <= radius)
            {
                if (Map[i, j] == null || Map[i, j].Type != MapElement.EType.Node)
                {
                    Map[i, j] = new MapElement(node);
                }
                else
                {
                    Map = cpy;
                    return false;
                }
            }

        return true;
    }

    private MapElement[,] InitGridMap()
    {
        var map = new MapElement[mapSize.x, mapSize.y];

        for (var x = 0; x < mapSize.x; x++)
        for (var y = 0; y < mapSize.y; y++)
            map[x, y] = new MapElement();
        return map;
    }

    private void InitOutputTexture()
    {
        Output = new Texture2D(mapSize.x * reprRezolutionMultiplyer, mapSize.y * reprRezolutionMultiplyer,
            TextureFormat.RGBA32, false);
        Output.filterMode = FilterMode.Point;
        image.sprite = Sprite.Create(Output, Rect.zero, Vector2.zero);
    }

    private void InitRandom()
    {
        if (useSeed)
            Random = new Random(seed);
        else
            Random = new Random();
    }

    public class MapElement
    {
        public enum EType
        {
            Node,
            Edge,
            Empty,
            PossibleEdgeList,
            Ramp
        }

        public readonly Graph.Edge Edge;
        [CanBeNull] public readonly Graph.Node Node;
        [CanBeNull] public List<Graph.Edge> Edges;

        public float Height = 0;

        public EType Type;

        public MapElement(Graph.Node node)
        {
            Node = node;
            Type = EType.Node;
            Edge = null;
        }

        public MapElement(Graph.Node node1, Graph.Node node2)
        {
            Edge = new Graph.Edge(node1, node2);
            Type = EType.Edge;
            Node = null;
        }

        public MapElement(Graph.Edge edge)
        {
            Edge = edge;
            Type = EType.Edge;
            Node = null;
        }

        public MapElement()
        {
            Type = EType.Empty;
            Node = null;
            Edge = null;
        }


        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case EType.Node:
                        if (Node.IsFrontier) return Color.red;

                        if (Node.IsMain) return Color.green;

                        return Color.blue;
                    case EType.Edge:
                        return Color.magenta;
                    case EType.Empty:
                        return Color.black;
                    case EType.PossibleEdgeList:
                        return new Color(1.0f, 1.0f / Edges.Count, 0);
                    case EType.Ramp:
                        return Color.cyan;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class Graph
    {
        public class Node
        {
            private Vector2Int _position;
            public int? DistanceFromFrontier;
            public int Height;
            public bool IsFrontier;
            public bool IsMain;
            public Node Parent = null;

            public Node(bool isFrontier = false)
            {
                IsFrontier = isFrontier;
                if (isFrontier) DistanceFromFrontier = 0;
            }

            public bool Placed { get; private set; }

            public Vector2Int Position
            {
                get => _position;
                set
                {
                    Placed = true;
                    _position = value;
                }
            }
        }

        public class Edge
        {
            public readonly Node End;
            public readonly Node Start;

            public Edge(Node start, Node end)
            {
                Start = start;
                End = end;
            }

            public bool Contains(Node node1)
            {
                return End == node1 || Start == node1;
            }
            
            public bool Contains(Node node1, out Node other)
            {
                if (End == node1)
                {
                    other = Start;
                    return true;
                }

                if (Start == node1)
                {
                    other = End;
                    return true;
                }

                other = null;
                return false;
            }
        }
    }

    public class MapArea
    {
        public enum DiagSplitPosition
        {
            TopLeft = 0b00,
            TopRight = 0b01,
            BottomLeft = 0b10,
            BottomRight = 0b11
        }

        public Func<Vector2Int> Sample; // function returning a random point in the area
        public Func<Vector2Int> SampleFrontier; // function returning a random point on the frontier of the area


        // TODO : add more splitting strategies
        public static (MapArea, MapArea, MapArea) DiagonalSplit(DiagSplitPosition player1Pos,
            int contestedAreaWidth, Random r, Vector2Int mapSize)
        {
            // TODO :  Expand to rectangular (non-square) maps
            Assert.AreEqual(mapSize.x, mapSize.y);

            var p1 = new MapArea();
            var p2 = new MapArea();
            var cont = new MapArea();

            Vector2Int dp1;
            Vector2Int dq1;
            Vector2Int orig1;
            Vector2Int dp2;
            Vector2Int dq2;
            Vector2Int orig2;

            switch (player1Pos)
            {
                case DiagSplitPosition.TopLeft:
                    dp1 = Vector2Int.down * (mapSize.y - contestedAreaWidth - 1);
                    dq1 = Vector2Int.right * (mapSize.x - contestedAreaWidth - 1);
                    orig1 = new Vector2Int(0, mapSize.y - 1);
                    dp2 = Vector2Int.up * (mapSize.y - contestedAreaWidth - 1);
                    dq2 = Vector2Int.left * (mapSize.x - contestedAreaWidth - 1);
                    orig2 = new Vector2Int(mapSize.x - 1, 0);
                    break;
                case DiagSplitPosition.TopRight:
                    dp1 = Vector2Int.down * (mapSize.y - contestedAreaWidth - 1);
                    dq1 = Vector2Int.left * (mapSize.x - contestedAreaWidth - 1);
                    orig1 = mapSize - Vector2Int.one;
                    dp2 = Vector2Int.up * (mapSize.y - contestedAreaWidth - 1);
                    dq2 = Vector2Int.right * (mapSize.x - contestedAreaWidth - 1);
                    orig2 = Vector2Int.zero;
                    break;
                case DiagSplitPosition.BottomLeft:
                    dp1 = Vector2Int.up * (mapSize.y - contestedAreaWidth - 1);
                    dq1 = Vector2Int.right * (mapSize.x - contestedAreaWidth - 1);
                    orig1 = Vector2Int.zero;
                    dp2 = Vector2Int.down * (mapSize.y - contestedAreaWidth - 1);
                    dq2 = Vector2Int.left * (mapSize.x - contestedAreaWidth - 1);
                    orig2 = mapSize - Vector2Int.one;
                    break;
                case DiagSplitPosition.BottomRight:
                    dp1 = Vector2Int.up * (mapSize.y - contestedAreaWidth - 1);
                    dq1 = Vector2Int.left * (mapSize.x - contestedAreaWidth - 1);
                    orig1 = new Vector2Int(mapSize.x - 1, 0);
                    dp2 = Vector2Int.down * (mapSize.y - contestedAreaWidth - 1);
                    dq2 = Vector2Int.right * (mapSize.x - contestedAreaWidth - 1);
                    orig2 = new Vector2Int(0, mapSize.y - 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(player1Pos), player1Pos, null);
            }

            p1.Sample = () =>
            {
                var p = (float)r.NextDouble();
                var q = (float)r.NextDouble();

                if (p + q > 1)
                {
                    p = 1 - p;
                    q = 1 - q;
                }

                return Vector2Int.RoundToInt(orig1 + (Vector2)dp1 * p + (Vector2)dq1 * q);
            };

            p2.Sample = () =>
            {
                var p = (float)r.NextDouble();
                var q = (float)r.NextDouble();

                if (p + q > 1)
                {
                    p = 1 - p;
                    q = 1 - q;
                }


                return Vector2Int.RoundToInt(orig2 + (Vector2)dp2 * p + (Vector2)dq2 * q);
            };

            Vector2Int sfm1;
            Vector2Int sfo1;
            Vector2Int sfm2;
            Vector2Int sfo2;

            switch (player1Pos)
            {
                case DiagSplitPosition.TopLeft:
                    sfm1 = Vector2Int.one;
                    sfo1 = new Vector2Int(0, contestedAreaWidth);
                    sfm2 = Vector2Int.one;
                    sfo2 = new Vector2Int(contestedAreaWidth, 0);
                    break;
                case DiagSplitPosition.TopRight:
                    sfm1 = new Vector2Int(1, -1);
                    sfo1 = new Vector2Int(contestedAreaWidth, mapSize.y - 1);
                    sfm2 = new Vector2Int(1, -1);
                    sfo2 = new Vector2Int(0, mapSize.y - contestedAreaWidth - 1);
                    break;
                case DiagSplitPosition.BottomLeft:
                    sfm1 = new Vector2Int(1, -1);
                    sfo1 = new Vector2Int(0, mapSize.y - contestedAreaWidth - 1);
                    sfm2 = new Vector2Int(1, -1);
                    sfo2 = new Vector2Int(contestedAreaWidth, mapSize.y - 1);
                    break;
                case DiagSplitPosition.BottomRight:
                    sfm1 = Vector2Int.one;
                    sfo1 = new Vector2Int(contestedAreaWidth, 0);
                    sfm2 = Vector2Int.one;
                    sfo2 = new Vector2Int(0, contestedAreaWidth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(player1Pos), player1Pos, null);
            }

            p1.SampleFrontier = () =>
            {
                var p = r.Next(mapSize.x - contestedAreaWidth);

                return p * sfm1 + sfo1;
            };

            p2.SampleFrontier = () =>
            {
                var p = r.Next(mapSize.x - contestedAreaWidth);

                return p * sfm2 + sfo2;
            };

            cont.Sample = () =>
            {
                var p = r.Next(-contestedAreaWidth / 2, contestedAreaWidth / 2 + 1);
                var q = r.Next(mapSize.x - contestedAreaWidth / 2 + 1);

                var pos = p * new Vector2Int(1, -1) + q * new Vector2Int(1, 1);

                if (pos.x < 0)
                {
                    pos.x += mapSize.x;
                    pos.y = mapSize.y - pos.y;
                }
                else if (pos.y < 0)
                {
                    pos.y += mapSize.y;
                    pos.x = mapSize.x - pos.x;
                }

                switch (player1Pos)
                {
                    case DiagSplitPosition.TopLeft:
                    case DiagSplitPosition.BottomRight: return pos;
                    default:
                        return new Vector2Int(mapSize.x - pos.x - 1, pos.y);
                }
            };


            return (p1, p2, cont);
        }
    }
}