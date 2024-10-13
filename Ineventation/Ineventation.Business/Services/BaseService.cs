using Ineventation.Data.Repositories;

namespace Ineventation.Business.Services
{
    public class BaseService
    {
        protected UserRepository userRepository = null;
        protected EventRepository eventRepository = null;
        protected CategoryRepository categoryRepository = null;
        protected NewsRepository newsRepository = null;



        public BaseService()
        {
            userRepository = new UserRepository();
            eventRepository = new EventRepository();
            categoryRepository = new CategoryRepository();
            newsRepository = new NewsRepository();
        }
    }
}
