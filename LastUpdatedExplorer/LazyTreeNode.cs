using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LastUpdatedExplorer
{
    public class LazyTreeNode : TreeNode
    {
        public bool IsExpandable { get; internal set; }
        public bool NodesLoaded { get; internal set; }



        public LazyTreeNode()
        {
        }

        public LazyTreeNode(string text) : base(text)
        {
        }

        public LazyTreeNode(string text, TreeNode[] children) : base(text, children)
        {
        }

        public LazyTreeNode(string text, int imageIndex, int selectedImageIndex) : base(text, imageIndex, selectedImageIndex)
        {
        }

        public LazyTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : base(text, imageIndex, selectedImageIndex, children)
        {
        }

        protected LazyTreeNode(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }
    }
}
