public class OrderCompletedEvent : Unity.Services.Analytics.Event
{
    public OrderCompletedEvent() : base("order_completion")
    {
    }

    public float CompletionTime { set { SetParameter("completion_time", value); } }
    public int ToppingsRequested { set { SetParameter("toppings_requested", value); } }
    public int ToppingsMatched { set { SetParameter("toppings_matched", value); } }
    public int MoneyEarned { set { SetParameter("money_earned", value); } }
    public float TimeRemainingRatio { set { SetParameter("time_remaining_ratio", value); } }
}
