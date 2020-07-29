using DevExpress.Mvvm;
using EasyDriverPlugin;
using EasyScada.License;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface ILicenseManagerService
    {
        string ComputerId { get; }
        string Product { get; }
        string SerialKey { get; }
        int LimitTagCount { get; }
        Task<int> Authenticate(string serialKey);
    }

    public class LicenseManagerService : ILicenseManagerService
    {
        public string ComputerId { get; private set; }

        public string SerialKey { get; private set; }

        public string Product { get; private set; }

        public int LimitTagCount { get; private set; }

        private readonly Task authenticateTask;
        private readonly IProjectManagerService projectManagerService;

        public LicenseManagerService(string product, IProjectManagerService projectManagerService)
        {
            ComputerId = LicenseManager.GetComputerId();
            Product = product;
            authenticateTask = Task.Factory.StartNew(Authenticate, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            SerialKey = LicenseManager.GetStoredSerialKey(product);
            this.projectManagerService = projectManagerService;
        }

        public async Task<int> Authenticate(string serialKey)
        {
            try
            {
                string url = $"http://localhost/EasyScadaLicsenceService/Authentication?" +
                    $"serialKey={serialKey}&product={Product}&computerId={ComputerId}";
                int limitTagCount = 0;

                if (string.IsNullOrEmpty(SerialKey))
                    limitTagCount = 0;
                else
                    limitTagCount = await LicenseManager.AuthenticateAsync(url, Product, ComputerId);

                if (limitTagCount > 0)
                {
                    LicenseManager.SetSerialKey(Product, serialKey);
                }
                LimitTagCount = limitTagCount;
                return limitTagCount;
            }
            catch { return 0; }
        }

        private async void Authenticate()
        {
            while (true)
            {
                if (projectManagerService.CurrentProject == null)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    // Do authenticate every 30 minutes
                    Thread.Sleep(30 * 60 * 1000);

                    string url = $"http://45.119.212.41/EasyScadaLicsenceService/Authentication?" +
                        $"serialKey={SerialKey}&product={Product}&computerId={ComputerId}";
                    int limitTagCount = 0;

                    if (!string.IsNullOrEmpty(SerialKey))
                    {
                        limitTagCount = await LicenseManager.AuthenticateAsync(url, Product, ComputerId);
                        LimitTagCount = limitTagCount;
                    }

                    if (projectManagerService.CurrentProject != null)
                    {
                        int tagCounts = projectManagerService.CurrentProject.LocalStation.Find(x => x is ITagCore, true).Count();
                        bool isAuthenticated = false;
                        string message = "";
                        if (tagCounts <= limitTagCount && tagCounts > 256)
                            isAuthenticated = true;
                        Messenger.Default.Send(new AuthenticateLicenseMessage(isAuthenticated, limitTagCount, message));
                    }
                }
            }
        }
    }
}
