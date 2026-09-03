using System.Threading.Tasks;

namespace CompanionDisplayWinUI.API
{
    public static class TaskAPI
    {
        public static async Task IgnoreExceptionsAsync(Task task)
        {
            try
            {
                await task;
            }
            catch{ }
        }
    }
}
