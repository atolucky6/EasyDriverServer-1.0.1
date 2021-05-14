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
            //authenticateTask = Task.Factory.StartNew(Authenticate, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            SerialKey = LicenseManager.GetStoredSerialKey(product);
            this.projectManagerService = projectManagerService;
        }

        public async Task<int> Authenticate(string serialKey)
        {
            try
            {
                string url = $"http://localhost/ScadaLicense/Authentication?" +
                    $"serialKey={serialKey}&product={Product}&computerId={ComputerId}";
                int limitTagCount = 0;

                if (string.IsNullOrEmpty(serialKey))
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
                try
                {
                    if (projectManagerService != null)
                    {
                        if (projectManagerService.CurrentProject == null)
                        {
                            await Task.Delay(5000);
                        }
                        else
                        {
                            // Do authenticate every 30 minutes
                            //Thread.Sleep(30 * 60 * 1000);

                            await Task.Delay(1000);

                            //string url = $"http://eslic.xyz/ScadaLicense/Authentication?" +
                            //    $"serialKey={SerialKey}&product={Product}&computerId={ComputerId}";

                            string url = $"http://localhost/ScadaLicense/Authentication?" +
                                    $"serialKey={SerialKey}&product={Product}&computerId={ComputerId}";

                            int limitTagCount = 0;

                            SerialKey = LicenseManager.GetStoredSerialKey(Product);

                            if (!string.IsNullOrEmpty(SerialKey))
                            {
                                limitTagCount = await LicenseManager.AuthenticateAsync(url, Product, ComputerId);
                                LimitTagCount = limitTagCount;
                            }

                            if (projectManagerService.CurrentProject != null && projectManagerService.CurrentProject.Childs.Count > 0)
                            {
                                int tagCounts = projectManagerService.CurrentProject.GetAllTags(true).Count();
                                bool isAuthenticated = false;
                                string message = "";
                                if (tagCounts <= limitTagCount || tagCounts <= 256)
                                    isAuthenticated = true;
                                Messenger.Default.Send(new AuthenticateLicenseMessage(isAuthenticated, limitTagCount, message));
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
}
