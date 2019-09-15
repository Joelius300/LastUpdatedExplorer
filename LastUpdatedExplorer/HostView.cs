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

        private bool _includeValuesFromFolders = true;
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
                return f => true;
            }
            else
            {
                // TODO: dynamically create core filter using the search criteria, (probably) using Expressions

                List<Expression> expressions = new List<Expression>();

                ParameterExpression fileInfoParam = Expression.Parameter(typeof(FileSystemInfo), "fileInfo");
                MethodInfo isBetweenMethod = typeof(DateTimeExtensions).GetMethod("Between", BindingFlags.Public | BindingFlags.Static);

                if (searchCriteria.HasFlag(SearchCriteria.CreationTime))
                {
                    PropertyInfo creationTimeProp = typeof(FileSystemInfo).GetProperty("CreationTime");
                    expressions.Add(
                        Expression.Call(
                                null,
                                isBetweenMethod,
                                new Expression[] {
                                Expression.Property(fileInfoParam, creationTimeProp), 
                                Expression.Constant(start),
                                Expression.Constant(end)
                                }));
                }

                if (searchCriteria.HasFlag(SearchCriteria.LastModified))
                {
                    PropertyInfo lastModifiedProp = typeof(FileSystemInfo).GetProperty("LastWriteTime");
                    Expression checkLastModified =
                        Expression.Call(
                                null,
                                isBetweenMethod,
                                new Expression[] {
                                Expression.Property(fileInfoParam, lastModifiedProp), 
                                Expression.Constant(start),
                                Expression.Constant(end)
                                });

                    if (expressions.Count == 0)
                    {
                        expressions.Add(checkLastModified);
                    }
                    else
                    {
                        Expression lastExpression = expressions.Last();
                        expressions.RemoveAt(expressions.Count - 1);

                        expressions.Add(
                            Expression.OrElse(
                                lastExpression,
                                checkLastModified));
                    }
                }

                if (searchCriteria.HasFlag(SearchCriteria.LastAccess))
                {
                    PropertyInfo lastAccessProp = typeof(FileSystemInfo).GetProperty("LastAccessTime");
                    Expression checkLastAccess =
                        Expression.Call(
                                null,
                                isBetweenMethod,
                                new Expression[] {
                                Expression.Property(fileInfoParam, lastAccessProp), 
                                Expression.Constant(start),
                                Expression.Constant(end)
                                });

                    if (expressions.Count == 0)
                    {
                        expressions.Add(checkLastAccess);
                    }
                    else
                    {
                        Expression lastExpression = expressions.Last();
                        expressions.RemoveAt(expressions.Count - 1);

                        expressions.Add(
                            Expression.OrElse(
                                lastExpression,
                                checkLastAccess));
                    }
                }

                Predicate<FileSystemInfo> coreFilter =
                    Expression.Lambda<Predicate<FileSystemInfo>>(Expression.Block(expressions), fileInfoParam).Compile();

                return f => ContainsAnyChange(f, coreFilter);
            }
        }

        private bool ContainsAnyChange(FileSystemInfo info, Predicate<FileSystemInfo> coreFilter)
        {
            if (_matchesCache.TryGetValue(info.FullName, out bool matchesFromCache))
            {
                s_logger.Debug($"{info.Name} was cached; short-circuit used.");
                return matchesFromCache;
            }

            if (info is DirectoryInfo dir)
            {
                if (_includeValuesFromFolders && coreFilter(info))
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
                    /* skip */
                    s_logger.Warn($"No read-access for {info.Name}; don't include => no match.");
                }
            }
            else
            {
                if (coreFilter(info))
                {
                    _matchesCache.Add(info.FullName, true);
                    return true;
                }
            }

            _matchesCache.Add(info.FullName, false);
            return false;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void BtnGo_Click(object sender, EventArgs e)
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

            // clear cache
            _matchesCache.Clear();

            // clear explorer and redraw form
            _explorer.Clear();
            Refresh();

            // communicate values with explorer
            _explorer.RootDirectory = new DirectoryInfo(_txtPath.Text);
            _explorer.Filter = GetFilter(_lastUpdateStart, _lastUpdateEnd, _searchCriteria);

            // add root node and expand it
            _explorer.PopulateFirstLayer(true);
        }
    }
}
