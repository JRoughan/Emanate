using System.Xml;
using System.Xml.Linq;

namespace Emanate.UnitTests.Core.Configuration
{
    /// <summary>
    /// Compares two xml files for equality of content in any order.
    /// </summary>
    public class XmlComparer
    {
        public static bool AreEqualNoOrder(XDocument docA, XDocument docB)
        {
            var xmlDocument1 = new XmlDocument();
            using (var xmlReader = docA.CreateReader())
            {
                xmlDocument1.Load(xmlReader);
            }
            var xmlDocument2 = new XmlDocument();
            using (var xmlReader = docB.CreateReader())
            {
                xmlDocument2.Load(xmlReader);
            }
            return AreEqualNoOrder(xmlDocument1, xmlDocument2);
        }

        /// <summary>
        /// Compares two xml documents for eqality as defined for this comparer.
        /// </summary>
        /// <param name="docA">The first xml document.</param>
        /// <param name="docB">the second xml document.</param>
        /// <returns>True if the documents are equal, otherwise false.</returns>
        public static bool AreEqualNoOrder(XmlDocument docA, XmlDocument docB)
        {
            return NodesAreEqualNoOrder(docA.FirstChild, docB.FirstChild);
        }

        /// <summary>
        /// Compares two nodes for equality as defined for this comparer. 
        /// </summary>
        /// <param name="nodeA">A node from the first document.</param>
        /// <param name="nodeB">a node from the second document.</param>
        /// <returns>True if the nodes are equal, otherwise false.</returns>
        private static bool NodesAreEqualNoOrder(XmlNode nodeA, XmlNode nodeB)
        {
            ///////////////
            // Compare Text
            ///////////////
            var textA = nodeA.Value;
            var textB = nodeB.Value;
            if (textA == null || textB == null)
            {
                // if either is null, then they should both be null.
                if (!(textA == null && textB == null))
                {
                    return false;
                }
            }
            else
            {
                // if they are not null, the text should be the same
                if (!textA.Trim().Equals(textB.Trim()))
                {
                    return false;
                }
            }
            /////////////////////
            // Compare Attributes
            /////////////////////
            var attributesA = nodeA.Attributes;
            var attributesB = nodeB.Attributes;
            if (attributesA == null || attributesB == null)
            {
                // if either is null, then they should both be null.
                if (!(attributesA == null && attributesB == null))
                {
                    return false;
                }
            }
            else
            {
                // if there are attributes, there should be the same number on A as on B.
                if (attributesA.Count != attributesB.Count)
                {
                    return false;
                }
                // check each attribute and value
                for (int i = 0; i < attributesA.Count; i++)
                {
                    var name = attributesA[i].Name;
                    // if nodeA has an attribute named x, then so should nodeB.
                    if (attributesB[name] == null)
                    {
                        return false;
                    }
                    // if nodeA and nodeB both have attributes named x, they should have the same value.
                    if (attributesB[name].Value != attributesA[name].Value)
                    {
                        return false;
                    }
                }
            }
            //////////////////////
            // Compare Child Nodes
            //////////////////////
            var childsA = nodeA.ChildNodes;
            var childsB = nodeB.ChildNodes;
            // the number of children of nodeA should be the same as the number of childeren of nodeB.
            if (childsA.Count != childsB.Count)
            {
                return false;
            }
            // every child of nodeA should have a matching child of nodeB, but not necessarily in the same order.
            while (childsA.Count > 0)
            {
                // look for a match in nodeB for the first child of nodeA
                var matchFound = false;
                for (int i = 0; i < childsB.Count; i++)
                {
                    if (NodesAreEqualNoOrder(childsA[0], childsB[i]))
                    {
                        // if a match is found in nodeB, remove the child in nodeA and the match in nodeB, then move on.
                        // this is important because if A had child nodes x and x and B had child nodes x and y,
                        // then both A nodes would match, but B would have a y that wasn't found.
                        matchFound = true;
                        childsA[0].ParentNode.RemoveChild(childsA[0]);
                        childsB[i].ParentNode.RemoveChild(childsB[i]);
                        break;
                    }
                }
                if (!matchFound)
                {
                    // if no match is found, the nodes ain't the same.
                    return false;
                }
            }
            // if no test failed, the nodes are equal.
            return true;
        }
    }
}