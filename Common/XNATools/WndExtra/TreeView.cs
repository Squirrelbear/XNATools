using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;

namespace XNATools.WndExtra
{
    public class TreeView : Panel
    {
        //http://stackoverflow.com/questions/9860207/build-a-simple-high-performance-tree-data-structure-in-c-sharp
        public class TreeNode
        {
            public int NodeID { get; set; }
            public string ContextName { get; set; }
            public object Data { get; set; }
            public Button AssociatedBtn { get; set; }

            public List<TreeNode> Children { get; set; }
        }

        private List<TreeNode> headNodes;

        public TreeView(Rectangle rect, TreeNode head = null)
            : this(rect, new List<TreeNode>())
        {
        }

        public TreeView(Rectangle rect, List<TreeNode> headNodes)
            : base(rect)
        {
            this.headNodes = headNodes;

            
        }

        public void setNodeList(List<TreeNode> newNodes)
        {
        }

        public TreeNode findParentOfNode(TreeNode node)
        {
            return null;
        }

        public void clearNodes(TreeNode node = null)
        {
            if (node == null && headNodes == null)
            {
                return;
            }
            else if (node == null)
            {
                foreach (TreeNode n in headNodes)
                {
                    clearNodes(n);
                }
            }
            else
            {
                removeComponent(node.AssociatedBtn);
                clearAllChildren(node);

            }
        }

        private void clearAllChildren(TreeNode node)
        {
            if(node.Children == null) return;

            foreach(TreeNode n in node.Children)
            {
                removeComponent(n.AssociatedBtn);
                if (node.Children != null)
                {
                    clearAllChildren(n);
                }
            }
        }
    }
}
