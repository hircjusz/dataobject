
namespace SoftwareMind.SimpleWorkflow
{
    public enum WFActivityState : int
    {
        /// <summary>
        /// Aktywny 
        /// </summary>
        Active,
        /// <summary>
        /// Krok został zakończony, 
        /// możliwe jest że kroki następne są wykonywane
        /// </summary>
        Completed,
        /// <summary>
        /// 
        /// </summary>
        Initialized,
        /// <summary>
        /// 
        /// </summary>
        Waiting,
        /// <summary>
        /// 
        /// </summary>
        Executed

    }
}
