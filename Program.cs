//Programmer: Shay Deshner
//Date: 4/04/2018
//The pattern for this program was based on a stackoverflow post by Charles Magar
//https://stackoverflow.com/questions/32215108/how-to-read-a-hierarchical-xml-with-linq-and-print-the-result-that-can-indicate

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XML_Recursion
{
    class Program
    {
        static void Main(string[] args)
        {
            //test to make sure exactly two arguments have been passed to the application
            if (args.Length < 2 || args.Length > 2)
            {
                Console.WriteLine("This console application Accepts two arguments: first, a path to a menu .xml file (e.g. \"c:\\schedaeromenu.xml\"); second an active path to match (e.g. \"/default.aspx\") ");
            }
            else
            {
                string xmlDocText = @args[0];
                string filePathText = args[1];
                
                //test if the file exists
                if (File.Exists(xmlDocText))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(xmlDocText);
                        List<Node> nodes = xmlDoc.Descendants("item").Select(NodeFrom).ToList();

                        WriteNodes(nodes, filePathText);
                        Console.ReadKey();
                    }
                    catch
                    {
                        Console.WriteLine("XML Failed to parse");
                    }
                }
            }
        }

        //Method builds a Node class object out of all item tags and their children item tags
        private static Node NodeFrom(XElement element)
        {
            return new Node(
                (string)element.Element("path").Attribute("value"),
                (string)element.Element("displayName"),
                 element.Descendants("item").Select(NodeFrom).ToList()

            );
        }


        //method writes to the console
        private static void WriteNodes(IEnumerable<Node> nodes, string path ,int depth = 0)
        {
            List<Node> nodesList = nodes.ToList();
            List<Node> nodeChildren = new List<Node>();

            for (int i = 0; i < nodesList.Count; i++)
            {
                //create a list of child nodes
                List<Node> children = nodesList[i].Children.ToList();
                if (children.Count() != 0) {
                    foreach (Node nodeChild in children)
                    {
                        nodeChildren.Add(nodeChild);
                    }
                }
            }

            //filter child nodes out of input list
            List<Node> result = nodesList.Where(p => !nodeChildren.Any(l => p.DisPlayName == l.DisPlayName)).ToList();

            foreach (var node in result)
            {
                //if a path value matches the input or a parent has a child that mathes mark a active
                if (path == node.PathValue || node.Children.Any(i => i.PathValue == path)) {
                    Console.Write(new string(' ', depth * 3));
                    Console.WriteLine(node.DisPlayName + ", " + node.PathValue + " ACTIVE");
                    WriteNodes(node.Children, path, depth + 1);
                }
                else
                {
                    Console.Write(new string(' ', depth * 3));
                    Console.WriteLine(node.DisPlayName + ", " + node.PathValue);
                    WriteNodes(node.Children, path, depth + 1);
                }
            }
        }
    }

   
    public class Node
    {
        public Node(string pathValue, string displayName, IEnumerable<Node>children)
        {
            PathValue = pathValue;
            DisPlayName = displayName;
            Children = children;
        }

        public string PathValue { get; private set; }
        public string DisPlayName { get; private set; }
        public IEnumerable<Node> Children { get; private set; }

    }
}
