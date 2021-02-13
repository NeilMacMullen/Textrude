using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Manages a TabControl
    /// </summary>
    /// <typeparam name="T"> type of Pane</typeparam>
    public class TabControlManager
    {
        private readonly InputMonacoPane _editPane;

        private readonly TabControl _tab;
        public List<EditPaneViewModel> Panes = new();
        private TabItem victim;

        public TabControlManager(TabControl tab,
            InputMonacoPane editPane)
        {
            _tab = tab;
            _editPane = editPane;
            _tab.SelectionChanged += TabOnSelectionChanged;
        }

        public int Count => Panes.Count;

        public EditPaneViewModel CurrentPane()
        {
            if (!Panes.Any())
                return new EditPaneViewModel();
            return Panes[_tab.SelectedIndex];
        }

        private void TabOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tab.SelectedIndex >= 0 && _tab.SelectedIndex < Panes.Count)
            {
                _editPane.DataContext = Panes[_tab.SelectedIndex];
            }
        }

        /// <summary>
        ///     Adds a pane to the tab
        /// </summary>
        public void AddPane(EditPaneViewModel pane)
        {
            Panes.Add(pane);
            if (string.IsNullOrWhiteSpace(pane.ScribanName))
                pane.ScribanName = "model";
            _tab.Items.Add(
                new TabItem
                {
                    Header = $"{pane.ScribanName}"
                });
            _tab.SelectedIndex = Math.Max(0, _tab.Items.Count - 1);
            EnsureOrdered();
        }

        public void EnsureOrdered()
        {
            if (!Panes.Any())
                return;

            var currentList = Panes
                .Select((p, i) => new {Pane = p, CurrentPos = i})
                .ToArray();

            var orderedList = currentList
                .OrderBy(p => p.Pane.PaneType)
                .ThenBy(p => p.CurrentPos)
                .ToArray();

            if (currentList.SequenceEqual(orderedList))
                return;

            var selectedPane = Panes[_tab.SelectedIndex];

            var c = 0;
            foreach (var p in orderedList)
            {
                var tabItem = (_tab.Items[c] as TabItem);
                tabItem.DataContext = p.Pane;
                tabItem.Header = p.Pane.ScribanName;
                c++;
            }

            Panes = orderedList.Select(o => o.Pane).ToList();
            _tab.SelectedIndex = Panes.IndexOf(selectedPane);
        }

        /// <summary>
        ///     Remove all the tabs from the control
        /// </summary>
        public void Clear()
        {
            Panes.Clear();
            _tab.Items.Clear();
        }

        public void ForAll(Action<EditPaneViewModel> func)
        {
            foreach (var p in Panes)
                func(p);
        }

        /// <summary>
        ///     Ensure that the first tab is visible
        /// </summary>
        public void FocusFirst()
        {
            if (_tab.Items.Count != 0)
                _tab.SelectedIndex = 0;
        }

        public void ToggleVisibility(Func<EditPaneViewModel, bool> shouldToggle)
        {
            for (var i = 0; i < Panes.Count; i++)
            {
                if (shouldToggle(Panes[i]))
                {
                    var tabItem = _tab.Items[i] as TabItem;
                    ToggleVisibility(tabItem);
                }
            }
        }

        private static void ToggleVisibility(Control c)
        {
            c.Visibility = c.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public void RemoveSelected(Func<EditPaneViewModel, bool> shouldRemove)
        {
            if (!Panes.Any())
                return;
            var index = _tab.SelectedIndex;
            var currentPane = Panes[index];
            if (shouldRemove(currentPane))
            {
                victim = _tab.Items[index] as TabItem;
                _tab.Items.Remove(victim);
                Panes.RemoveAt(index);
            }
        }

        public void RepaintHeaders()
        {
            for (var i = 0; i < Panes.Count; i++)
            {
                (_tab.Items[i] as TabItem).Header = Panes[i].ScribanName;
            }
        }
    }
}
