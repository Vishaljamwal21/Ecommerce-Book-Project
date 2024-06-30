using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.Utility
{
    public static class SD
    {
        //CoverType Stored Procedure
        public const string Proc_GetCoverTypes = "Get_CoverTypes";
        public const string Proc_GetCoverType = "Get_CoverType";
        public const string Proc_CreateCoverTypes = "Create_CoverType";
        public const string Proc_UpdateCoverTypes = "Update_CoverType";
        public const string Proc_DeleteCoverTypes = "Delete_CoverType";


        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //Order Status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProgress = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusRefunded = "Refunded";
        public const string OrderStatusCancelled = "Cancelled";

        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "Payment Status Delay";
        public const string PaymentStatusRejected = "Rejected";

        //Session Cart Count
        public const String Ss_CartSessionCount = "Cart Count Session";

        public static double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price100;
        }

    }
}
