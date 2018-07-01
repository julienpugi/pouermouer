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
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace PanneauLed
{
    public class ImgTree
    {
        List<GifImage> imgList = new List<GifImage> { };

        public int getSize()
        {
            return imgList.Count;
        }

        public GifImage GetImg(int index)
        {
            return imgList.ElementAt(index);
        }

        public void AddImage(String path, int height, int width)
        {
            Form myForm = new Form1();
            imgList.Add(new GifImage(path,height,width,false));
            Console.WriteLine("taille" + imgList.Count);
        }

        public void RemoveImage(TreeNode node)
        {
            Console.WriteLine("index" + node.Index);
            imgList.RemoveAt(node.Index);
        }

        public void AddText(int height, int width)
        {
            imgList.Add(new GifImage("null", height, width, true));
        }

        public void RemoveText()
        {

        }

        public List<GifImage> getList()
        {
            return imgList;
        }

        public void SetSelected(int index, bool state)
        {
            for (int i = 0; i < imgList.Count; i++)
            {
                if (imgList.ElementAt(i).GetSelection() == true)
                {
                    imgList.ElementAt(i).SetSelection(false);
                }
            }
            GetImg(index).SetSelection(state);
        }

        public GifImage GetSelected()
        {
            for (int i = 0; i < imgList.Count; i++)
            {
                if (imgList.ElementAt(i).GetSelection() == true)
                {
                    return imgList.ElementAt(i);
                }
            }
            return null;

        }


    }

    public class Pixel
    {
        private int pixelCoordinateX;
        private int pixelCoordinateY;
        private byte pixelValue;

        public Pixel(int X, int Y, byte val)
        {
            pixelCoordinateX = X;
            pixelCoordinateY = Y;
            pixelValue = val;
        }

        public byte GetPixelValue()
        {
            return this.pixelValue;
        }
    }

    public class GifImage
    {
        private Image gifImage;
        private byte [,] gifBytes;
        private FrameDimension dimension;
        
        private int screenHeight;
        private int screenWidth;
        private int imgHeight;
        private int imgWidth;
        private int frameCount;
        private int currentFrame = -1;
        private bool reverse;
        private int step = 1;
        private int Xoffset = 0;
        private int Yoffset= 0;
        private int XanimOffset = 0;
        private int YanimOffset = 0;
        private int cycles = 5;
        private int speed = 5;
        private int type = 0;
        private bool isText = false;
        private bool isSelected = false;

        //Text parameters
        private Font textFont = new Font("Arial", 10, FontStyle.Regular);
        private string text = "texte";
        bool antialias = false;

        public GifImage(string path, int height, int width, bool isText)
        {
            
            this.isText = isText;
            
            if (this.isText)
            {
                gifImage = (Image)ImgFromText(text, textFont);
            }
            else
            {
                
                gifImage = Image.FromFile(path); //initialize
            }
            imgHeight = gifImage.Height;
            imgWidth = gifImage.Width;
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation
            screenHeight = height/10;
            screenWidth = width/10;
            gifBytes = Array2DFromBitmap(new Bitmap(gifImage));
            Console.WriteLine("gifbyte length: " + gifBytes.Length);

        }

        private void updateText()
        {
            gifImage = (Image)ImgFromText(text, textFont);
            imgHeight = gifImage.Height;
            imgWidth = gifImage.Width;
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation
            gifBytes = Array2DFromBitmap(new Bitmap(gifImage));
            Console.WriteLine("gifbyte length: " + gifBytes.Length);
        }

        private Image ImgFromText(string text, Font fontName)
        {
            Console.WriteLine("c'est du texte !");
            Bitmap objBmpImage = new Bitmap(1, 1);

            // Create the Font object for the image text drawing.
            Font objFont = fontName; //new Font(fontName, 10, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);
            
            // This is where the bitmap size is determined.
            int intWidth = (int)objGraphics.MeasureString(text, objFont).Width;
            int intHeight = (int)objGraphics.MeasureString(text, objFont).Height;
            Console.WriteLine("textWidth: " + intWidth + ", textHeight: " + intHeight);
            /*intWidth = 64;
            intHeight = 16;*/

            // Create the bmpImage again with the correct size for the text and font.
            if (intWidth <= 0)
            {
                objBmpImage = new Bitmap(objBmpImage, new Size(1, 1));
            }
            else
            {
                objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));
            }
            

                // Add the colors to the new bitmap.
             objGraphics = Graphics.FromImage(objBmpImage);
  
            // Set Background color
            objGraphics.Clear(Color.White);
            if (antialias)
            {
                objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
            else
            {
                objGraphics.SmoothingMode = SmoothingMode.None;
                objGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            }
            
            objGraphics.DrawString(text, objFont, new SolidBrush(Color.FromArgb(0, 0, 0)), 0, 0);
            objGraphics.Flush();

            return (objBmpImage);
        }

        public void setText(string text)
        {
            if (this.isText)
            {
                this.text = text;
                updateText();
            }
            
        }

        public void setFontSize(float size)
        {
            if (this.isText)
            {
                textFont = new Font(textFont.Name, size, textFont.Style);
                updateText();
            }   
        }

        public void setAntialiasing(bool isEnabled)
        {
            antialias = isEnabled;
            updateText();
        }

        public void setFont(Font font)
        {
            if (this.isText)
            {
                textFont = font;
                updateText();
            }
                
        }

        public int GetImgHeight()
        {
            return imgHeight;
        }

        public bool GetIsText()
        {
            return isText;
        }

        public bool GetAntialiasing()
        {
            return antialias;
        }

        public int GetImgWidth()
        {
            return imgWidth;
        }

        public int GetXoffset()
        {
            return Xoffset+ XanimOffset;
        }

        public int GetYoffset()
        {
            return Yoffset+YanimOffset;
        }

        public void SetXoffset(int pos)
        {
            Xoffset = pos;
        }

        public void SetYoffset(int pos)
        {
            Yoffset = pos;
        }

        public void animStep(bool reset)
            
        {
            
            switch (type)
            {
                case 1:
                    if (reset)
                    {
                        XanimOffset = imgWidth * -1;
                    }
                    if (XanimOffset < screenWidth)
                    {
                        XanimOffset++;
                    }
                    else
                    {
                        XanimOffset = imgWidth * -1;
                    }
                    
                    break;
                case 2:
                    if (reset)
                    {
                        XanimOffset = screenWidth;
                    }
                    if (XanimOffset > imgWidth * -1)
                    {
                        XanimOffset += (-1);
                    }
                    else
                    {
                        XanimOffset = screenWidth;
                    }
                    break;
                 default:
                    //Console.WriteLine("Default case");
                    break;
            }
        }

        public byte[,] GetByteArray()
        {
            return gifBytes;

        }
            
        private byte[,] Array2DFromBitmap(Bitmap bmp)
        {
            byte[,] bytes = new byte [bmp.Width,bmp.Height]; ;
            for (int i= 0; i<bmp.Width; i++)
            {
                for (int j=0; j< bmp.Height; j++)
                {
                    bytes[i, j] = bmp.GetPixel(i, j).R;
                }
                
            }
            return bytes;
        }

        public int[] GetProperties()
        {
            return new int[] { Xoffset, Yoffset, cycles, speed, type };
        }

        public string GetText()
        {
            return text;
        }
        public Font GetFont()
        {
            return textFont;
        }
        public float GetFontSize()
        {
            return textFont.Size;
        }

        public void SetProperties(int[] properties)
        {
            Xoffset = properties[0];
            Yoffset = properties[1];
            cycles = properties[2];
            speed = properties[3];
            type = properties[4];
        }

        

        public void SetSelection(bool state)
        {
            isSelected = state;
        }

        public bool GetSelection()
        {
            return isSelected;
        }


        public byte[] GenerateCode(byte imgId, byte animType, byte speed, byte loops, byte nextanimImg)
        {

            Pixel[,] array = this.GetPixelArray(0);
            var imgCode = new List<byte>
            {
                imgId,
                animType,
                speed,
                loops,
                nextanimImg
            };

            Boolean init = true;
            byte prevValue = 0;
            byte sameValIncrement = 0;
            for (int Xcount = 0; Xcount < array.GetLength(0); Xcount++)
            {
                for (int Ycount = 0; Ycount < array.GetLength(1); Ycount++)
                {

                    if (init)
                    {
                        prevValue = array[Xcount, Ycount].GetPixelValue();
                        imgCode.Add(prevValue);
                        init = false;
                    }
                    else
                    {
                        if (array[Xcount, Ycount].GetPixelValue() == prevValue)
                        {
                            if (sameValIncrement < 200)
                            {
                                sameValIncrement++;
                            }
                            else
                            {
                                imgCode.Add(sameValIncrement);
                                sameValIncrement = 0;
                            }

                        }
                        else
                        {
                            prevValue = array[Xcount, Ycount].GetPixelValue();
                            if (sameValIncrement == 0)
                            {
                                imgCode.Add(prevValue);
                            }
                            else
                            {
                                imgCode.Add(sameValIncrement);
                                imgCode.Add(prevValue);
                                sameValIncrement = 0;
                            }
                        }
                    }

                }
            }
            return imgCode.ToArray();
        }


        public Pixel[,] GetPixelArray(int frame)
        {
            Pixel[,] pixelArray = new Pixel[64, 16];
            Image tempImg = this.GetFrame(frame);
            Bitmap tempBmp = new Bitmap(tempImg);
            Console.WriteLine(tempBmp.Width);
            for (int Xcount = 0; Xcount < tempBmp.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < tempBmp.Height; Ycount++)
                {
                    pixelArray[Xcount, Ycount] = new Pixel(Xcount, Ycount, tempBmp.GetPixel(Xcount, Ycount).R);
                }
            }
            return pixelArray;
        }

        public bool ReverseAtEnd //whether the gif should play backwards when it reaches the end
        {
            get { return reverse; }
            set { reverse = value; }
        }

        public Image GetNextFrame()
        {

            currentFrame += step;

            //if the animation reaches a boundary...
            if (currentFrame >= frameCount || currentFrame < 1)
            {
                if (reverse)
                {
                    step *= -1; //...reverse the count
                    currentFrame += step; //apply it
                }
                else
                    currentFrame = 0; //...or start over
            }
            return GetFrame(currentFrame);
        }

        public Image GetFrame(int index)
        {
            gifImage.SelectActiveFrame(dimension, index); //find the frame
            return (Image)gifImage.Clone(); //return a copy of it
        }
    }

}