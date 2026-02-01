namespace MudSharp.Framework.Scheduling
{
    public delegate void HeartbeatManagerDelegate();

    public interface IHeartbeatManager
    {
        void StartHeartbeatTick();

        /// <summary>
        ///     The SecondHeartbeat fires approximately once every second. It does not allow the same delegate to be subscribed
        ///     more than once.
        /// </summary>
        event HeartbeatManagerDelegate SecondHeartbeat;

        /// <summary>
        ///     The TenSecondHeartbeat fires approximately once every 10 seconds. It does not allow the same delegate to be
        ///     subscribed more than once.
        /// </summary>
        event HeartbeatManagerDelegate TenSecondHeartbeat;

        /// <summary>
        /// The ThirtySecondHeartbeat fires approximately once every 30 seconds. It does not allow the same delegate to be subscribed more than once.
        /// </summary>
        event HeartbeatManagerDelegate ThirtySecondHeartbeat;

        /// <summary>
        ///     The MinuteHeartbeat fires approximately once every minute. It does not allow the same delegate to be subscribed
        ///     more than once.
        /// </summary>
        event HeartbeatManagerDelegate MinuteHeartbeat;

        /// <summary>
        ///     The HourHeartbeat fires approximately once every hour. It does not allow the same delegate to be subscribed more
        ///     than once.
        /// </summary>
        event HeartbeatManagerDelegate HourHeartbeat;

        /// <summary>
        /// The FuzzyFiveSecondHeartbeat fires every 5 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 5 second intervals
        /// </summary>
        event HeartbeatManagerDelegate FuzzyFiveSecondHeartbeat;

        /// <summary>
        /// The FuzzyTenSecondHeartbeat fires every 10 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 10 second intervals
        /// </summary>
        event HeartbeatManagerDelegate FuzzyTenSecondHeartbeat;

        event HeartbeatManagerDelegate FuzzyThirtySecondHeartbeat;

        /// <summary>
        /// The FuzzyMinuteHeartbeat fires every 60 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 60 second intervals
        /// </summary>
        event HeartbeatManagerDelegate FuzzyMinuteHeartbeat;

		/// <summary>
		/// The event fires every 300 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 60 second intervals
		/// </summary>
		/// <remarks>The timing of this event is not guaranteed to be exactly five minutes; it may vary
		/// slightly due to system scheduling or other factors. Subscribers should not rely on precise intervals for
		/// time-sensitive operations.</remarks>
		event HeartbeatManagerDelegate FuzzyFiveMinuteHeartbeat;

		/// <summary>
		/// The event fires every 600 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 60 second intervals
		/// </summary>
		/// <remarks>The timing of this event is not guaranteed to be exactly ten minutes; it may vary
		/// slightly due to system scheduling or other factors. Subscribers should not rely on precise intervals for
		/// time-sensitive operations.</remarks>
		event HeartbeatManagerDelegate FuzzyTenMinuteHeartbeat;

		/// <summary>
		/// The event fires every 1800 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 60 second intervals
		/// </summary>
		/// <remarks>The timing of this event is not guaranteed to be exactly thirty minutes; it may vary
		/// slightly due to system scheduling or other factors. Subscribers should not rely on precise intervals for
		/// time-sensitive operations.</remarks>
		event HeartbeatManagerDelegate FuzzyThirtyMinuteHeartbeat;

		/// <summary>
		/// The FuzzyHourHeartbeat fires every 3600 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 3600 second intervals
		/// </summary>
		event HeartbeatManagerDelegate FuzzyHourHeartbeat;

        void ManuallyFireHeartbeatHour();
        void ManuallyFireHeartbeatMinute();
        void ManuallyFireHeartbeat30Second();
        void ManuallyFireHeartbeat10Second();
        void ManuallyFireHeartbeat5Second();
        void ManuallyFireHeartbeatSecond();
	}
}