﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This class implements the levels of the expression graph.
    /// </summary>
    public sealed class ExpressionLevel : IExpressionLevel
    {
        #region Properties

        /// <summary>
        /// The content of the level
        /// </summary>
        private Dictionary<LevelKey, IExpressionLevelEntry> _Content;

        public Dictionary<LevelKey, IExpressionLevelEntry> ExpressionLevels
        {
            get
            {
                lock (_Content)
                {
                    return _Content;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpressionLevel()
        {
            _Content = new Dictionary<LevelKey, IExpressionLevelEntry>();
        }

        #endregion

        #region IExpressionLevel Members

        public void AddNodeAndBackwardEdge(LevelKey myPath, Int64 aDBInt64, EdgeKey backwardDirection, Int64 backwardDestination, IComparable EdgeWeight, IComparable NodeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(aDBInt64))
                    {


                        //Node exists
                        _Content[myPath].Nodes[aDBInt64].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);


                    }
                    else
                    {

                        //Node does not exist
                        _Content[myPath].Nodes.Add(aDBInt64, new ExpressionNode(aDBInt64, NodeWeight));
                        _Content[myPath].Nodes[aDBInt64].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                    }
                }
                else
                {
                    HashSet<IExpressionEdge> backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(backwardDestination, EdgeWeight, backwardDirection) };

                    _Content.Add(myPath, new ExpressionLevelEntry(myPath));
                    _Content[myPath].Nodes.Add(aDBInt64, new ExpressionNode(aDBInt64, NodeWeight));
                    _Content[myPath].Nodes[aDBInt64].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                }

            }
        }

        public void AddNodeAndBackwardEdge(LevelKey myPath, IVertex aIVertex, EdgeKey backwardDirection, Int64 backwardDestination, IComparable EdgeWeight, IComparable NodeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(aIVertex.VertexID))
                    {


                        //Node exists
                        _Content[myPath].Nodes[aIVertex.VertexID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);


                    }
                    else
                    {

                        //Node does not exist
                        _Content[myPath].Nodes.Add(aIVertex.VertexID, new ExpressionNode(aIVertex, NodeWeight));
                        _Content[myPath].Nodes[aIVertex.VertexID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                    }
                }
                else
                {
                    HashSet<IExpressionEdge> backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(backwardDestination, EdgeWeight, backwardDirection) };


                    _Content.Add(myPath, new ExpressionLevelEntry(myPath));
                    _Content[myPath].Nodes.Add(aIVertex.VertexID, new ExpressionNode(aIVertex, NodeWeight));
                    _Content[myPath].Nodes[aIVertex.VertexID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                }

            }
        }

        public void AddBackwardEdgesToNode(LevelKey myPath, Int64 myInt64, EdgeKey backwardDestination, Dictionary<Int64, IComparable> validUUIDs)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(myInt64))
                    {

                        //Node exists
                        _Content[myPath].Nodes[myInt64].AddBackwardEdges(backwardDestination, validUUIDs);

                    }
                    else
                    {
                        //Node does not exist
                        throw new ExpressionGraphInternalException("The node does not exist in this LevelKey.");
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new ExpressionGraphInternalException("The LevelKey does not exist in this ExpressionLevel.");
                }
            }
        }

        public void AddForwardEdgeToNode(LevelKey levelKey, Int64 aInt64, EdgeKey forwardDirection, Int64 destination, IComparable edgeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    //the level exists
                    if (_Content[levelKey].Nodes.ContainsKey(aInt64))
                    {
                        //Node exists
                        _Content[levelKey].Nodes[aInt64].AddForwardEdge(forwardDirection, destination, edgeWeight);
                    }
                    else
                    {
                        //Node does not exist
                        throw new ExpressionGraphInternalException("The node does not exist in this LevelKey.");
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new ExpressionGraphInternalException("The LevelKey does not exist in this ExpressionLevel.");
                }
            }
        }

        public void AddNode(LevelKey levelKey, IExpressionNode expressionNode)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    var tempInt64 = expressionNode.GetVertexID();

                    if (_Content[levelKey].Nodes.ContainsKey(tempInt64))
                    {
                        //the node already exist, so update its edges
                        foreach (var aBackwardEdge in expressionNode.BackwardEdges)
                        {
                            _Content[levelKey].Nodes[tempInt64].AddBackwardEdges(aBackwardEdge.Value);
                        }

                        foreach (var aForwardsEdge in expressionNode.ForwardEdges)
                        {
                            _Content[levelKey].Nodes[tempInt64].AddForwardEdges(aForwardsEdge.Value);
                        }
                    }
                    else
                    {
                        _Content[levelKey].Nodes.Add(tempInt64, expressionNode);
                    }
                }
                else
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));
                    _Content[levelKey].Nodes.Add(expressionNode.GetVertexID(), expressionNode);
                }
            }
        }

        public void AddEmptyLevelKey(LevelKey levelKey)
        {
            lock (_Content)
            {
                if (!_Content.ContainsKey(levelKey))
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));
                }
            }
        }

        public IEnumerable<IExpressionNode> GetNodes(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myLevelKey))
                {
                    return _Content[myLevelKey].Nodes.Values;
                }

                return null;
            }
        }

        public Dictionary<Int64, IExpressionNode> GetNode(LevelKey myLevelKey)
        {
            lock (_Content)
            {

                if (_Content.ContainsKey(myLevelKey))
                    return _Content[myLevelKey].Nodes;

                return null;
            }
        }

        public void RemoveNode(LevelKey myLevelKey, Int64 myInt64)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myLevelKey))
                {
                    _Content[myLevelKey].Nodes.Remove(myInt64);
                }
            }
        }

        public Boolean HasLevelKey(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                return _Content.ContainsKey(myLevelKey);
            }
        }

        #endregion

        #region override

        public override string ToString()
        {
            return String.Format("Level#: {0}", ExpressionLevels.Count);
        }

        #endregion
    }
}