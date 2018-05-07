﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using Microsoft.Toolkit.Uwp.UI.Controls.Graph;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class AadLoginPage : IXamlRenderListener
    {
        private AadLogin _aadLoginControl;
        private string _graphAccessToken;
        private string _userId;

        public AadLoginPage()
        {
            InitializeComponent();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _aadLoginControl = control.FindDescendantByName("AadLoginControl") as AadLogin;
            if (_aadLoginControl != null)
            {
                _aadLoginControl.SignInCompleted += AadLoginControl_SignInCompleted;
                _aadLoginControl.SignOutCompleted += AadLoginControl_SignOutCompleted;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Shell.Current.RegisterNewCommand("Change default image", async (sender, args) =>
            {
                if (_aadLoginControl != null)
                {
                    var openPicker = new FileOpenPicker();
                    openPicker.ViewMode = PickerViewMode.Thumbnail;
                    openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    openPicker.FileTypeFilter.Add(".jpg");
                    openPicker.FileTypeFilter.Add(".jpeg");
                    openPicker.FileTypeFilter.Add(".png");
                    openPicker.FileTypeFilter.Add(".gif");
                    openPicker.FileTypeFilter.Add(".bmp");

                    // Open a stream for the selected file
                    StorageFile file = await openPicker.PickSingleFileAsync();

                    // Ensure a file was selected
                    if (file != null)
                    {
                        using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            // Set the image source to the selected bitmap
                            var defaultImage = new BitmapImage();
                            await defaultImage.SetSourceAsync(fileStream);
                            _aadLoginControl.DefaultImage = defaultImage;
                        }
                    }
                }
            });

            Shell.Current.RegisterNewCommand("Copy GraphAccessToken to clipboard", async (sender, args) =>
            {
                if (_aadLoginControl != null)
                {
                    if (string.IsNullOrEmpty(_graphAccessToken))
                    {
                        var dialog = new MessageDialog("Please click the profile button to login first.");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        DataPackage copyData = new DataPackage();
                        copyData.SetText(_graphAccessToken);
                        Clipboard.SetContent(copyData);
                    }
                }
            });

            Shell.Current.RegisterNewCommand("Copy UserId to clipboard", async (sender, args) =>
            {
                if (_aadLoginControl != null)
                {
                    if (string.IsNullOrEmpty(_userId))
                    {
                        var dialog = new MessageDialog("Please click the profile button to login first.");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        DataPackage copyData = new DataPackage();
                        copyData.SetText(_userId);
                        Clipboard.SetContent(copyData);
                    }
                }
            });
        }

        private void AadLoginControl_SignInCompleted(object sender, SignInEventArgs e)
        {
            _graphAccessToken = e.GraphAccessToken;
            _userId = e.CurrentSignInUserId;
        }

        private void AadLoginControl_SignOutCompleted(object sender, EventArgs e)
        {
            _graphAccessToken = string.Empty;
            _userId = string.Empty;
        }
    }
}
