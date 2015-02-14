
namespace SoftwareMind.SimpleWorkflow
{
    public class WFSignal
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public WFActivityBase Activity { get; set; }
        
        public void Run()
        {
            Logger.Log.Debug("Zasygnalizowano {0} {1} ", this.Code, this.Name);
            Activity.Run();
        }
    }
}
