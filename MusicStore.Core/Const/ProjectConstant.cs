namespace MusicStore.Core.Const
{
    public class ProjectConstant
    {
        public const string ResultNotFound = "Data not found";

        public const string Proc_CoverType_GetAll = "sp_GetCoverTypes";
        public const string Proc_CoverType_Get = "sp_GetCoverType";
        public const string Proc_CoverType_Delete = "sp_DeleteCoverType";
        public const string Proc_CoverType_Create = "sp_CreateCoverType";
        public const string Proc_CoverType_Update = "sp_UpdateCoverType";

        public const string Role_User_Indi = "Individual Customer";
        public const string Role_User_Comp = "Company Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string ShoppingCart = "ShoppingCart";

        // PaymentStatus
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayed = "Delayed";
        

        // OrderStatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCanceled = "Cancelled";
        public const string StatusRefund = "Refund";

        public static double GetPriceBaseOnQuantity(int quantity, double price, double price50, double price100)
        {
            if (quantity < 50)
                return price;

            else
            {
                if (quantity < 100)
                    return price50;
                else
                    return price100;
            }
        }

        public static string ConvertToRawHtml(string description)
        {
            char[] array = new char[description.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < description.Length; i++)
            {
                char let = description[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array,0,arrayIndex);
        }
    }
}
