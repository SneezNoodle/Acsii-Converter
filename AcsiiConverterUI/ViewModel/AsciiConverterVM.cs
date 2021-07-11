using AsciiConverterUI.Services;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AsciiConverterUI.ViewModel
{
    public class AsciiConverterVM : Bindable
    {
        #region Backing
        private string _lineHeightEntry = "7";
        private string _fontSizeEntry = "7";
        private string _symbolEntry;
        private BitmapSource _currentImage;
        private string _asciiText;
        #endregion

        public string LineHeightEntry
        {
            get => _lineHeightEntry;
            set => SetProperty(ref _lineHeightEntry, value);
        }
        public string FontSizeEntry
        {
            get => _fontSizeEntry;
            set => SetProperty(ref _fontSizeEntry, value);
        }
        public string SymbolEntry
        {
            get => _symbolEntry;
            set => SetProperty(ref _symbolEntry, value);
        }
        public BitmapSource CurrentImage
        {
            get => _currentImage;
            set => SetProperty(ref _currentImage, value);
        }
        public string AsciiText
        {
            get => _asciiText;
            set => SetProperty(ref _asciiText, value);
        }

        public AsciiConverter Converter { get; }

        public Command LoadImageCMD { get; }
        public Command PasteImageCMD { get; }
        public Command GenerateAsciiCMD { get; }
        public Command CopyAsciiCMD { get; }
        public Command UpdateSymbolsCMD { get; }

        public AsciiConverterVM()
        {
            LoadImageCMD = new Command(LoadImageFromPath);
            PasteImageCMD = new Command(LoadImageFromClipboard);
            GenerateAsciiCMD = new Command(GenerateAscii);
            CopyAsciiCMD = new Command(CopyAsciiToClipboard);
            UpdateSymbolsCMD = new Command(UpdateSymbols);

            // Default shade characters
            Converter = new AsciiConverter(10, new char[]
            {
                ' ',
                '.',
                '<',
                '(',
                '%',
                '#',
            });
            SymbolEntry = new string(Converter.Palette);
        }

        private void LoadImageFromPath()
        {
            // Open a file dialog
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Image files|*.png;*.jpg",
            };

            // Open the selected file if the dialog was accepted
            if (fileDialog.ShowDialog() == true)
            {
                string errorMessage = string.Empty;

                // Try to open the selected file
                try
                {
                    var uri = new Uri(fileDialog.FileName);
                    CurrentImage = new BitmapImage(uri);
                } // Catch errors and display the appropriate message
                catch (System.IO.FileNotFoundException)
                { errorMessage = "File not found."; }
                catch (UriFormatException)
                { errorMessage = "A Uri format error occured."; }
                catch (ArgumentNullException)
                { errorMessage = "Somehow, the selected file resulted in a null path. Just talk to Sneez."; }

                // If an error occured, display the respective message
                if (errorMessage != string.Empty)
                    _ = MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
            }
        }
        private void LoadImageFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                CurrentImage = img;
            }
            else
                MessageBox.Show("Invalid clipboard data.", "Error", MessageBoxButton.OK);
        }
        private void CopyAsciiToClipboard()
        {
            if (AsciiText != null)
                Clipboard.SetText(AsciiText);
            else
                _ = MessageBox.Show("No Ascii to copy.", "Error", MessageBoxButton.OK);
        }
        private void GenerateAscii()
        {
            // Display an error if no image is available
            if (CurrentImage == null)
            {
                _ = MessageBox.Show(
                    "No image imported.\nUse the \"Paste\" or \"Open file\" buttons to import an image.",
                    "Error",
                    MessageBoxButton.OK);
                return;
            }

            // Get the lightness map for the current image
            var pixels = ImageReader.GetLightnessMap(ImageReader.GetPixels(CurrentImage));

            // Generate Ascii and save it to AsciiString
            AsciiText = Converter.GetAsciiString(pixels, CurrentImage.PixelWidth);
        }
        private void UpdateSymbols()
        {
            // Get symbols from SymbolEntry
            char[] symbols = new char[SymbolEntry.Length];
            for (int i = 0; i < SymbolEntry.Length; i++)
            {
                symbols[i] = SymbolEntry[i];
            }

            // Set Converter.ShadeChars
            Converter.Palette = symbols;
        }
    }
}
