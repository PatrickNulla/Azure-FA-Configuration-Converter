namespace Azure_FA_Configuration_Converter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConverter converter = new Converter();
            converter.LocalToPipeline();
            converter.PipelineToAzureFaConfig();

            bool pauseOnFinish = args.Contains("--pauseOnFinish");
            if (pauseOnFinish)
            {
                Console.WriteLine("Configuration conversion finished. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
