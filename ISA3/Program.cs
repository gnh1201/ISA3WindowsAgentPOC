using RestSharp;
using SnipeSharp;
using SnipeSharp.Endpoints.Models;
using System;
using System.Collections.Generic;

namespace ISA3
{
    class Program
    {
        static string AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6ImQxNGQzMmRmMGQ3ZjVjOTU3ODIzYmIyYWExZjliMjBmOTk0OWRkYmYxY2NhYTAwODdiNmZiNDgyNDE3YjEzOWZhNDM1ODVkMjY0NGI2ODdmIn0.eyJhdWQiOiIxIiwianRpIjoiZDE0ZDMyZGYwZDdmNWM5NTc4MjNiYjJhYTFmOWIyMGY5OTQ5ZGRiZjFjY2FhMDA4N2I2ZmI0ODI0MTdiMTM5ZmE0MzU4NWQyNjQ0YjY4N2YiLCJpYXQiOjE1NzU4MjcwNDIsIm5iZiI6MTU3NTgyNzA0MiwiZXhwIjoxNjA3NDQ5NDQyLCJzdWIiOiIyIiwic2NvcGVzIjpbXX0.9G1dn8kQUiqiqVaY9GQH2gaC6-p27A5xOzurAmvvvrzm20_np5faPWwnI8EVnFGUTFCqeXP0qXUypKefaa7ZjAcuowR2abvkNGWrGnZNfj9_PYfkakWB35YcZ78A6dFedgVJhTmB-uTHtzs94rBjq4SkD7MrIRAxRp1qaUk5EaFBSz3jGtfKBWHfq-GqCozHLCk7xgyScgYFjszo5lddaEqhvnKMhbuSP79UMYVLL8e6bM7Gfb_WOtr3rBWg5DJm6-ei0bluiD_x3oXLCj8JMYkSIZFKiG2cPo2IPUsx7GhaAvqfyPy7RwSq7o-iBukTDVNnhVzyMU4E_yC8B2ml3sOITqimnP7o-h6CCUWcmJHnbthXwMs_cXpV0I7g0flQTioVy1UpKVhEBdoMFwhiSZtlpY8sKibRTCY0khdqFSBoXGk5dEkRlo_GYoNc9VvCfQJEFxFQ00r24ZSZ-8kb9rQt8Cf-zPRe7PI0h5pBf8KoKMoAEx04pHX5ZuuA0fReUFDAR6YYvLD1DpVPF8-008vDAo-Ee3M67Rm_wMpaJtzOQv__hZkIARtvwPDBW7EnJDj-PCiJnNsJomNM3SRBVin4CtNPt44iZSB8tbYfMUQ9BOfc6XcpuABFY0seLBN5ynQe0DWg-jkH5MfU6X3kGXRCp0ON-bSdDxFfubOWDxI";
        static string ApiUrl = "http://66.42.61.142/api/v1";

        static void Main(string[] args)
        {
            // 자산 관리 서버에 인증합니다.
            SnipeItApi snipe = new SnipeItApi();
            snipe.ApiSettings.ApiToken = AccessToken;
            snipe.ApiSettings.BaseUrl = new Uri(ApiUrl);

            // 회사 정보를 불러옵니다.
            Company company = snipe.CompanyManager.Get("캐츠워즈리서치");

            // 자산 부가 정보를 생성합니다.
            Dictionary<string, string> customFields = new Dictionary<string, string>();
            customFields.Add("_snipeit_mac_iioe_4", DeviceService.GetPrimaryMAC());
            customFields.Add("_snipeit_ipv4_iioe_5", DeviceService.GetPrimaryMAC());
            customFields.Add("_snipeit_iii0_ie_8", DeviceService.GetComputerName());
            customFields.Add("_snipeit_ioeeis0_e2i_9", DeviceService.GetOSVersion());

            // 현재 자산을 등록합니다.
            Asset asset = new Asset()
            {
                Name = DeviceService.GetComputerName(),
                AssetTag = "12345678",
                Model = snipe.ModelManager.Get("OEM"),
                StatusLabel = snipe.StatusLabelManager.Get("사용가능"),
                Location = snipe.LocationManager.Get("ICN"),
                CustomFields = customFields
            };
            snipe.AssetManager.Create(asset);

            // 현재 설치된 소프트웨어 목록을 불러옵니다.
            List<SoftwareModel> softwares = RegistryService.GetInstalledSoftwares();

            // 소프트웨어 목록에 따라 작업을 진행합니다.
            foreach(SoftwareModel software in softwares)
            {
                // 제조사(개발사) 정보를 먼저 등록합니다.
                Manufacturer manufacturer = new Manufacturer
                {
                    Name = software.Publisher,
                    Url = "",
                    Image = "",
                    SupportUrl = software.HelpLink,
                    SupportPhone = "",
                    SupportEmail = "",
                    AssetsCount = 0,
                    LicensesCount = 0
                };
                snipe.ManufacturerManager.Create(manufacturer);
                manufacturer = snipe.ManufacturerManager.Get(software.Publisher);

                // 라이센스 정보를 등록합니다.
                var client = new RestClient(ApiUrl);
                var request = new RestRequest("/licenses", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("name", software.DisplayName);
                request.AddParameter("seats", 1);
                request.AddParameter("category_id", 1);
                if(manufacturer != null) {
                    request.AddParameter("manufacturer_id", manufacturer.Id);
                }
                if (company != null)
                {
                    request.AddParameter("company_id", company.Id);
                }
                client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", AccessToken));
                client.Execute(request);

                /*
                License license = new License
                {
                    Name = software.DisplayName,
                    Company = snipe.CompanyManager.Get("캐츠워즈리서치"),
                    Seats = 1,
                    CategoryId = 1,
                    //ExpirationDate = null,
                    //FreeSeatsCount = 1,
                    //LicenseEmail = "",
                    //LicenseName = software.DisplayName,
                    //Maintained = false,
                    Manufacturer = snipe.ManufacturerManager.Get(software.Publisher),
                    //Notes = "",
                    //OrderNumber = "",
                    //ProductKey = "",
                    //PurchaseCost = "",
                    //PurchaseDate = null,
                    //PurchaseOrder = "",
                    //Seats = 1,
                    //Supplier = null,
                    //UserCanCheckout = false
                };
                snipe.LicenseManager.Create(license);
                */
            }

            Console.WriteLine("등록이 되었는지 확인하세요.");

            Console.WriteLine("Press ESC to exit...");

            ConsoleKeyInfo k;
            while (true)
            {
                k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Escape)
                    break;

                Console.WriteLine("{0} --- ", k.Key);
            }
        }
    }
}
