using UnityEngine;

public class EpochTimeManager : Singleton<EpochTimeManager>
{
    public static long CurrentTime() => System.DateTimeOffset.Now.ToUnixTimeSeconds();
    public static long CurrentTimeMS() => System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    
    public static long UTCTime2EpochTime(int year, int month, int day, int hour, int minute, int second)
    {
        // Create a DateTime object for the specified date and time.
        // Note: Ensure the DateTime is in UTC to get the correct Unix time.
        System.DateTime dateTime = new System.DateTime(year, month, day, hour, minute, second, System.DateTimeKind.Utc);
        
        // Convert the DateTime object to Unix epoch time (seconds since 1970-01-01T00:00:00Z).
        long epochTime = new System.DateTimeOffset(dateTime).ToUnixTimeSeconds();
        
        return epochTime;
    }
}
