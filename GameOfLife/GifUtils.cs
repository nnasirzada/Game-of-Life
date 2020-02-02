using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    /* Some parts of the code has been taken from following source: https://github.com/TASVideos/BizHawk/blob/master/BizHawk.Client.EmuHawk/AVOut/GifWriter.cs#L161 
       I didn't just copy paste entire code. I've made adjustments to make the code compatible with my solution.
    */
    public static class GifUtils
    {
        /* GIF protocol constants 
           33 - extension introducer
           255 - application extension
           11 - size of block
           78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48 - NETSCAPE2.0
           3 - Size of block
           1, 0, 0 - Block terminator
        */
        private static readonly byte[] GifAnimation = { 33 , 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
        /* 
            50 - Delay time low byte, 1/100th of a second
            0 - Delay time high byte
        */
        private static readonly byte[] Delay = { 50, 0 };
        /* frames - list of byte arrays. Each array represents an image.
           filePath - destination of exported GIF file.
           This method creates an animated GIF file.
        */
        public static void ExportGif(List<byte[]> frames, string filePath)
        {
            MemoryStream MS = new MemoryStream();
            BinaryReader BR = new BinaryReader(MS);
            BinaryWriter BW = new BinaryWriter(new FileStream(filePath, FileMode.Create));

            byte[] B = frames[0];
            B[10] = (byte)(B[10] & 0X78); //No global color table.
            BW.Write(B, 0, 13);
            BW.Write(GifAnimation);
            WriteGifImage(B, BW);
            for (int i = 1; i < frames.Count; i++)
            {
                MS.SetLength(0);
                B = frames[i];
                WriteGifImage(B, BW);
            }
            BW.Write(B[B.Length - 1]);
            BW.Close();
            MS.Dispose();
        }
        /* B - a frame 
           Writes given byte array - B to the GIF image using binary writer.
        */
        public static void WriteGifImage(byte[] B, BinaryWriter BW)
        {
            B[785] = Delay[0]; // 0.5 secs delay
            B[786] = Delay[1];
            B[798] = (byte)(B[798] | 0X87);
            BW.Write(B, 781, 18);
            BW.Write(B, 13, 768);
            BW.Write(B, 799, B.Length - 800);
        }

        /* Draw content of picturebox to bitmap, and convert it to byte array. Later I use that list of byte arrays to generate animated GIF file. */
        public static void SaveFrame(ref List<byte[]> frames, PictureBox pictureBox)
        {
            using (var stream = new MemoryStream())
            {
                using (var bitmap = new Bitmap(pictureBox.Width, pictureBox.Height))
                {
                    pictureBox.DrawToBitmap(bitmap, pictureBox.ClientRectangle);
                    bitmap.Save(stream, ImageFormat.Gif);
                    frames.Add(stream.ToArray());
                }
            }
        }
    }
}
