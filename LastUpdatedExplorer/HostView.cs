using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace LastUpdatedExplorer
{
    public partial class HostView : Form
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, bool> _matchesCache = new Dictionary<string, bool>();

        private bool _considerFolderStamps;
        private DateTime _lastUpdateStart;
        private DateTime _lastUpdateEnd;
        private SearchCriteria _searchCriteria;

        private bool AnyFilterSet =>
            _cbxCreationTime.Checked ||
            _cbxLastModified.Checked ||
            _cbxLastAccessed.Checked;

        public HostView()
        {
            InitializeComponent();

            string dateAndTimePattern = $"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern}";

            _dtpFrom.Format = DateTimePickerFormat.Custom;
            _dtpFrom.CustomFormat = dateAndTimePattern;

            _dtpTo.Format = DateTimePickerFormat.Custom;
            _dtpTo.CustomFormat = dateAndTimePattern;
        }

        private Predicate<FileSystemInfo> GetFilter(DateTime start, DateTime end, SearchCriteria searchCriteria)
        {
            s_logger.Info($"Creating filter for files between {start.ToLongTimeString()} and {end.ToLongTimeString()}. Criteria are {searchCriteria}.");

            if (searchCriteria == SearchCriteria.None)
            {
                // just include everything
                return f => true;
            }
            else
            {
                Expression<Predicate<FileSystemInfo>> expression = null;

                // add filter for creation time
                if (searchCriteria.HasFlag(SearchCriteria.CreationTime))
                {
                    Expression<Predicate<FileSystemInfo>> checkCreationTime = info => info.CreationTime.Between(start, end);
                    expression = checkCreationTime; // no need to check, it's going to be null
                }

                // add filter for modified time (|| existing if necessary)
                if (searchCriteria.HasFlag(SearchCriteria.LastModified))
                {
                    Expression<Predicate<FileSystemInfo>> checkLastModified = info => info.LastWriteTime.Between(start, end);
                    expression = expression.OrIfNotNull(checkLastModified);
                }

                // add filter for access time (|| existing if necessary)
                if (searchCriteria.HasFlag(SearchCriteria.LastAccess))
                {
                    Expression<Predicate<FileSystemInfo>> checkLastAccess = info => info.LastAccessTime.Between(start, end);
                    expression = expression.OrIfNotNull(checkLastAccess);
                }

                // compile to delegate
                Predicate<FileSystemInfo> coreFilter = expression.Compile();

                // return predicate which additionally uses cache for folders in combination with the core-filter
                return f => ContainsAnyChange(f, coreFilter);
            }
        }

        private bool ContainsAnyChange(FileSystemInfo info, Predicate<FileSystemInfo> coreFilter)
        {
            if (info is DirectoryInfo dir)
            {
                if (_matchesCache.TryGetValue(info.FullName, out bool matchesFromCache))
                {
                    s_logger.Debug($"{info.Name} was cached; short-circuit used.");
                    return matchesFromCache;
                }

                if (_considerFolderStamps && coreFilter(info))
                {
                    _matchesCache.Add(info.FullName, true);
                    return true;
                }

                try
                {
                    // recursive call for the folder content
                    bool containsChange = dir.EnumerateFileSystemInfos()
                        .Any(f => ContainsAnyChange(f, coreFilter));

                    _matchesCache.Add(info.FullName, containsChange);
                    return containsChange;
                }
                catch (UnauthorizedAccessException)
                {
                    s_logger.Warn($"No read-access for {info.Name}; don't include => no match.");
                    _matchesCache.Add(info.FullName, false);
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    s_logger.Warn($"Parts of the folder {info.Name} were not found; don't include => no match.");
                    _matchesCache.Add(info.FullName, false);
                    return false;
                }
            }

            // it's just a file at this point
            bool matches = coreFilter(info);
            return matches;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (_folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _txtPath.Text = _folderBrowserDialog.SelectedPath;
            }
        }

        private async void BtnGo_Click(object sender, EventArgs e)
        {
            if (_dtpFrom.Value > _dtpTo.Value)
            {
                MessageBox.Show("The start time has to be lower than the end time.",
                    "End time lower than start time",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!Directory.Exists(_txtPath.Text))
            {
                MessageBox.Show("The root directory has to exist.",
                    "Non-existent root directory",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!AnyFilterSet)
            {
                if (MessageBox.Show("Are you sure you want to browse without any filter?",
                    "No filter",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            // save start and end time
            _lastUpdateStart = _dtpFrom.Value;
            _lastUpdateEnd = _dtpTo.Value;

            // create the search criteria using logical OR and flags enum
            _searchCriteria = SearchCriteria.None;

            if (_cbxCreationTime.Checked) _searchCriteria |= SearchCriteria.CreationTime;
            if (_cbxLastModified.Checked) _searchCriteria |= SearchCriteria.LastModified;
            if (_cbxLastAccessed.Checked) _searchCriteria |= SearchCriteria.LastAccess;

            // check if we should consider the folder timestamps themselfs as well
            _considerFolderStamps = _cbxConsiderFolderStamps.Checked;

            // clear cache
            _matchesCache.Clear();

            // clear explorer and redraw form
            _explorer.Clear();
            Refresh();

            // communicate values with explorer
            _explorer.RootDirectory = new DirectoryInfo(_txtPath.Text);
            _explorer.Filter = GetFilter(_lastUpdateStart, _lastUpdateEnd, _searchCriteria);

            // add root node and expand it
            await _explorer.PopulateFirstLayer(true).ConfigureAwait(true);
        }
    }
}
