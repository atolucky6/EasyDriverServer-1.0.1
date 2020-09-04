namespace EasyScada.Core
{
    public static class EasyDriverConnectorProvider
    {
        static IEasyDriverConnector driverConnector;

        public static IEasyDriverConnector GetEasyDriverConnector()
        {
            if (driverConnector == null)
                driverConnector = new EasyDriverConnector();
            return driverConnector;
        }
    }
}
