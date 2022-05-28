using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace QuestEyes_Server.Functions
{
    public static class EyeTrackFramework
    {
        public static void ProcessData(byte[] data) //recieves eye tracking frames from camera system in byte format
        {
            MemoryStream stream = new(data);
            Mat main = Mat.FromStream(stream, ImreadModes.Unchanged);
            //Mat kernel = Cv2.GetStructuringElement();
            Cv2.CvtColor(main, main, ColorConversionCodes.BGR2GRAY);


        }
    }
}
