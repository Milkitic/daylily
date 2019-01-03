using System;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Bot.Sessions.TreeStructure
{
    public class Node
    {
        private List<Node> _children;
        private int _index;
        private int _order;
        private Node _parent;

        /// <summary>
        /// 以新名称增加一个子节点。
        /// </summary>
        /// <param name="name"></param>
        public Node(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        /// <summary>
        /// 增加一个子节点。
        /// </summary>
        /// <param name="node">节点对象</param>
        public void AddChild(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (_children == null)
                _children = new List<Node>();
            node._index = Index + 1;
            node._order = _children.Count;
            node._parent = this;
            _children.Add(node);
        }

        /// <summary>
        /// 增加一个子节点。
        /// </summary>
        /// <param name="nodeName">节点名</param>
        public void AddChild(string nodeName)
        {
            AddChild(new Node(nodeName));
        }

        /// <summary>
        /// 删除一个子节点。
        /// </summary>
        /// <param name="node">节点对象</param>
        public void RemoveChild(Node node)
        {
            if (_children.Contains(node))
                _children.Remove(node);
            else
                throw new ArgumentOutOfRangeException(nameof(node));
        }

        /// <summary>
        /// 删除一个子节点。
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="removedNode">被删除的节点</param>
        public void RemoveChild(string nodeName, out Node removedNode)
        {
            var child = _children.FirstOrDefault(node => node.Name == nodeName);
            if (child != null)
            {
                _children.Remove(child);
                removedNode = child;
                removedNode._index = 0;
                removedNode._order = 0;
                removedNode._parent = null;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(nodeName));
        }

        public Node SearchChild(string nodeName)
        {
            if (Children == null)
                return null;
            foreach (var child in Children)
            {
                if (child.Name == nodeName)
                    return child;
                var result = child.SearchChild(nodeName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// 获取此节点Id。
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 获取或设置此节点名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取此节点深度。
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// 获取此节点于兄弟节点中的位置。
        /// </summary>
        public int Order => _order;

        /// <summary>
        /// 获取此节点的子节点。
        /// </summary>
        public IReadOnlyList<Node> Children => _children;

        /// <summary>
        /// 获取此节点的第一个子节点。
        /// </summary>
        public Node FirstChild => (_children == null || _children.Count == 0) ? null : _children.First();

        /// <summary>
        /// 获取此节点的最后一个子节点。
        /// </summary>
        public Node LastChild => (_children == null || _children.Count == 0) ? null : _children.Last();

        /// <summary>
        /// 获取此节点的父节点。
        /// </summary>
        public Node Parent => _parent;

        /// <summary>
        /// 获取此节点的所有子孙节点。
        /// </summary>
        public IReadOnlyList<Node> AllChildren => InnerGetAllChildren(new List<Node>());

        /// <summary>
        /// 获取此节点的所有祖父节点。
        /// </summary>
        public IReadOnlyList<Node> AllParent => InnerGetAllParent(new List<Node>());

        private List<Node> InnerGetAllChildren(List<Node> list)
        {
            if (_children == null)
                return null;
            foreach (var child in _children)
            {
                list.Add(child);
                child.InnerGetAllChildren(list);
            }

            return list;
        }

        private List<Node> InnerGetAllParent(List<Node> list)
        {
            if (Parent == null)
                return null;
            list.Add(Parent);
            Parent.InnerGetAllParent(list);
            return list;
        }
    }
}