using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GrlReportSign
{
    public class HMACVerifierSignature
    {

        private readonly ISignatureProvider _signatureStrategy;

        public HMACVerifierSignature(ISignatureProvider signatureStrategy)
        {
            _signatureStrategy = signatureStrategy ?? throw new ArgumentNullException(nameof(signatureStrategy));
        }

        public bool VerifySignatureHTML(string folderLoc)
        {
            try
            {
                if (string.IsNullOrEmpty(folderLoc)) throw new ArgumentNullException(nameof(folderLoc));

                List<string> fileLst = new List<string>();
                if (Directory.Exists(folderLoc))
                {
                    fileLst = Directory.GetFiles(folderLoc, "*.html", SearchOption.AllDirectories).ToList() ?? new List<string>();
                }
                else
                {
                    return false;
                }


                foreach (var filePath in fileLst)
                {
                    // Read the file content

                    string htmlContent = File.ReadAllText(filePath);
                    var signature = ExtractSignature(htmlContent);

                    if (signature == null)
                    {
                        Console.WriteLine($"No signature meta tag found in file: {filePath}");
                        continue;
                    }

                    var contentWithoutSignature = RemoveSignatureMetaTag(htmlContent);

                    #region old
                    //var metaTagStart = htmlContent.IndexOf("<meta name=\"signature\"", StringComparison.OrdinalIgnoreCase);
                    //if (metaTagStart == -1)
                    //{
                    //    Console.WriteLine($"No signature meta tag found in file: {filePath}");
                    //    continue;
                    //}

                    //var contentStart = htmlContent.IndexOf("content=\"", metaTagStart, StringComparison.OrdinalIgnoreCase) + 9;

                    //var contentEnd = htmlContent.IndexOf("\"", contentStart);

                    //var signature = htmlContent.Substring(contentStart, contentEnd - contentStart);

                    //var metaTagEnd = htmlContent.IndexOf(">", metaTagStart);
                    //var contentWithoutSignature = htmlContent.Remove(metaTagStart, metaTagEnd - metaTagStart + 1);
                    #endregion

                    string computedSignature = _signatureStrategy.GenerateSignature(contentWithoutSignature);

                    bool Istampered = _signatureStrategy.VerifySignature(contentWithoutSignature, computedSignature);

                    if (Istampered)
                    {
                        Console.WriteLine($"Signature is valid for file: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Signature is invalid for file: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return false;

        }

        private string ExtractSignature(string htmlContent)
        {
            var match = Regex.Match(htmlContent, "<meta name=\"signature\" content=\"(.*?)\"", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private string RemoveSignatureMetaTag(string htmlContent)
        {
            var match = Regex.Match(htmlContent, "<meta name=\"signature\" content=\"(.*?)\"\\s*/?>", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int startIndex = match.Index;
                int length = match.Length;
                var contentWithoutSignature = htmlContent.Remove(startIndex, length);
                return contentWithoutSignature;
            }
            return htmlContent;
        }
    }

}
