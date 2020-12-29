#region License
/*
MIT License

Copyright (c) 2020 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using ImageBox.Coloring;
using ImageBox.Flipping;
using ImageBox.Splitting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Triangulator.UI.Controls;
using Triangulator.UI.Properties;
using WaterWave;
using Splitter = ImageBox.Splitting.Splitter;

namespace Triangulator.UI.Windows
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            OriginalImageZoomValue = 100;
            PreviewImageZoomValue = 100;
        }

        public virtual bool IsAutomaticChange { get; set; }

        protected virtual double OriginalImageZoomValue { get; set; }

        protected virtual double PreviewImageZoomValue { get; set; }

        protected virtual void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutWindow().ShowDialog(this);
        }

        protected virtual void AngleNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (OriginalImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual void ApplyChanges()
        {
            if (OriginalImagePictureBox.Image == default) { return; }

            using (var imageBox = new ImageBox.ImageBox(OriginalImagePictureBox.Image).Statistics())
            {
                ConfigurationGrayScaleCheckBox.Checked = !imageBox.Stats.IsGrayScale;

                OriginalImageBrightestPictureBox.BackColor = imageBox.Stats.Brightest;
                OriginalImageDarkestPictureBox.BackColor = imageBox.Stats.Darkest;

                // Gray Scale & Rotation & Flip
                imageBox.Color(!imageBox.Stats.IsGrayScale ? ColorerMatrix.GrayScale() : ColorerMatrix.Identity())
                         .Rotate((float)AngleNumericUpDown.Value, FillColorPictureBox.BackColor)
                         .Flip(((FlipComboBoxItem)FlipComboBox.SelectedItem).Type)
                         .Statistics();

                ModifiedImageBrightestPictureBox.BackColor = imageBox.Stats.Brightest;
                ModifiedImageDarkestPictureBox.BackColor = imageBox.Stats.Darkest;

                if (PreviewImagePictureBox.Image != default)
                {
                    var img = PreviewImagePictureBox.Image;
                    PreviewImagePictureBox.Image = default;
                    img.Dispose();
                }

                if (PreviewImagePictureBox.Tag != default)
                {
                    var img = PreviewImagePictureBox.Tag as Image;
                    PreviewImagePictureBox.Image = default;
                    img.Dispose();
                }


                var image = new Bitmap(imageBox.Image.Width, imageBox.Image.Height);
                using(var g = Graphics.FromImage(image))
                {
                    g.DrawImage(imageBox.Image, 0, 0);
                }

                var tag = new Bitmap(imageBox.Image.Width, imageBox.Image.Height);
                using (var g = Graphics.FromImage(tag))
                {
                    g.DrawImage(imageBox.Image, 0, 0);
                }

                PreviewImagePictureBox.Tag = image;
                PreviewImagePictureBox.Image = tag;

                PreviewImagePictureBox.Visible = true;
                PreviewImageZoomValueChanged(this, EventArgs.Empty);
            }

            ApplySplitChanges();
        }

        protected virtual void ApplySplitChanges()
        {
            if (PreviewImagePictureBox.Image == default) { return; }

            var type = SplitComboBox.SelectedItem as SplitComboBoxItem;

            IsAutomaticChange = true;

            var tag = PreviewImagePictureBox.Tag as Image;
            var height = tag.Height;
            var width = tag.Width;

            SplitXNumericUpDown.Maximum = width;
            SplitYNumericUpDown.Maximum = height;

            IsAutomaticChange = false;

            var splitX = SplitXNumericUpDown.Value;
            var splitY = SplitYNumericUpDown.Value;

            if ((type.Type == SplitType.Piece && splitX == 1 && splitY == 1) || (type.Type == SplitType.Pixel && splitX == width && splitY == height))
            {
                var bmp = new Bitmap(width, height);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(tag, 0, 0);
                }

                var i = PreviewImagePictureBox.Image;
                PreviewImagePictureBox.Image = bmp;
                i.Dispose();

                return;
            }

            if (!PreviewImageDrawGridToolStripButton.Checked) { return; }

            var bitmap = new Bitmap(tag.Width, tag.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(tag, 0, 0);

                var w = type.Type == SplitType.Pixel ? (int)splitX : (int)Math.Ceiling((float)width / (float)splitX);
                var h = type.Type == SplitType.Pixel ? (int)splitY : (int)Math.Ceiling((float)height / (float)splitY);

                using (var pen = new Pen(Color.Magenta, 1))
                {
                    for (var x = w; x < width - 1; x += w)
                    {
                        g.DrawLine(pen, x, 0, x, height);
                    }

                    for (var y = h; y < height - 1; y += h)
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }

            var image = PreviewImagePictureBox.Image;
            PreviewImagePictureBox.Image = bitmap;
            image.Dispose();
        }

        protected virtual void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected virtual void ExportConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            var conf = new Request()
            {
                IgnoreTransparent = IgnoreTransparentCheckBox.Checked,

                Angle = (float)AngleNumericUpDown.Value,
                Color = FillColorPictureBox.BackColor,

                FlipType = (FlipComboBox.SelectedItem as FlipComboBoxItem).Type,

                OffsetX = (float)OffsetXNumericUpDown.Value,
                OffsetZ = (float)OffsetZNumericUpDown.Value,

                MinimumHeight = (float)HeightMinNumericUpDown.Value,
                MaximumHeight = (float)HeightMaxNumericUpDown.Value,

                ScaleX = (float)ScaleXNumericUpDown.Value,
                ScaleZ = (float)ScaleZNumericUpDown.Value,

                SplitType = (SplitComboBox.SelectedItem as SplitComboBoxItem).Type,
                SplitX = (int)SplitXNumericUpDown.Value,
                SplitY = (int)SplitYNumericUpDown.Value
            };

            try
            {
                ConfigurationSaveFileDialog.FileName = string.Empty;

                if (ConfigurationSaveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(ConfigurationSaveFileDialog.FileName, JsonConvert.SerializeObject(conf, Formatting.Indented), Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void FillColorPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = FillColorPictureBox.BackColor;

            if (MainColorDialog.ShowDialog(this) == DialogResult.OK)
            {
                FillColorPictureBox.BackColor = MainColorDialog.Color;

                ApplyChanges();
            }
        }

        protected virtual void FlipComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }
            if (OriginalImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual void ImportConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            ConfigurationOpenFileDialog.FileName = string.Empty;

            if (ConfigurationOpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var conf = JsonConvert.DeserializeObject<Request>(File.ReadAllText(ConfigurationOpenFileDialog.FileName, Encoding.UTF8));

                    IsAutomaticChange = true;

                    IgnoreTransparentCheckBox.Checked = conf.IgnoreTransparent;

                    AngleNumericUpDown.Value = (decimal)conf.Angle;

                    for (var x = 0; x < FlipComboBox.Items.Count; x++)
                    {
                        var item = FlipComboBox.Items[x] as FlipComboBoxItem;
                        if (item.Type == conf.FlipType)
                        {
                            FlipComboBox.SelectedIndex = x;
                        }
                    }

                    FillColorPictureBox.BackColor = conf.Color;

                    ScaleXNumericUpDown.Value = (decimal)conf.ScaleX;
                    ScaleZNumericUpDown.Value = (decimal)conf.ScaleZ;

                    HeightMinNumericUpDown.Value = (decimal)conf.MinimumHeight;
                    HeightMaxNumericUpDown.Value = (decimal)conf.MaximumHeight;

                    OffsetXNumericUpDown.Value = (decimal)conf.OffsetX;
                    OffsetZNumericUpDown.Value = (decimal)conf.OffsetZ;

                    for (var x = 0; x < SplitComboBox.Items.Count; x++)
                    {
                        var item = SplitComboBox.Items[x] as SplitComboBoxItem;
                        if (item.Type == conf.SplitType)
                        {
                            SplitComboBox.SelectedIndex = x;
                        }

                    }

                    SplitXNumericUpDown.Value = conf.SplitX;
                    SplitYNumericUpDown.Value = conf.SplitY;

                    IsAutomaticChange = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void MainWindowDragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != default && files.Length != 0)
            {
                try
                {
                    OpenImage(files[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void MainWindowDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        protected virtual void MainWindowLoad(object sender, EventArgs e)
        {
            Icon = Resources.Icon;

            IsAutomaticChange = true;

            FlipComboBox.Items.AddRange(new[]
            {
                new FlipComboBoxItem() { Name = "None", Type = FlipType.None },
                new FlipComboBoxItem() { Name = "Horizontal", Type = FlipType.Horizontal },
                new FlipComboBoxItem() { Name = "Vertical", Type = FlipType.Vertical },
                new FlipComboBoxItem() { Name = "Both", Type = FlipType.Both }
            });

            FlipComboBox.SelectedIndex = 0;

            SplitComboBox.Items.AddRange(new[]
            {
                new SplitComboBoxItem() { Name = "Pieces", Type = SplitType.Piece },
                new SplitComboBoxItem() { Name = "Pixels", Type = SplitType.Pixel }
            });

            SplitComboBox.SelectedIndex = 0;

            IsAutomaticChange = false;
        }

        protected virtual void ModifiedImageBrightestPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = ModifiedImageBrightestPictureBox.BackColor;
            MainColorDialog.ShowDialog(this);
        }

        protected virtual void ModifiedImageDarkestPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = ModifiedImageDarkestPictureBox.BackColor;
            MainColorDialog.ShowDialog(this);
        }

        protected virtual void OpenImage(string path)
        {
            OriginalImagePictureBox.Image = Image.FromFile(path);

            SaveImageAsToolStripMenuItem.Enabled = true;
            SaveObjAsToolStripMenuItem.Enabled = true;

            OriginalImagePictureBox.Visible = true;
            DragDropLabel.Visible = false;
            MainToolStripStatusLabel.Text = string.Format("File: {0} @ {1}x{2}", path, OriginalImagePictureBox.Image.Width, OriginalImagePictureBox.Image.Height);

            OriginalImageZoomValueChanged(this, EventArgs.Empty);

            ApplyChanges();
        }

        protected virtual void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (ImageOpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    OpenImage(ImageOpenFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void OriginalImageBrightestPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = OriginalImageBrightestPictureBox.BackColor;
            MainColorDialog.ShowDialog(this);
        }

        protected virtual void OriginalImageDarkestPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = OriginalImageDarkestPictureBox.BackColor;
            MainColorDialog.ShowDialog(this);
        }

        protected virtual void OriginalImageOpenToolStripButtonClick(object sender, EventArgs e)
        {
            if (ImageOpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    OpenImage(ImageOpenFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        protected virtual void OriginalImagePanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            OriginalImagePictureBox.Left = -e.NewValue;
        }

        protected virtual void OriginalImagePanelResize(object sender, EventArgs e)
        {
            if (OriginalImagePictureBox.Image == default) { return; }

            var point = new System.Drawing.Point(
                Math.Max(0, (OriginalImagePanel.Width - OriginalImagePictureBox.Width) / 2),
                Math.Max(0, (OriginalImagePanel.Height - OriginalImagePictureBox.Height) / 2));

            var extraWidth = OriginalImagePictureBox.Width - OriginalImagePanel.Width;
            var extraHeight = OriginalImagePictureBox.Height - OriginalImagePanel.Height;

            if (extraWidth > 0)
            {
                OriginalImagePanelHorizontalScrollBar.Maximum = OriginalImagePictureBox.Width - OriginalImagePanel.Width;
            }

            OriginalImagePanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                OriginalImagePanelVerticalScrollBar.Maximum = OriginalImagePictureBox.Height - OriginalImagePanel.Height;
            }

            OriginalImagePanelVerticalScrollBar.Visible = extraHeight > 0;

            OriginalImagePictureBox.Location = new System.Drawing.Point(point.X - (OriginalImagePanelHorizontalScrollBar.Visible ? OriginalImagePanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (OriginalImagePanelVerticalScrollBar.Visible ? OriginalImagePanelVerticalScrollBar.Value : 0));
        }

        protected virtual void OriginalImagePanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            OriginalImagePictureBox.Top = -e.NewValue;
        }

        protected virtual void OriginalImageZoom100ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 100;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom100ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = true;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom150ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 150;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom150ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = true;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom200ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 200;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom200ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = true;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom300ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 300;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom300ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = true;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom400ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 400;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom400ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = true;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom500ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 500;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom500ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = false;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = true;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoom50ToolStripMenuItemClick(object sender, EventArgs e)
        {
            OriginalImageZoomValue = 50;
            OriginalImageZoomToolStripDropDownButton.Text = OriginalImageZoom50ToolStripMenuItem.Text;

            OriginalImageZoom50ToolStripMenuItem.Checked = true;
            OriginalImageZoom100ToolStripMenuItem.Checked = false;
            OriginalImageZoom150ToolStripMenuItem.Checked = false;
            OriginalImageZoom200ToolStripMenuItem.Checked = false;
            OriginalImageZoom300ToolStripMenuItem.Checked = false;
            OriginalImageZoom400ToolStripMenuItem.Checked = false;
            OriginalImageZoom500ToolStripMenuItem.Checked = false;

            OriginalImageZoomValueChanged(sender, e);
        }

        protected virtual void OriginalImageZoomValueChanged(object sender, EventArgs e)
        {
            OriginalImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (OriginalImagePictureBox.Image != default)
            {
                OriginalImagePictureBox.Width = (int)Math.Ceiling(OriginalImagePictureBox.Image.Width * OriginalImageZoomValue / 100);
                OriginalImagePictureBox.Height = (int)Math.Ceiling(OriginalImagePictureBox.Image.Height * OriginalImageZoomValue / 100);

                OriginalImagePanelResize(sender, e);
            }
        }

        protected virtual void PreviewImageDrawGridToolStripButtonCheckedChanged(object sender, EventArgs e)
        {
            if(PreviewImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual void PreviewImagePanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            PreviewImagePictureBox.Left = -e.NewValue;
        }

        protected virtual void PreviewImagePanelResize(object sender, EventArgs e)
        {
            if (PreviewImagePictureBox.Image == default) { return; }

            var point = new System.Drawing.Point(
                Math.Max(0, (PreviewImagePanel.Width - PreviewImagePictureBox.Width) / 2),
                Math.Max(0, (PreviewImagePanel.Height - PreviewImagePictureBox.Height) / 2));

            var extraWidth = PreviewImagePictureBox.Width - PreviewImagePanel.Width;
            var extraHeight = PreviewImagePictureBox.Height - PreviewImagePanel.Height;

            if (extraWidth > 0)
            {
                PreviewImagePanelHorizontalScrollBar.Maximum = PreviewImagePictureBox.Width - PreviewImagePanel.Width;
            }

            PreviewImagePanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                PreviewImagePanelVerticalScrollBar.Maximum = PreviewImagePictureBox.Height - PreviewImagePanel.Height;
            }

            PreviewImagePanelVerticalScrollBar.Visible = extraHeight > 0;

            PreviewImagePictureBox.Location = new System.Drawing.Point(point.X - (PreviewImagePanelHorizontalScrollBar.Visible ? PreviewImagePanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (PreviewImagePanelVerticalScrollBar.Visible ? PreviewImagePanelVerticalScrollBar.Value : 0));
        }

        protected virtual void PreviewImagePanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            PreviewImagePictureBox.Top = -e.NewValue;
        }

        protected virtual void PreviewImageSaveImageToolStripButtonClick(object sender, EventArgs e)
        {
            ImageSaveFileDialog.FileName = string.Empty;

            if (PreviewImagePictureBox.Image == default) { return; }

            if (ImageSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveImage(ImageSaveFileDialog.FileName);
            }
        }

        protected virtual void PreviewImageSaveObjToolStripButtonClick(object sender, EventArgs e)
        {
            if (PreviewImagePictureBox.Image == default) { return; }

            ObjSaveFileDialog.FileName = string.Empty;

            if (ObjSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveObj(ObjSaveFileDialog.FileName);
            }
        }

        protected virtual void PreviewImageZoom100ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 100;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom100ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = true;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom150ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 150;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom150ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = true;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom200ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 200;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom200ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = true;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom300ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 300;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom300ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = true;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom400ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 400;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom400ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = true;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom500ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 500;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom500ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = false;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = true;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoom50ToolStripMenuItemClick(object sender, EventArgs e)
        {
            PreviewImageZoomValue = 50;
            PreviewImageZoomToolStripDropDownButton.Text = PreviewImageZoom50ToolStripMenuItem.Text;

            PreviewImageZoom50ToolStripMenuItem.Checked = true;
            PreviewImageZoom100ToolStripMenuItem.Checked = false;
            PreviewImageZoom150ToolStripMenuItem.Checked = false;
            PreviewImageZoom200ToolStripMenuItem.Checked = false;
            PreviewImageZoom300ToolStripMenuItem.Checked = false;
            PreviewImageZoom400ToolStripMenuItem.Checked = false;
            PreviewImageZoom500ToolStripMenuItem.Checked = false;

            PreviewImageZoomValueChanged(sender, e);
        }

        protected virtual void PreviewImageZoomValueChanged(object sender, EventArgs e)
        {
            PreviewImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (PreviewImagePictureBox.Image != default)
            {
                PreviewImagePictureBox.Width = (int)Math.Ceiling(PreviewImagePictureBox.Image.Width * PreviewImageZoomValue / 100);
                PreviewImagePictureBox.Height = (int)Math.Ceiling(PreviewImagePictureBox.Image.Height * PreviewImageZoomValue / 100);

                PreviewImagePanelResize(sender, e);
            }
        }

        protected virtual void ReportAnIssueToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Triangulator/issues") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }
        protected virtual void SaveImage(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant().Replace(".", string.Empty);

            if (!TryParseImageFormat(extension, out var imageFormatValue))
            {
                MessageBox.Show(this, string.Format("Image extension <{0}> is not supported. Saving image as a BMP.", extension), "ImageBox", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var outputFileName = imageFormatValue != default ? fileName : (fileName + ".bmp");

            try
            {
                var split = SplitImage(PreviewImagePictureBox.Tag as Image);

                if (split == default) { return; }

                // Single
                if (split.Length == 1 && split[0].Length == 1)
                {
                    split[0][0].Image.Save(outputFileName, imageFormatValue ?? ImageFormat.Bmp);
                    return;
                }

                // Multiple
                var fileNameDirectory = Path.GetDirectoryName(fileName);
                var fileNameTemplate = Path.GetFileNameWithoutExtension(fileName);

                for (var x = 0; x < split.Length; x++)
                {
                    var line = split[x];

                    for (var y = 0; y < line.Length; y++)
                    {
                        line[y].Image.Save(Path.Combine(fileNameDirectory, string.Format("{0}.{1:D4}.{2:D4}.{3}", fileNameTemplate, y, x, extension.ToLowerInvariant())), imageFormatValue ?? ImageFormat.Bmp);
                        line[y].Image.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void SaveImageAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImageSaveFileDialog.FileName = string.Empty;

            if (PreviewImagePictureBox.Image == default) { return; }

            if (ImageSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveImage(ImageSaveFileDialog.FileName);
            }
        }

        protected virtual void SaveObj(string fileName)
        {
            var image = OriginalImagePictureBox.Image;

            var triangulator = new Triangulator();

            try
            {
                var objs = triangulator.Triangulate(image, new Request()
                {
                    IgnoreTransparent = IgnoreTransparentCheckBox.Checked,

                    OffsetX = (float)OffsetXNumericUpDown.Value,
                    OffsetZ = (float)OffsetZNumericUpDown.Value,

                    MinimumHeight = (float)HeightMinNumericUpDown.Value,
                    MaximumHeight = (float)HeightMaxNumericUpDown.Value,

                    ScaleX = (float)ScaleXNumericUpDown.Value,
                    ScaleZ = (float)ScaleZNumericUpDown.Value,

                    SplitType = (SplitComboBox.SelectedItem as SplitComboBoxItem).Type,
                    SplitX = (int)SplitXNumericUpDown.Value,
                    SplitY = (int)SplitYNumericUpDown.Value,

                    Angle = (float)AngleNumericUpDown.Value,
                    Color = FillColorPictureBox.BackColor,
                    FlipType = (FlipComboBox.SelectedItem as FlipComboBoxItem).Type
                });

                // Single
                if (objs.Length == 1 && objs[0].Length == 1)
                {
                    ObjFile.Write(objs[0][0], fileName);
                    return;
                }

                // Multiple
                var fileNameDirectory = Path.GetDirectoryName(fileName);
                var fileNameTemplate = Path.GetFileNameWithoutExtension(fileName);

                for (var x = 0; x < objs.Length; x++)
                {
                    var line = objs[x];

                    for (var y = 0; y < line.Length; y++)
                    {
                        ObjFile.Write(line[y], Path.Combine(fileNameDirectory, string.Format("{0}.{1:D4}.{2:D4}.obj", fileNameTemplate, y, x)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Triangulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void SaveObjAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (PreviewImagePictureBox.Image == default) { return; }

            ObjSaveFileDialog.FileName = string.Empty;

            if (ObjSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveObj(ObjSaveFileDialog.FileName);
            }
        }

        protected virtual void SplitComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }
            if (OriginalImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual SplitterResult[][] SplitImage(Image image)
        {
            if (image == default) { return default; }

            var type = SplitComboBox.SelectedItem as SplitComboBoxItem;

            var splitX = (int)SplitXNumericUpDown.Value;
            var splitY = (int)SplitYNumericUpDown.Value;

            // Single
            if ((type.Type == SplitType.Piece && splitX == 1 && splitY == 1) || (type.Type == SplitType.Pixel && splitX == image.Width && splitY == image.Height))
            {
                return new SplitterResult[][] { new SplitterResult[] { new SplitterResult() { Image = image } } };
            }

            // Multiple
            return new Splitter(image).Split(new SplitterRequest()
            {
                Type = type.Type,
                Horizontal = splitX,
                Vertical = splitY
            });
        }

        protected virtual void SplitXNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }
            if (OriginalImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual void SplitYNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }
            if (OriginalImagePictureBox.Image == default) { return; }

            ApplyChanges();
        }

        protected virtual bool TryParseImageFormat(string format, out ImageFormat imageFormat)
        {
            var extension = format.ToLowerInvariant().Replace("ico", "icon").Replace("jpg", "jpeg").Replace("tif", "tiff");

            var imageFormatProperty = typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                                                            .FirstOrDefault(p => p.Name.ToLowerInvariant() == extension);

            imageFormat = imageFormatProperty == default ? default : (ImageFormat)imageFormatProperty.GetValue(default, default);

            return imageFormat != default;
        }
        protected virtual void VisitWebsiteToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Triangulator") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }
    }
}
