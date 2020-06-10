namespace EasyScada.ServerApplication
{
    public class ShowPropertiesMessage
    {
        public object Sender { get; private set; }
        public object Item { get; private set; }
        public ShowPropertiesMessage(object sender, object item)

        {
            Sender = sender;
            Item = item;
        }
    }

    public class HidePropertiesMessage
    {
        public object Sender { get; private set; }

        public HidePropertiesMessage(object sender)
        {
            Sender = sender;
        }
    }
}
