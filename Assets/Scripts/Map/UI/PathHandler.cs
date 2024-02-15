using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Map.Locations;
using UnityEngine;

namespace Assets.Scripts.Map.UI
{
    public class PathHandler:MonoBehaviour
    {
        private Canvas canvas;
        private int numBranches;
        private List<MapNode> mapBranchNodes=new List<MapNode>();
        private List<List<Vector2>> mapLines=new List<List<Vector2>>();
        private List<List<MapNode>> mapPaths=new List<List<MapNode>>();

        public void Init(Canvas mapCanvas,int numBranches, MapNode firstNode)
        {
            canvas = mapCanvas;
            this.numBranches = numBranches;
            firstNode.SetStatus(MapNodeStatus.Unlocked);
            List<MapNode> startingNodes=firstNode.GetNextClosestNodes(numBranches);
            for (int i = 0; i < numBranches; i++)
            {
                AddBranchingNode(startingNodes[i]);
                MapNode nextNode = startingNodes[i];
                firstNode.hasPath = true;
                MakePath(new List<MapNode>() { firstNode,nextNode});
            }

            MakeBranchingPaths();
        }

        public List<List<MapNode>> GetMapNodes()
        {
            return mapPaths.OrderBy(list=>list.Count).ToList();
        }
        
        
        #region Path Generating

        /// <summary>
        /// Make a path that uses the given path 
        /// </summary>
        /// <param name="currentPath">The path</param>
        private void MakePath(List<MapNode> currentPath)
        {
            List<MapNode> newNodes = currentPath[^1].GetNextClosestNodes(numBranches);
            if (newNodes != null)
            {
                if (newNodes.Count>0)
                {
                    MapNode nextNode = newNodes[0];
                    AddBranchingNodes(newNodes);
                
                    List<MapNode> path = currentPath;
                    path.Add(nextNode);
                    MakePath(currentPath);
                    return;
                }
            }
            AddPath(currentPath);
        }
        
        private void AddBranchingNode(MapNode branchNode)
        {
            if (branchNode==null || mapBranchNodes.Contains(branchNode)) return;
            mapBranchNodes.Add(branchNode);
        }

        private void AddBranchingNodes(List<MapNode> branchNodes)
        {
            branchNodes.Remove(branchNodes[0]);
            if (branchNodes.Count == 0) return;
            foreach (MapNode node in branchNodes)
            {
                AddBranchingNode(node);
            }
        }

        private void MakeBranchingPaths()
        {
            foreach (MapNode branchNode in mapBranchNodes)
            {
                List<MapNode> startingNodes=branchNode.GetNextClosestNodes(numBranches);
                if (startingNodes != null)
                {
                    for (int i = 0; i < startingNodes.Count-1; i++)
                    {
                        MapNode nextNode = startingNodes[i];
                        MakePath(new List<MapNode>() { branchNode,nextNode});
                    }
                }
            }
        }

        public List<Vector2> GetPositionList(List<MapNode> nodes)
        {
            List<Vector2> line = new List<Vector2>();
            foreach (MapNode node in nodes)
            {
                float x=node.transform.position.x - canvas.pixelRect.width/2;
                float y=node.transform.position.y - canvas.pixelRect.height/2;
                line.Add(new Vector2(x,y));
            }

            return line;
        }

        /// <summary>
        /// Adds the given path to mapPaths if it isn't in already
        /// </summary>
        /// <param name="path">The path being added</param>
        private void AddPath(List<MapNode> path)
        {
            if (ShouldAddPath(path) && !HasIntersection(path))
            {
                mapPaths.Add(path);
            }
        }
        #endregion
        
        #region Path Bool Checks

        /// <summary>
        /// Checks if the given path should be added to mapPaths
        /// </summary>
        /// <param name="path">The path potentially being added</param>
        /// <returns>Whether the path should be added</returns>
        private bool ShouldAddPath(List<MapNode> path)
        {
            return NoDuplicates(path) && NotPartOfPath(path) && path.Count>1;
        }

        /// <summary>
        /// Checks to see if mapPaths contains path
        /// </summary>
        /// <param name="path">The path being checked</param>
        /// <returns>True if mapPaths doesn't contain path</returns>
        private bool NoDuplicates(List<MapNode> path)
        {
            return !mapPaths.Contains(path);
        }
        
        /// <summary>
        /// Checks to see if the given path is apart of mapPaths
        /// </summary>
        /// <param name="path">The path that could be part of a path in mapPaths</param>
        /// <returns>Whether the given path is part of mapPaths</returns>
        private bool NotPartOfPath(List<MapNode> path)
        {
            foreach (List<MapNode> acceptedPaths in mapPaths)
            {
                if (PartOfPath(acceptedPaths, path))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks to see if the given smallerPath is part of the given biggerPath
        /// </summary>
        /// <param name="biggerPath">The path that could potentially contain the smallerPath</param>
        /// <param name="smallerPath">The path that could be part of the biggerPath</param>
        /// <returns>Whether the biggerPath contains the smallerPath</returns>
        private bool PartOfPath(List<MapNode> biggerPath, List<MapNode> smallerPath)
        {
            return biggerPath.Intersect(smallerPath).Count() == smallerPath.Count();
        }
        
        /// <summary>
        /// Checks to see if path crosses over any of the accepted paths
        /// </summary>
        /// <param name="path">The list of points on a path</param>
        /// <returns>Whether or not the given path overlaps with any path in mapPaths</returns>
        private bool HasIntersection(List<MapNode> path)
        {
            return HasIntersection(GetPositionList(path));
        }
        
        /// <summary>
        /// Checks to see if path crosses over any of the accepted paths
        /// </summary>
        /// <param name="path">The list of points on a path</param>
        /// <returns>Whether or not the given path overlaps with any path in mapPaths</returns>
        private bool HasIntersection(List<Vector2> path)
        {
            foreach (List<Vector2> line in mapLines)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    bool intersection = LineSegmentsIntersect(path[i-1], path[i], line[i-1], line[i]);    
                    if (!intersection)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if line intersects
        /// </summary>
        /// <param name="lineOneA">The start of line 1</param>
        /// <param name="lineOneB">The end of line 1</param>
        /// <param name="lineTwoA">The start of line 2</param>
        /// <param name="lineTwoB">The end of line 2</param>
        /// <returns>True if an intersection exists</returns>
        private static bool LineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
        {
            return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)) != ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) && ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
        }

        #endregion
    }
}