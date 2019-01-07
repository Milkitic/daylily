namespace Daylily.Bot.Session.TreeStructure
{
    public class Tree
    {
        public Tree(Node rootNode)
        {
            Root = rootNode;
        }

        public Tree(string rootName) : this(new Node(rootName))
        {
        }

        /// <summary>
        /// 查找指定子节点。
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public virtual Node SearchNode(string nodeName)
        {
            if (Root.Name == nodeName)
                return Root;
            if (Root.Children == null)
                return null;
            return Root.SearchChild(nodeName);
        }

        public Node Root { get; }
    }
}
