using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{


    public class Trie
    {


        public Node root;
        public List<String> resultList;


        public Trie()
        {
            this.root = new Node(' ');
            this.resultList = new List<String>();
        }


        //adds a new title into trie
        public void AddTitle(String title)
        {
            Node node = root;
            foreach (char ch in title)
            {
                char currentCh = ch;
                if (currentCh == '_')
                {
                    currentCh = ' ';
                }
                else
                {
                    currentCh = char.ToLower(currentCh);
                }
                if (!node.dict.ContainsKey(currentCh))
                {
                    node.dict.Add(currentCh, new Node(currentCh));
                }
                node = node.dict[currentCh]; //updates the node
            }
            node.eow = true; //sets the end node's eow to be true
        }


        //finds the starting node for SearchForPrefix
        //returns 10 suggested titles as a list
        public List<String> SearchForPrefix(String userInput)
        {
            resultList.Clear();
            Node node = root;
            foreach (char ch in userInput)
            {
                if (node.dict.ContainsKey(ch))
                {
                    node = node.dict[ch];
                }
                else
                {
                    return new List<String>();
                }
            }
            SearchForPrefixHelper(node, userInput);
            return resultList;
        }


        //adds 10 suggested titles in a resultList
        private void SearchForPrefixHelper(Node node, String result)
        {
            if (resultList.Count() >= 10)
            {
                return;
            }
            if (node.eow == true)
            {
                resultList.Add(result);
            }
            foreach (KeyValuePair<char, Node> pair in node.dict)
            {
                String temp = result;
                temp += pair.Key;
                SearchForPrefixHelper(pair.Value, temp);
            }
        }


    }
}
