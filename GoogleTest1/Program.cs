using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GoogleTest1
{
    class Program
    {
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "Test1";
        private static string FolderID = "1z637kX0QL1-TRgBmOhjDKD5pWwQKIYdr"; //nazwa folderu, do którego chcemy umieścić plik
        private static string Nazwa = "TestowyPlik_1"; // jak plik ma być nazwany
        private static string Sciezka = @"C:\NaDyskGoogle\September.mp3"; //ścieżka do pliku
        /*Typ pliku, aby mógł być odczytany przez Google Drive*/
        //private static string Rodzaj = "application/zip";
        //private static string Rodzaj = "type:image/gif";
        private static string Rodzaj = "type:audio/mpeg";
        //private static string
        static void Main(string[] args)
        {
            Console.WriteLine("Autoryzacja: ");
            UserCredential credential = GetUserCredential();

            Console.WriteLine("Łączenie: ");
            DriveService service = GetDriveService(credential);

            Console.WriteLine("Wysyłanie Pliku: ");
            UploadFile(service, Nazwa, Sciezka, Rodzaj);

            Console.WriteLine("Wysłano: ");
        }

        private static UserCredential GetUserCredential()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string creadPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                creadPath = Path.Combine(creadPath, "driveApiCredentials", "drive-credentials.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes, "User", CancellationToken.None,
                    new FileDataStore(creadPath, true)).Result;
            }
        }

        private static DriveService GetDriveService(UserCredential credential)
        {
            return new DriveService(new BaseClientService.Initializer {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }

        private static string UploadFile(DriveService service, string fileName, string filePath, string contentType)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = fileName;
            fileMetadata.Parents = new List<string> { FolderID };
            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, contentType);
                request.Upload();
            }

            var file = request.ResponseBody;
            return file.Id;

        }
    }
}
