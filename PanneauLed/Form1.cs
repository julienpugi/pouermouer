using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace PanneauLed
{
    public partial class Form1 : Form
    {
        GifImage loadedGif;
        ImgTree componentsTree = new ImgTree();
        bool timerState = false;


        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 30;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //Console.WriteLine(comboBoxType.SelectedItem.ToString());
        }
        private void treeView1_drawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Color foreColor = Color.Black;
            if (e.Node.IsSelected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                foreColor = Color.White;
            }
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            TextRenderer.DrawText(e.Graphics,
                                   e.Node.Text,
                                   e.Node.TreeView.Font,
                                   e.Node.Bounds,
                                   foreColor,
                                   TextFormatFlags.GlyphOverhangPadding);
        }




        //-------------------------------------------------------------------------------------
        // CONTROL UPDATING

        private void updateTree(TreeView tree, string action, TreeNode node)
        {
            tree.BeginUpdate();
            if (action == "add")
            {

                tree.Nodes.Add(node);   //(Path.GetFileName(path));
                treeView1.SelectedNode = node;
                componentsTree.SetSelected(treeView1.SelectedNode.Index, true);
                setAllAnimParameters(true);
                comboBoxType.Text = "Fixe";

                if (componentsTree.GetSelected().GetIsText())
                {
                    setAllTextParameters(true);

                }
                updateParameters();
            }
            else if (action == "remove")
            {
                tree.Nodes.Remove(node);
                updateParameters();
            }

            tree.EndUpdate();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Console.WriteLine("selected");
            componentsTree.SetSelected(treeView1.SelectedNode.Index, true);
            updateParameters();
            Button_deleteImg.Show();
        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            componentsTree.SetSelected(treeView1.SelectedNode.Index, false);
            Button_deleteImg.Hide();
            //numericUpDownX.Enabled = false;
            treeView1.SelectedNode = null;
            //treeView1.SelectedNode = null;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("A");
            updateProperties();
        }

        private void numericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("B");
            updateProperties();
        }

        
        private void numericUpDownY_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("C");
            updateProperties();
        }
        private void numericUpDownCycles_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("D");
            updateProperties();
        }
        private void numericUpDownSpeed_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("E");
            updateProperties();
        }

        private void TextEnterBox_TextChanged(object sender, EventArgs e)
        {
            GifImage selectedComponent = componentsTree.GetSelected();
            if (selectedComponent != null)
            {
                selectedComponent.setText(TextEnterBox.Text);
                updateScreen(mainScreen);
            }
        }
        private void numericUpDownFontSize_ValueChanged(object sender, EventArgs e)
        {
            GifImage selectedComponent = componentsTree.GetSelected();
            if (selectedComponent != null)
            {
                selectedComponent.setFontSize((float)numericUpDownFontSize.Value);
                Console.WriteLine("SETTING FONT SIZE :" + (float)numericUpDownFontSize.Value);
                updateScreen(mainScreen);
            }
        }
        private void Button_searchFont_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;

            fontDialog1.Font = FontTextBox.Font;
            fontDialog1.Color = FontTextBox.ForeColor;

            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {

                GifImage selectedComponent = componentsTree.GetSelected();
                if (selectedComponent != null)
                {
                    selectedComponent.setFont(fontDialog1.Font);
                    updateScreen(mainScreen);
                }

                //FontTextBox.Font = fontDialog1.Font;
                FontTextBox.Text = fontDialog1.Font.Name;
                //FontTextBox.ForeColor = fontDialog1.Color;
            }
        }

        private void antialiasingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GifImage selectedComponent = componentsTree.GetSelected();
            if (selectedComponent != null)
            {
                selectedComponent.setAntialiasing(antialiasingCheckBox.Checked);
                updateScreen(mainScreen);
            }
        }

        public void updateProperties()
        {
            if (componentsTree.GetSelected() != null)
            {
                componentsTree.GetSelected().SetProperties(new int[] { Convert.ToInt32(numericUpDownX.Value), Convert.ToInt32(numericUpDownY.Value), Convert.ToInt32(numericUpDownCycles.Value), Convert.ToInt32(numericUpDownSpeed.Value), Convert.ToInt32(comboBoxType.SelectedIndex) });

                updateScreen(mainScreen);
            }
            
        }

        private void setAllTextParameters(bool isEnabled)
        {
            TextBarSeparator.Enabled = isEnabled;
            labelText.Enabled = isEnabled;
            FontTextBox.Enabled = isEnabled;
            FontLabel.Enabled = isEnabled;
            TextEnterBox.Enabled = isEnabled;
            SearchFontBtn.Enabled = isEnabled;
            SizeLabel.Enabled = isEnabled;
            numericUpDownFontSize.Enabled = isEnabled;
            antialiasingCheckBox.Enabled = isEnabled;

            //Sets visibility
            TextBarSeparator.Visible = isEnabled;
            labelText.Visible = isEnabled;
            FontTextBox.Visible = isEnabled;
            FontLabel.Visible = isEnabled;
            TextEnterBox.Visible = isEnabled;
            SearchFontBtn.Visible = isEnabled;
            SizeLabel.Visible = isEnabled;
            numericUpDownFontSize.Visible = isEnabled;
            antialiasingCheckBox.Visible = isEnabled;
        }

        private void unbindAnimControlChangeValueEvents()
        {
            numericUpDownX.ValueChanged -= numericUpDownX_ValueChanged;
            numericUpDownY.ValueChanged -= numericUpDownY_ValueChanged;
            numericUpDownSpeed.ValueChanged -= numericUpDownSpeed_ValueChanged;
            numericUpDownCycles.ValueChanged -= numericUpDownCycles_ValueChanged;
            comboBoxType.SelectedIndexChanged -= comboBoxType_SelectedIndexChanged;
        }
        private void bindAnimControlChangeValueEvents()
        {
            numericUpDownX.ValueChanged += numericUpDownX_ValueChanged;
            numericUpDownY.ValueChanged += numericUpDownY_ValueChanged;
            numericUpDownSpeed.ValueChanged += numericUpDownSpeed_ValueChanged;
            numericUpDownCycles.ValueChanged += numericUpDownCycles_ValueChanged;
            comboBoxType.SelectedIndexChanged += comboBoxType_SelectedIndexChanged;
        }
        private void unbindTextControlChangeValueEvents()
        {
            TextEnterBox.TextChanged -= TextEnterBox_TextChanged;
            numericUpDownFontSize.ValueChanged -= numericUpDownFontSize_ValueChanged;
            antialiasingCheckBox.CheckedChanged -= antialiasingCheckBox_CheckedChanged;
        }
        private void bindTextControlChangeValueEvents()
        {
            TextEnterBox.TextChanged += TextEnterBox_TextChanged;
            numericUpDownFontSize.ValueChanged += numericUpDownFontSize_ValueChanged;
            antialiasingCheckBox.CheckedChanged += antialiasingCheckBox_CheckedChanged;
        }

        private void setAllAnimParameters(bool isEnabled)
        {
            numericUpDownX.Enabled = isEnabled;
            comboBoxType.Enabled = isEnabled;
            numericUpDownY.Enabled = isEnabled;
            numericUpDownSpeed.Enabled = isEnabled;
            numericUpDownCycles.Enabled = isEnabled;
            if (isEnabled)
            {
                updateParameters();
            }
            else
            {

                unbindAnimControlChangeValueEvents();
                comboBoxType.Text = "";
                numericUpDownX.Value = 0;
                numericUpDownY.Value = 0;
                numericUpDownSpeed.Value = 0;
                numericUpDownCycles.Value = 0;
                bindAnimControlChangeValueEvents();



            }
            

        }

        private void updateParameters()
        {
            //Console.Write("update param");
            GifImage selectedComponent = componentsTree.GetSelected();
            if (selectedComponent != null)
            {
                int[] tempValues = selectedComponent.GetProperties();

                unbindAnimControlChangeValueEvents();

                numericUpDownX.Value = tempValues[0];
                numericUpDownY.Value = tempValues[1];
                numericUpDownCycles.Value = tempValues[2];
                numericUpDownSpeed.Value = tempValues[3];
                comboBoxType.SelectedIndex = tempValues[4];

                bindAnimControlChangeValueEvents();

                unbindTextControlChangeValueEvents();

                FontTextBox.Text = selectedComponent.GetFont().Name;
                TextEnterBox.Text = selectedComponent.GetText();
                numericUpDownFontSize.Value = (decimal)selectedComponent.GetFontSize();
                antialiasingCheckBox.Checked = selectedComponent.GetAntialiasing();

                bindTextControlChangeValueEvents();
            }
            else
            {
                setAllAnimParameters(false);
                setAllTextParameters(false);
                //comboBoxType.Text = "Fixe";
            }
            
            //numericUpDownX
        }

        private void updateScreen(PictureBox screen)
        {

            byte[,] screenImgBytes = new byte[640, 160];

            for (int imgCount = 0; imgCount < componentsTree.getSize(); imgCount++)
            {
                GifImage imgRef = componentsTree.GetImg(imgCount);
                byte[,] baseImg = imgRef.GetByteArray();

                int ScreenXoffset = 0;
                int ScreenYoffset = 0;
                int ImgXoffset = 0;
                int ImgYoffset = 0;
                int Xcount = 0;
                int Ycount = 0;
                int imgRefHeight = imgRef.GetImgHeight();
                int imgRefWidth = imgRef.GetImgWidth();

                if (imgRef.GetXoffset() >= 0)
                {
                    ScreenXoffset = imgRef.GetXoffset() * 10;
                }
                else
                {
                    ImgXoffset = imgRef.GetXoffset() * -10;
                }

                if (imgRef.GetYoffset() >= 0)
                {
                    ScreenYoffset = imgRef.GetYoffset() * 10;
                }
                else
                {
                    ImgYoffset = imgRef.GetYoffset() * -10;
                }



                for (int XcountScreen = ScreenXoffset; XcountScreen < screenImgBytes.GetLength(0); XcountScreen++)

                {
                    if (XcountScreen >= (imgRefWidth * 10) + (imgRef.GetXoffset() * 10))
                    {
                        //Console.WriteLine("STOPWidth");
                        break;
                    }
                    for (int YcountScreen = ScreenYoffset; YcountScreen < screenImgBytes.GetLength(1); YcountScreen++)
                    {
                        if (YcountScreen >= (imgRefHeight * 10) + (imgRef.GetYoffset() * 10))
                        {
                            //Console.WriteLine("STOPHeight");
                            break;
                        }
                        //Console.WriteLine("xcount : "+ Xcount + ", imgXoffset : "+ ImgXoffset+ "YcountScreen : " + YcountScreen+ "ImgYoffset : "+ ImgYoffset);
                        byte tempPixelValue = Convert.ToByte(((baseImg[(Xcount + ImgXoffset) / 10, (Ycount + ImgYoffset) / 10]) - 255) * -1);

                        if (screenImgBytes[XcountScreen, YcountScreen] < tempPixelValue)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    screenImgBytes[XcountScreen + j, YcountScreen + i] = tempPixelValue;
                                }


                            }
                        }

                        YcountScreen += 9;
                        Ycount += 10;

                    }
                    XcountScreen += 9;
                    Xcount += 10;
                    Ycount = 0;

                }
                Console.WriteLine("Xoffset:" + (ImgXoffset));
            }
            Console.WriteLine("array length: " + screenImgBytes.Length);
            screen.Image = ArrayToBitmap(screenImgBytes);
        }

        //-------------------------------------------------------------------------------------
        // CONTENT ADDING / REMOVING


        private void Button_addText_Click(object sender, EventArgs e)
        {
            componentsTree.AddText(mainScreen.Height, mainScreen.Width);
            updateTree(treeView1, "add", new TreeNode("test"));
            updateScreen(mainScreen);
        }

        

        private void Button_addImg_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {

                    componentsTree.AddImage(file,mainScreen.Height,mainScreen.Width);
                    updateTree(treeView1, "add", new TreeNode(Path.GetFileName(file)));
                    
                    updateScreen(mainScreen);

                }
                catch (IOException)
                {
                }
            }

            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }


        private void Button_deleteImg_Click(object sender, EventArgs e)
        {
            componentsTree.RemoveImage(treeView1.SelectedNode);
            updateTree(treeView1, "remove", treeView1.SelectedNode);
            updateScreen(mainScreen);
            if (treeView1.GetNodeCount(true) == 0)
            {
                Button_deleteImg.Hide();
                treeView1.SelectedNode = null;
            }
        }




        

        private Bitmap ArrayToBitmap(byte[,] array)
        {
            int ImWidth = array.GetLength(0);
            int ImHeight = array.GetLength(1);

            byte[] byteIn = new byte[ImWidth * ImHeight*3];
            int k = 2;
            for (int j = 0; j < ImHeight; j++)
            {
                for (int i = 0; i < ImWidth; i++)
                {
                    byteIn[k] = array[i, j];
                    //Console.WriteLine(array[i, j]);
                    k+=3;
                }
            }
            Bitmap tempBmp = new Bitmap(ImWidth, ImHeight, PixelFormat.Format24bppRgb);
            BitmapData bmpData = tempBmp.LockBits(new Rectangle(0, 0, ImWidth, ImHeight), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            Int32 psize = bmpData.Stride * ImHeight;
            Console.WriteLine("psize: "+ ptr);
            System.Runtime.InteropServices.Marshal.Copy(byteIn, 0, ptr, psize);
            tempBmp.UnlockBits(bmpData);
            return tempBmp;
        }




        //-------------------------------------------------------------------------------------
        // ANIMATION

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("tick");
            for (int imgCount = 0; imgCount < componentsTree.getSize(); imgCount++)
            {
                GifImage imgRef = componentsTree.GetImg(imgCount);
                imgRef.animStep(false);
                updateScreen(mainScreen);
            }
        }

        private void playAnimButton_Click(object sender, EventArgs e)
        {
            if (!timerState)
            {
                for (int imgCount = 0; imgCount < componentsTree.getSize(); imgCount++)
                {
                    GifImage imgRef = componentsTree.GetImg(imgCount);
                    imgRef.animStep(true);
                    updateScreen(mainScreen);
                }
                timer1.Start();
            }
            else
            {
                timer1.Stop();
            }
            
        }

        //-------------------------------------------------------------------------------------
        // ARDUINO ENCODING

        private void GenerateCodeBtn_Click(object sender, EventArgs e)
        {
            Pixel[,] pixArray = loadedGif.GetPixelArray(0);

            for (int Xcount = 0; Xcount < 64; Xcount++)
            {
                for (int Ycount = 0; Ycount < 16; Ycount++)
                {
                    Console.WriteLine(pixArray[Xcount, Ycount].GetPixelValue());
                }
            }

        }

        private void SaveAnimBtn_Click(object sender, EventArgs e)
        {
            byte[] array = loadedGif.GenerateCode(0, 0, 0, 0, 0);
            int count = 0;
            for (int Xcount = 0; Xcount < array.Length; Xcount++)
            {
                Console.WriteLine(array[Xcount]);
                count++;
            }
            Console.WriteLine("total byte nbr :");
            Console.WriteLine(count);
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    //string line = "";
                    using (StreamWriter sw = new StreamWriter(openFileDialog1.FileName))
                    {
                        for (int Xcount = 0; Xcount < array.Length; Xcount++)
                        {

                        }
                        /*WriteAsync(Char)
                        while ((line = sw.WriteLine()) != null)
                        {
                            Console.WriteLine(line);
                        }*/
                    }
                }
                catch (IOException)
                {
                }
            }


        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }

    


    

}
