public class OrderFailedEvent : Unity.Services.Analytics.Event
{
    public OrderFailedEvent() : base("order_failed")
    {
    }

    public float WaitTime { set { SetParameter("wait_time", value); } }
    public int ToppingsRequested { set { SetParameter("toppings_requested", value); } }
}
