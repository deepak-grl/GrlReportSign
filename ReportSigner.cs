using System;
using System.IO;
using System.Text;

namespace GrlReportSign
{
    public class ReportSigner
    {

        private readonly ISignatureProvider _signatureStrategy;

        public ReportSigner(ISignatureProvider signatureStrategy)
        {
            _signatureStrategy = signatureStrategy ?? throw new ArgumentNullException(nameof(signatureStrategy));
        }

        public void AddReportFileSignatureHTML(StringBuilder reportContent)
        {
            string content = reportContent.ToString();
            string signature = _signatureStrategy.GenerateSignature(content);
            AppendSignatureToHTMLFile(reportContent, signature);
        }

        private void AppendSignatureToHTMLFile(StringBuilder reportContent, string signature)
        {

            if (reportContent == null)
                throw new ArgumentNullException(nameof(reportContent), "Report content cannot be null.");

            if (signature == null)
                throw new ArgumentNullException(nameof(signature), "Signature cannot be null.");

            // Find the end of the <head> tag
            int headTagEnd = reportContent.ToString().IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            if (headTagEnd > -1)
            {
                string metaTag = $"<meta name=\"signature\" content=\"{signature}\" />";
                // Insert the meta tag before the </head> tag
                reportContent.Insert(headTagEnd, metaTag);
            }
            else
            {
                throw new InvalidOperationException("No </head> tag found in the report content.");
            }
        }
    }
}
