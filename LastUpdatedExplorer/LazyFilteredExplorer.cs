using LastUpdatedExplorer.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LastUpdatedExplorer
{
    public partial class LazyFilteredExplorer : UserControl
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private Predicate<FileSystemInfo> _filter;
        private DirectoryInfo _rootDirectory;

        public Predicate<FileSystemInfo> Filter
        {
            get => _filter;
            set
            {
                if (_treeView.Nodes.Count > 0)
                    throw new InvalidOperationException("The filter can only be changed if the control is cleared.");

                _filter = value;
            }
        }

        public DirectoryInfo RootDirectory
        {
            get => _rootDirectory;
            set
            {
                if (_treeView.Nodes.Count > 0)
                    throw new InvalidOperationException("The root directory can only be changed if the control is cleared.");

                _rootDirectory = value;
            }
        }

        public LazyFilteredExplorer()
        {
            InitializeComponent();

            _treeView.BeforeSelect += Node_BeforeSelect;
            _treeView.BeforeExpand += Node_BeforeExpand;
            _treeView.NodeMouseDoubleClick += Node_MouseDoubleClick;

            _listView.DoubleClick += ListView_DoubleClick;
        }

        private void Node_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode selected = e.Node;
            DirectoryInfo nodeDirInfo = (DirectoryInfo)selected.Tag;

            // show the files and folders in the list-view
            PopulateListView(nodeDirInfo);
        }

        private async void Node_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // if it's not a lazy-node, don't do anything
            if (!(e.Node is LazyTreeNode node)) return;

            // if the nodes are already present we don't need to load them
            if (node.NodesLoaded) return;

            node.Nodes.Clear(); // delete dummy node
            DirectoryInfo nodeDirInfo = (DirectoryInfo)node.Tag;

            UseWaitCursor = true;           // use loading cursor
            Enabled = false;

            await AddNewLayer(nodeDirInfo, node).ConfigureAwait(true); // add new nodes

            Enabled = true;
            UseWaitCursor = false;          // go back to default cursor

            // nodes are now available, don't waste time if you expand this again
            node.NodesLoaded = true;

            if (!node.IsExpanded) node.Expand();
        }

        private void Node_MouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (MessageBox.Show("Fully expand?",
                    "Expand entirely",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                e.Node.ExpandAll();
            }
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count < 1) return;

            ListViewItem item = _listView.SelectedItems[0];

            // open folder or select file
            if (item.Tag is DirectoryInfo dir)
            {
                Process.Start("explorer.exe", $@"""{dir.FullName}""");
            }
            else if (item.Tag is FileInfo file)
            {
                Process.Start("explorer.exe", $@"/select,""{file.FullName}""");
            }
        }

        public async Task PopulateFirstLayer(bool expandRoot = true)
        {
            if (RootDirectory == null)
                throw new InvalidOperationException("No root directory was specified.");

            if (!RootDirectory.Exists)
                throw new InvalidOperationException("The specified root directory doesn't exist.");

            if(Filter == null)
                throw new InvalidOperationException("No filter was specified.");

            if (_treeView.Nodes.Count > 0)
                throw new InvalidOperationException("The first layer can only be populated if the control is cleared.");

            TreeNode rootNode = new TreeNode(RootDirectory.Name)
            {
                Tag = RootDirectory
            };

            await AddNewLayer(RootDirectory, rootNode).ConfigureAwait(true);

            _treeView.Nodes.Add(rootNode);
            if(expandRoot) rootNode.Expand();
        }

        public void Clear()
        {
            _treeView.Nodes.Clear();
            _listView.Items.Clear();
        }

        private bool CanRead(DirectoryInfo directory)
        {
            try
            {
                var enumerable = directory.EnumerateFileSystemInfos();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // we don't have read-access to this folder
                return false;
            }
        }

        private bool MatchesFilter(FileSystemInfo info)
        {
            // DEBUG
#if DEBUG
            Stopwatch stopwatchFile = Stopwatch.StartNew();
#endif

            bool matches = Filter(info);

            // DEBUG
#if DEBUG
            stopwatchFile.Stop();
#endif

            s_logger.Debug($"{info.Name} is {(matches ? "" : "not ")}a match."
#if DEBUG
                 + $" Evaluated in {stopwatchFile.ElapsedMilliseconds}ms."
#endif
                );

            return matches;
        }

        private bool HasSubFolders(DirectoryInfo directory) =>
            directory.EnumerateDirectories().Any(f => MatchesFilter(f));

        private async Task AddNewLayer(DirectoryInfo currentDir, TreeNode parent)
        {
            // if we don't have read access, we can't do anything
            if (!CanRead(currentDir)) return;

#if DEBUG
            // DEBUG
            Stopwatch rootStopwatch = Stopwatch.StartNew();
#endif

            // "don't waste time drawing yourself, I'm adding nodes"
            //_treeView.BeginUpdate();

            foreach (DirectoryInfo subDir in currentDir.EnumerateDirectories())
            {
#if DEBUG
                // DEBUG
                Stopwatch innerStopwatch = Stopwatch.StartNew();
#endif

                // skip if it doesn't match our criteria
                bool matches = await Task.Run(() => MatchesFilter(subDir)).ConfigureAwait(true);
                if (!matches)
                {
#if DEBUG
                    // DEBUG
                    innerStopwatch.Stop();
#endif
                    continue;
                }

                LazyTreeNode node = new LazyTreeNode(subDir.Name, 0, 0)
                {
                    Tag = subDir,
                    ImageKey = "folder"
                };

#if DEBUG
                // DEBUG
                Stopwatch isExpandableStopwatch = Stopwatch.StartNew();
#endif
                // the node can be expanded if we have read-access and it contains sub-folders
                node.IsExpandable = CanRead(subDir) && HasSubFolders(subDir);

#if DEBUG
                // DEBUG
                isExpandableStopwatch.Stop();
#endif

                s_logger.Debug($"The node for the folder {subDir.Name} is {(node.IsExpandable ? "" : "not ")}expandable."
#if DEBUG
                    + $" Evaluated in {isExpandableStopwatch.ElapsedMilliseconds}ms"
#endif
                    );

                if (node.IsExpandable)
                {
                    // add dummy node
                    node.Nodes.Add("Loading..");
                    // signal that the nodes for this node are not available yet and need to be loaded
                    node.NodesLoaded = false;
                }

                // add to the parent
                parent.Nodes.Add(node);

#if DEBUG
                // DEBUG
                innerStopwatch.Stop();
                s_logger.Debug($"The folder {subDir.Name} was processed and added in {innerStopwatch.ElapsedMilliseconds}ms.");
#endif
            }

            if (parent is LazyTreeNode p)
            {
                // signal that the values for the parent are now available
                p.NodesLoaded = true;
            }

            // "okay the nodes are added, you can draw youself again"
            //_treeView.EndUpdate();

#if DEBUG
            // DEBUG
            rootStopwatch.Stop();
#endif

            s_logger.Debug($"A layer of nodes were added for {currentDir.Name}."
#if DEBUG
                + $" In {rootStopwatch.ElapsedMilliseconds}ms."
#endif
                );
        }

        private void PopulateListView(DirectoryInfo dir)
        {
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            // remove old files and dirs
            _listView.Items.Clear();

            // show dummy if we don't have access
            if (!CanRead(dir))
            {
                _listView.Items.Add("No access..");
                _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                return;
            }

            // "don't waste time drawing yourself, I'm adding rows"
            _listView.BeginUpdate();
            UseWaitCursor = true;   // use loading cursor

            // add directories
            foreach (DirectoryInfo subDir in dir.EnumerateDirectories()
                                                .Where(d => MatchesFilter(d))
                                                .OrderBy(d => d.Name))
            {
                item = new ListViewItem(subDir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "Directory"),
                    new ListViewItem.ListViewSubItem(item, subDir.CreationTime.ToString("G", CultureInfo.CurrentCulture)),
                    new ListViewItem.ListViewSubItem(item, subDir.LastWriteTime.ToString("G", CultureInfo.CurrentCulture)),
                    new ListViewItem.ListViewSubItem(item, subDir.LastAccessTime.ToString("G", CultureInfo.CurrentCulture))
                };

                item.Tag = subDir;

                item.SubItems.AddRange(subItems);
                _listView.Items.Add(item);
            }

            // add files
            foreach (FileInfo file in dir.EnumerateFiles()
                                            .Where(f => MatchesFilter(f))
                                            .OrderBy(f => f.Name))
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "File"),
                    new ListViewItem.ListViewSubItem(item, file.CreationTime.ToString("G", CultureInfo.CurrentCulture)),
                    new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToString("G", CultureInfo.CurrentCulture)),
                    new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToString("G", CultureInfo.CurrentCulture))
                };

                item.Tag = file;

                item.SubItems.AddRange(subItems);
                _listView.Items.Add(item);
            }

            // resize columns
            _listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // "okay the rows are added, you can draw youself again"
            _listView.EndUpdate();
            UseWaitCursor = false;  // go back to default cursor
        }
    }
}
