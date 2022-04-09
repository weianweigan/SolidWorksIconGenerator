using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SwIconGenerator
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Icon> _icons = new ObservableCollection<Icon>();
        private RelayCommand _openFolderCmd;
        private RelayCommand _openFileCmd;
        private string _desDir;
        private Icon _selectedIcon;
        private ImageSource _previewImage;
        private RelayCommand _upCmd;
        private RelayCommand _downCmd;
        private RelayCommand _exportCmd;

        public List<IconBarSize> Sizes { get; set; } = new List<IconBarSize>()
        {
            new IconBarSize(20),
            new IconBarSize(32),
            new IconBarSize(40),
            new IconBarSize(64),
            new IconBarSize(96),
            new IconBarSize(128),
        };

        public ObservableCollection<Icon> Icons
        {
            get => _icons; set
            {
                _icons = value;
                RaisePropertyChanged(nameof(Icons));
            }
        }

        public string DesDir { get => _desDir; set => Set(ref _desDir, value); }

        public ImageSource PreviewImage { get => _previewImage; set => Set(ref _previewImage, value); }

        public RelayCommand OpenFolderCmd { get => _openFolderCmd ?? (_openFolderCmd = new RelayCommand(OpenFolderClick)); }

        public RelayCommand OpenFileCmd { get => _openFileCmd ?? (_openFileCmd = new RelayCommand(OpenFileClick)); }

        public RelayCommand UpCmd { get => _upCmd ?? (_upCmd = new RelayCommand(UpClick, () => SelectedIcon != null)); }

        public RelayCommand DownCmd { get => _downCmd ?? (_downCmd = new RelayCommand(DownClick, () => SelectedIcon != null)); }

        public RelayCommand ExportCmd { get => _exportCmd ?? (_exportCmd = new RelayCommand(ExportClick)); }

        private void ExportClick()
        {
            try
            {
                if (!Directory.Exists(DesDir))
                {
                    Directory.CreateDirectory(DesDir);
                }

                var sizes = Sizes.Where(p => p.Enable);
                foreach (var size in sizes)
                {
                    var bitmap = IconGeneratorUtil.CombineBitmap(Icons.Select(p => p.FilePathName).ToList(), size.Size);
                    var location = Path.Combine(DesDir, $"ToolBarImages{size.Size}.png");
                    bitmap.Save(location);
                    bitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Icon SelectedIcon
        {
            get => _selectedIcon; set
            {
                Set(ref _selectedIcon, value);
                UpCmd.RaiseCanExecuteChanged();
                DownCmd.RaiseCanExecuteChanged();
            }
        }

        private void OpenFileClick()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Image|*.png",
                Multiselect = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;
                foreach (var file in files)
                {
                    Icons.Add(new Icon(file));
                }
                PreView();
            }
        }

        private void OpenFolderClick()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var dir = dialog.SelectedPath;
                var files = Directory.GetFiles(dir)
                    .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

                foreach (var file in files)
                {
                    Icons.Add(new Icon(file));
                }
                DesDir = Path.Combine(dir, "Result");
                PreView();
            }
        }

        private void DownClick()
        {
            var selec = SelectedIcon;
            var index = Icons.IndexOf(selec);
            if (index >= 0 && index < Icons.Count - 1)
            {
                Icons.RemoveAt(index);
                Icons.Insert(index + 1, selec);
            }
            SelectedIcon = selec;
            PreView();
        }

        private void UpClick()
        {
            var selec = SelectedIcon;
            var index = Icons.IndexOf(selec);
            if (index > 0)
            {
                Icons.RemoveAt(index);
                Icons.Insert(index - 1, selec);
            }
            SelectedIcon = selec;
            PreView();
        }

        public void PreView()
        {
            var bitmap = IconGeneratorUtil.CombineBitmap(Icons.Select(p => p.FilePathName).ToList(), 32);
            if (bitmap != null)
            {
                PreviewImage = BitmapSourceConvert.ToBitmapSource(bitmap);
                bitmap.Dispose();
            }
        }

        private RelayCommand _browserCmd;

        public ICommand BrowserCmd
        {
            get
            {
                if (_browserCmd == null)
                {
                    _browserCmd = new RelayCommand(PerformBrowserCmd);
                }

                return _browserCmd;
            }
        }

        private void PerformBrowserCmd()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DesDir = dialog.SelectedPath;
            }
        }
    }
}
