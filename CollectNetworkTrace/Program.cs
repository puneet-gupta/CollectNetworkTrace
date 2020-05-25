using System;

namespace CollectNetworkTrace
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Utility.Trace("duration missing for CollectNetworkTrace.exe");
                Utility.FlushTrace();
                return;
            }
            try
            {
                if (int.TryParse(args[0], out int duration))
                {
                    duration = duration > 900 ? 60 : duration;
                    var config = NetworkTraceConfiguation.GetConfiguration();
                    var authToken = AuthService.GetAcccessToken(config);
                    var armService = new ArmService(config, authToken);
                    Utility.Trace($"Chosen duration for Network Trace = {duration}");
                    armService.StartNetworkTrace(duration);
                }
                else
                {
                    Utility.Trace($"Invalid argument value for duration = {args[0]}");
                }
            }
            catch (Exception ex)
            {
                Utility.Trace($"Exception while running the tool {ex.GetType()}:{ex.Message} {Environment.NewLine} {ex.StackTrace}");
            }
            finally
            {
                Utility.FlushTrace();
            }
        }
    }
}
