/**
 * Copyright @ 2011 Quan Nguyen
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using System.Text;
using System.IO;

using Tesseract;
using Page = Tesseract.Page;
using IImage = Microsoft.Maui.Graphics.IImage;
using Microsoft.Maui.Graphics.Skia;

namespace VietOCR
{
    class OCRImages : OCR<IImage>
    {
        /// <summary>
        /// Recognize text.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="inputName">input filename</param>
        /// <param name="index"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public override string RecognizeText(IList<IImage> images, string inputName)
        {
            IEnumerable<string> configs_file = new List<string>() { CONFIGS_FILE };

            using (TesseractEngine engine = new TesseractEngine(Datapath, Language, EngineMode, configs_file))
            {
                engine.SetVariable("tessedit_create_hocr", OutputFormat == "hocr" ? "1" : "0");
                ControlParameters(engine);
                Tesseract.PageSegMode psm = (PageSegMode)Enum.Parse(typeof(PageSegMode), PageSegMode);

                StringBuilder strB = new();
                int pageNum = 0;

                foreach (IImage image in images)
                {
                    pageNum++;
                    using (Pix pix = ConvertImageToPix(image))
                    {
                        using (Page page = engine.Process(pix, inputName, psm))
                        {
                            string text = OutputFormat == "hocr" ? page.GetHOCRText(pageNum - 1) : page.GetText();

                            if (text == null) return String.Empty;
                            strB.Append(text);
                        }
                    }
                }

                return strB.ToString().Replace("\n", Environment.NewLine);
            }
        }

        /// <summary>
        /// Reads tessdata/configs/tess_configvars and SetVariable on Tesseract engine.
        /// This only works for non-init parameters (@see <a href="https://code.google.com/p/tesseract-ocr/wiki/ControlParams">ControlParams</a>).
        /// </summary>
        /// <param name="engine"></param>
        void ControlParameters(TesseractEngine engine)
        {
            string configsFilePath = Path.Combine(Datapath, "tessdata/configs/" + CONFIGVARS_FILE);
            if (!File.Exists(configsFilePath))
            {
                return;
            }

            string[] lines = File.ReadAllLines(configsFilePath);
            foreach (string line in lines)
            {
                if (!line.Trim().StartsWith("#"))
                {
                    try
                    {
                        string[] keyValuePair = line.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        string value = keyValuePair[1];
                        if (value == "T" || value == "F")
                        {
                            engine.SetVariable(keyValuePair[0], value == "T" ? true : false);
                        }
                        else
                        {
                            engine.SetVariable(keyValuePair[0], keyValuePair[1]);
                        }
                    }
                    catch
                    {
                        //ignore and continue on
                    }
                }
            }
        }

        public override void ProcessFile(string filename)
        {
            List<RenderedFormat> renderedFormats = new List<RenderedFormat>();

            foreach (string format in OutputFormat.ToUpper().Split(','))
            {
                renderedFormats.Add((RenderedFormat)Enum.Parse(typeof(RenderedFormat), format));
            }
            
            using (IResultRenderer renderer = new AggregateResultRenderer(ResultRenderer.CreateRenderers(OutputFile, Datapath, renderedFormats)))
            {
                ProcessImageFile(renderer, filename);
            }
        }

        /// <summary>
        /// Processes a file using ResultRenderers.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="filename"></param>
        private void ProcessImageFile(IResultRenderer renderer, string filename)
        {
            IEnumerable<string> configs_file = new List<string>() { CONFIGS_FILE };

            using (TesseractEngine engine = new TesseractEngine(Datapath, Language, EngineMode, configs_file))
            {
                ControlParameters(engine);
                Tesseract.PageSegMode psm = (PageSegMode)Enum.Parse(typeof(PageSegMode), PageSegMode);

                string imageName = Path.GetFileNameWithoutExtension(filename);

                using (var pixA = LoadPixArray(filename))
                {
                    using (renderer.BeginDocument(imageName))
                    {
                        foreach (var pix in pixA)
                        {
                            Pix pixd = null;
                            Pix pixr = null;

                            try
                            {
                                if (ProcessingOptions.Deskew)
                                {
                                    pixd = pix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction, Pix.DefaultBinaryThreshold, out Scew scew);
                                }
                                if (ProcessingOptions.RemoveLines)
                                {
                                    //pixr = ImageHelper.RemoveLines(pixd ?? pix);
                                }

                                using (var page = engine.Process(pixr ?? pixd ?? pix, imageName, psm))
                                {
                                    var addedPage = renderer.AddPage(page);
                                }
                            }
                            finally
                            {
                                if (pixd != null)
                                {
                                    ((IDisposable)pixd).Dispose();
                                }
                                if (pixr != null)
                                {
                                    ((IDisposable)pixr).Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        private PixArray LoadPixArray(string filename)
        {
            if (filename.ToLower().EndsWith(".tif") || filename.ToLower().EndsWith(".tiff"))
            {
                return PixArray.LoadMultiPageTiffFromFile(filename);
            }
            else
            {
                PixArray pixA = PixArray.Create(0);
                IList<IImage> imageList = new List<IImage>();
                using (Stream stream = File.OpenRead(filename))
                {
                    imageList.Add(SkiaImage.FromStream(stream));
                }

                foreach (IImage image in imageList)
                {
                    pixA.Add(ConvertImageToPix(image));
                }

                return pixA;
            }
        }

        /// <summary>
        /// Converts .NET Bitmap to Leptonica Pix.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Pix ConvertImageToPix(IImage image)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    image.Save(memStream);
                    return Pix.LoadFromMemory(memStream.ToArray());
                }
            }
            catch
            {
                return ConvertImageToPixViaFile(image);
            }
        }

        /// <summary>
        /// Writes .NET image to file and read it back as Pix image. Works in all cases but not efficient.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Pix ConvertImageToPixViaFile(IImage image)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";

                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    ((IImage)image).Save(memStream, Microsoft.Maui.Graphics.ImageFormat.Png);
                    memStream.WriteTo(file);
                    Pix pix = Pix.LoadFromFile(fileName);
                    File.Delete(fileName);

                    return pix;
                }
            }
        }

        /// <summary>
        /// Gets segmented regions at specified page iterator level.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public override List<System.Drawing.Rectangle> GetSegmentedRegions(IImage image, PageIteratorLevel level)
        {
            using (var engine = new TesseractEngine(Datapath, Language, EngineMode))
            {
                using (var page = engine.Process(ConvertImageToPix(image)))
                {
                    List<System.Drawing.Rectangle> boxes = page.GetSegmentedRegions(level);
                    return boxes;
                }
            }
        }
    }
}
