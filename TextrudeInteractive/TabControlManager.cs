using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Manages a TabControl
    /// </summary>
    /// <typeparam name="T"> type of Pane</typeparam>
    public class TabControlManager : EditPaneViewModel
    {
        private readonly InputMonacoPane _editPane;

        private readonly TabControl _tab;
        public List<EditPaneViewModel> Panes = new();

        public TabControlManager(TabControl tab,
            InputMonacoPane editPane)
        {
            _tab = tab;
            _editPane = editPane;
            _tab.SelectionChanged += TabOnSelectionChanged;
        }

        public int Count => Panes.Count;

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
        }

        /// <summary>
        ///     Remove the right-most tab from the control
        /// </summary>
        public void RemoveLast()
        {
            if (_tab.Items.Count == 0)
            {
                return;
            }

            var last = _tab.Items[^1] as TabItem;
            _tab.Items.Remove(last);
            Panes.RemoveAt(Panes.Count - 1);
        }

        /// <summary>
        ///     Remove all the tabs from the control
        /// </summary>
        public void Clear()
        {
            while (Panes.Count > 0)
                RemoveLast();
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
    }
}
