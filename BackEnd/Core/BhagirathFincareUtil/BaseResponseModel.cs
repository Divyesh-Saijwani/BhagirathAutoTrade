using BhagirathFincareUtil.Enum;

namespace BhagirathFincareUtil
{
    public class BaseResponseModel
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public SuccessStatus Success { get; set; }
    }
}
