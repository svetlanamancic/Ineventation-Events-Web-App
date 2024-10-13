using Ineventation.Business.Objects.Requests;
using Ineventation.Business.Objects.Responses;
using System;

namespace Ineventation.Business.Services
{
    public class CategoryService: BaseService
    {
        public CategoryService():base()
        {
        }

        public CategoryListResponse GetCategories()
        {
            try
            {
                var list = categoryRepository.GetAllCategories();
                

                if (list != null)
                {
                    return new CategoryListResponse { Status = true, Message = "Load successfull.", Target = list };
                }
                else
                {
                    return new CategoryListResponse { Status = false, Message = "." };
                }

            }
            catch (Exception)
            {
                return new CategoryListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public CategoryListResponse GetFavorites(GetCategoriesForUser request)
        {
            try
            {
                var list = categoryRepository.GetLikes(request.Token);


                if (list != null)
                {
                    return new CategoryListResponse { Status = true, Message = "Load successfull.", Target = list };
                }
                else
                {
                    return new CategoryListResponse { Status = false, Message = "." };
                }

            }
            catch (Exception)
            {
                return new CategoryListResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }

        public StringResponse AddCategory(string name)
        {
            try
            {
                bool result = categoryRepository.ExistsCategory(name);
                if (!result)
                {
                    var res = categoryRepository.CreateCategory(name);


                    if (res != null)
                    {
                        return new StringResponse { Status = true, Message = "Load successfull.", Target = "Success." };
                    }
                    else
                    {
                        return new StringResponse { Status = false, Message = "." };
                    }
                }
                return new StringResponse { Status = true, Message = "Already exists." };

            }
            catch (Exception)
            {
                return new StringResponse { Status = false, Message = "Internal server error.", Code = 501 };
            }
        }
    }
}
