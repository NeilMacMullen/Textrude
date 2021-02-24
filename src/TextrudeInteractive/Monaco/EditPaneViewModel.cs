﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TextrudeInteractive.Annotations;

namespace TextrudeInteractive
{
    public class EditPaneViewModel : INotifyPropertyChanged

    {
        private string _format = string.Empty;
        private string _linkedPath = string.Empty;
        private string _scribanName = string.Empty;
        private string _text = string.Empty;
        public FileLinkageTypes FileLinkage { get; set; } = FileLinkageTypes.LoadSave;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text || value == null) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Format
        {
            get => _format;
            set
            {
                if (value == _format || value == null) return;
                _format = value;
                OnPropertyChanged();
            }
        }

        public string LinkedPath
        {
            get => _linkedPath;
            set
            {
                if (value == _linkedPath || value == null) return;
                _linkedPath = value;
                OnPropertyChanged();
            }
        }

        public string ScribanName
        {
            get => _scribanName;
            set
            {
                if (value == _scribanName || value == null) return;
                _scribanName = value;
                OnPropertyChanged();
            }
        }

        //this doesn't have to be notifiable because it is constant 
        public string[] AvailableFormats { get; set; } = Array.Empty<string>();
        public PaneType PaneType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Clear()
        {
            //temporary
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveIfLinked()
        {
            FileManager.TrySave(LinkedPath, Text);
        }

        public void LoadIfLinked()
        {
            if (FileManager.TryLoadFile(LinkedPath, out var text))
                Text = text;
        }
    }
}
