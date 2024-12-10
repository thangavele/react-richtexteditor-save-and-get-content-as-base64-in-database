using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Text.RegularExpressions;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
namespace RTEImageWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class RichTextEditorController : Controller
    {
        // Interface that provides Provides information about the web hosting environment an application is running in.
        // WebRootPath - Path of the www folder(Gets or sets the absolute path to the directory that contains the web-servable application content files)
        // ContentRootPath − Path of the root folder which contains all the Application files(Gets or sets an IFileProvider pointing at WebRootPath.)
        // To Learn more click here https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-7.0

        private readonly IWebHostEnvironment _webHostEnvironment;
        private int count = 1;
        public RichTextEditorController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [AcceptVerbs("Post")]
        [EnableCors("AllowAllOrigins")]
        [Route("GetForecastAsync")]
        public FileStreamResult GetForecastAsync(string RteValue)
        {
            using (FileStream fileStreamPath = new FileStream(Path.GetFullPath(@"Template.docx"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (WordDocument document = new WordDocument())
                {
                    string htmlText = Request.Form["customtool"].ToString(); ;
                    //Hooks the ImageNodeVisited event to open the image from a specific location
                    document.HTMLImportSettings.ImageNodeVisited += OpenImage;
                    document.Open(fileStreamPath, Syncfusion.DocIO.FormatType.Html);
                    //Html string to be inserted
                    string htmlstring = htmlText;

                    string modifiedHtml = Regex.Replace(htmlstring, "<img[^>]*>", match =>
                    {
                        string imgTag = match.Value;
                        if (!imgTag.EndsWith("/>"))
                        {
                            imgTag = imgTag.Substring(0, imgTag.Length - 1) + "/>";
                        }
                        return imgTag;
                    });

                    //Validates the Html string
                    bool isValidHtml = document.LastSection.Body.IsValidXHTML(modifiedHtml, XHTMLValidationType.None);
                    //When the Html string passes validation, it is inserted to the document
                    if (isValidHtml)
                    {
                        //Appends the Html string to first paragraph in the document
                        document.Sections[0].Body.Paragraphs[0].AppendHTML(modifiedHtml);
                    }
                    //Unhooks the ImageNodeVisited event after loading HTML
                    document.HTMLImportSettings.ImageNodeVisited -= OpenImage;
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    stream.Position = 0;

                    //Download Word document in the browser
                    return File(stream, "application/msword", "Result.docx");
                }
            }
        }
        private static void OpenImage(object sender, ImageNodeVisitedEventArgs args)
        {
            if (args.Uri.StartsWith("https://"))
            {
                WebClient client = new WebClient();
                //Download the image as a stream.
                byte[] image = client.DownloadData(args.Uri);
                Stream stream = new MemoryStream(image);
                //Set the retrieved image from the input Markdown.
                args.ImageStream = stream;
            }
        }

        public class RTEContent
        {
            public string RTE1Content { get; set; }
            public string RTE2Content { get; set; }
            public string RTE3Content { get; set; }
            public string RTE4Content { get; set; }
        }
        
        [HttpGet("GetAllRTEContent")]
        [EnableCors("AllowAllOrigins")]
        public IActionResult GetAllRTEContent()
        {
           // Define the base64 strings for each RTE content
           string base64StringRTE1 = "PGh0bWw+PGJvZHk+PGgxPkhlbGxvLCBXb3JsZCE8L2gxPjxwPlRoaXMgaXMgYSBzYW1wbGUgSFRNTCBjb250ZW50LjwvcD48L2JvZHk+PC9odG1sPg==\n";
           string base64StringRTE2 = "PGh0bWw+PGJvZHk+PGgxPkhlbGxvLCBXb3JsZCE8L2gxPjxwPlRoaXMgaXMgYSBzYW1wbGUgSFRNTCBjb250ZW50LjwvcD48L2JvZHk+PC9odG1sPg==\n";
           string base64StringRTE3 = "PGh0bWw+PGJvZHk+PGgxPkhlbGxvLCBXb3JsZCE8L2gxPjxwPlRoaXMgaXMgYSBzYW1wbGUgSFRNTCBjb250ZW50LjwvcD48L2JvZHk+PC9odG1sPg==\n";
           string base64StringRTE4 = "PGh0bWw+PGJvZHk+PGgxPkhlbGxvLCBXb3JsZCE8L2gxPjxwPlRoaXMgaXMgYSBzYW1wbGUgSFRNTCBjb250ZW50LjwvcD48L2JvZHk+PC9odG1sPg==\n";
           // Decode Base64 strings into byte arrays
           byte[] byteArrayRTE1 = Convert.FromBase64String(base64StringRTE1);
           byte[] byteArrayRTE2 = Convert.FromBase64String(base64StringRTE2);
           byte[] byteArrayRTE3 = Convert.FromBase64String(base64StringRTE3);
           byte[] byteArrayRTE4 = Convert.FromBase64String(base64StringRTE4);

           // Initialize RTEContent object to hold all HTML contents
           var rteContents = new RTEContent();

           // Create the HTML content for RTE1
           rteContents.RTE1Content = GetHtmlContentFromBase64(byteArrayRTE1);
           // Create the HTML content for RTE2
           rteContents.RTE2Content = GetHtmlContentFromBase64(byteArrayRTE2);
           // Create the HTML content for RTE3
           rteContents.RTE3Content = GetHtmlContentFromBase64(byteArrayRTE3);
           // Create the HTML content for RTE4
           rteContents.RTE4Content = GetHtmlContentFromBase64(byteArrayRTE4);

           return Ok(rteContents);
        }
        
        // Helper method to decode base64 and convert to HTML string
        private string GetHtmlContentFromBase64(byte[] byteArray)
        {
            using (MemoryStream inputStream = new MemoryStream(byteArray))
            {
                // Use the correct constructor: Stream and FormatType.Html
                using (WordDocument document = new WordDocument(inputStream, FormatType.Html))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream, FormatType.Html);  // Save as HTML to MemoryStream
                        stream.Position = 0;  // Reset the position to the beginning
                        string htmlString = new StreamReader(stream).ReadToEnd();
                        htmlString = ExtractBodyContent(htmlString);
                        return htmlString; // Convert MemoryStream content to string
                    }
                }
            }
        }
        
        public string ExtractBodyContent(string html)
        {
            if (html.Contains("<html") && html.Contains("<body"))
            {
                return html.Remove(0, html.IndexOf("<body>") + 6).Replace("</body></html>", "");
            }
            return html;
        }
        
        [AcceptVerbs("Post")]
        [EnableCors("AllowAllOrigins")]
        [Route("ExportToBase64")]
        public void ExportToBase64(string customtool1, string customtool2, string customtool3, string customtool4)
        {
            // Step 1: Process each editor's content individually
            string base64String1 = ConvertEditorContentToBase64(customtool1);
            string base64String2 = ConvertEditorContentToBase64(customtool2);
            string base64String3 = ConvertEditorContentToBase64(customtool3);
            string base64String4 = ConvertEditorContentToBase64(customtool4);

            // Step 2: Print each base64 string to the console
            Console.WriteLine("Base64 String for CustomTool1:");
            Console.WriteLine(base64String1);

            Console.WriteLine("Base64 String for CustomTool2:");
            Console.WriteLine(base64String2);

            Console.WriteLine("Base64 String for CustomTool3:");
            Console.WriteLine(base64String3);

            Console.WriteLine("Base64 String for CustomTool4:");
            Console.WriteLine(base64String4);
        }
        
        private string ConvertEditorContentToBase64(string editorContent)
        {
            // Create an HTML structure for the content of the editor
            string htmlText = $"<html><body><div>{editorContent}</div></body></html>";
    
            // Step 1: Load HTML string into WordDocument
            WordDocument document = new WordDocument();
            document.EnsureMinimal();
            document.LastParagraph.AppendHTML(htmlText);

            // Step 2: Save as HTML to memory stream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                document.Save(memoryStream, FormatType.Html);
                memoryStream.Position = 0;

                // Step 3: Convert to Base64 string
                string base64String = Convert.ToBase64String(memoryStream.ToArray());
        
                // Dispose the document
                document.Dispose();

                return base64String;
            }
        }

        // To rename the files that are recieved on the server.
        // Step 1: Change the Controllder to RenameFile
        // Step 2: Handle the client side file renaming using the imageUploadSuccess event.

        [AcceptVerbs("Post")]
        [EnableCors("AllowAllOrigins")]
        [Route("SaveFile")]
        public IActionResult SaveFile(IList<IFormFile> UploadFiles)
        {
            try
            {
               foreach (IFormFile uploadFile in UploadFiles)
               {
                    // To get the file name from the header using the ContentDispositionHeaderValue class.
                    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.net.http.headers.contentdispositionheadervalue?view=aspnetcore-7.0

                    string fileName = ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');

                    // Construct the full path to save the file
                    fileName = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    // Check if the file doesn't exist and create it
                    if (!System.IO.File.Exists(fileName))
                    {
                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            uploadFile.CopyTo(fs);
                            fs.Flush();
                        }

                        return Ok();
                    }
               }
            } catch (Exception ex)
            {
                Response.Clear();

                Response.ContentType = "application/json; charset=utf-8";

                Response.StatusCode = 204;

                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";

                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = ex.Message;

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            // Add a return statement here to handle the case when no file is found.
            return StatusCode(500, $"An error occurred.");
        }

        [AcceptVerbs("Post")]
        [EnableCors("AllowAllOrigins")]
        [Route("RenameFile")]
        public IActionResult RenameFile(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (IFormFile uploadFile in UploadFiles)
                {
                    // To get the file name from the header using the ContentDispositionHeaderValue class.
                    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.net.http.headers.contentdispositionheadervalue?view=aspnetcore-7.0

                    string fileName = ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    string fileExtension = Path.GetExtension(fileName);
                    string baseFileName = "RTE_Image_";

                    string newFileName = baseFileName + count + fileExtension;

                    while (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "images", newFileName)))
                    {
                        count++;
                        newFileName = baseFileName + count + fileExtension;
                    }

                    // Construct the full path to save the file
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", newFileName);

                    // Save the renamed file
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        uploadFile.CopyTo(fs);
                        fs.Flush();
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = ex.Message;
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            // Add a return statement here to handle the case when no file is found.
            return StatusCode(500, $"An error occurred.");
        }

        [AcceptVerbs("Post")]
        [EnableCors("AllowAllOrigins")]
        [Route("DeleteFile")]
        public IActionResult DeleteFile(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (IFormFile uploadFile in UploadFiles)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');

                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return Ok($"File '{fileName}' has been deleted.");
                    }
                    else
                    {
                        return NotFound($"File '{fileName}' not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = ex.Message;
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return StatusCode(500, $"An error occurred.");

        }
    }
}
