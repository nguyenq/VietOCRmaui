using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using VietOCR.Models;

namespace VietOCR.ViewModels
{
    public partial class OcrLanguageViewModel : ObservableObject
    {
        public ObservableCollection<OcrLanguage> OcrLanguages { set; get; }

        [ObservableProperty]
        public ObservableCollection<object> selectedLanguages;

        public IList<string> SelectedLanguageCodes { 
            set {
                SelectedLanguages = new ObservableCollection<object>(OcrLanguages.Where(x => value.Contains(x.Code)).ToList());
            } 
        }
        
        private Dictionary<string, string> installedLanguageCodes;

        private Dictionary<string, string> lookupISO639;

        public Dictionary<string, string> LookupISO639
        {
            get { return lookupISO639; }
        }

        private Dictionary<string, string> lookupISO_3_1_Codes;

        public Dictionary<string, string> LookupISO_3_1_Codes
        {
            get { return lookupISO_3_1_Codes; }
        }

        public OcrLanguageViewModel()
        {
            OcrLanguages = new ObservableCollection<OcrLanguage>(GetInstalledLanguagePacks());
        }

        /// <summary>
        /// Gets Tesseract's installed language data packs.
        /// </summary>
        IList<OcrLanguage> GetInstalledLanguagePacks()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var tessdataPath = Path.Combine(baseDir, "tessdata");

            lookupISO639 = new Dictionary<string, string>();
            lookupISO_3_1_Codes = new Dictionary<string, string>();
            installedLanguageCodes = new Dictionary<string, string>();

            try
            {
                string[] installedLanguagePacks = Directory.GetFiles(tessdataPath, "*.traineddata");
                installedLanguagePacks = installedLanguagePacks.Where(x => !x.EndsWith("osd.traineddata")).ToArray();

                string xmlFilePath = Path.Combine(baseDir, "Data/ISO639-3.xml");
                LoadFromXML(lookupISO639, xmlFilePath);
                xmlFilePath = Path.Combine(baseDir, "Data/ISO639-1.xml");
                LoadFromXML(lookupISO_3_1_Codes, xmlFilePath);

                if (installedLanguagePacks != null)
                {
                    foreach (string langPack in installedLanguagePacks)
                    {
                        string langCode = Path.GetFileNameWithoutExtension(langPack);
                        // translate ISO codes to full English names for user-friendliness
                        if (lookupISO639.ContainsKey(langCode))
                        {
                            installedLanguageCodes.Add(langCode, lookupISO639[langCode]);
                        }
                        else
                        {
                            installedLanguageCodes.Add(langCode, langCode);
                        }
                    }

                    List<OcrLanguage> langList = installedLanguageCodes.Select(x => new OcrLanguage { Code = x.Key, Name = x.Value }).OrderBy(x => x.Name).ToList();
                    return langList;

                    //List<string> lst = installedLanguageCodes.Values.ToList();
                    //lst.Sort();
                    //return lst;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(this, e.Message, strProgName);
                // this also applies to missing language data files in tessdata directory
                Console.WriteLine(e.StackTrace);
            }

            return new List<OcrLanguage>();
        }

        /// <summary>
        /// Populates a dictionary with entries from an XML document.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="xmlFilePath"></param>
        public static void LoadFromXML(Dictionary<string, string> table, string xmlFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList list = doc.GetElementsByTagName("entry");
            foreach (XmlNode node in list)
            {
                if (!table.ContainsKey(node.Attributes[0].Value))
                {
                    table.Add(node.Attributes[0].Value, node.InnerText);
                }
            }
        }
    }
}
