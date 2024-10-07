//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BhagirathFincareClassLibrary.Models;
//using System.Data.Entity;
//using BhagirathFincareUtil;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;

//namespace BhagirathFincareCore.Lib
//{
//    public partial class AccountLib
//    {


//        /// <summary>
//        /// Get Users List
//        /// </summary>
//        /// <returns></returns>
//        public List<Users> GetUsers()
//        {
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    List<Users> userlist = context.Users.Where(x => x.IsAssociate == false && x.IsAdmin == false).OrderByDescending(x => x.Id).ToList();
//                    return userlist;

//                }
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Get Associate List
//        /// </summary>
//        /// <returns></returns>
//        public List<Users> GetAssociate()
//        {
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    List<Users> accociatelist = context.Users.Where(x => x.IsAssociate == true && x.IsAdmin == false).OrderByDescending(x => x.Id).ToList();
//                    return accociatelist;
//                }
//            }
//            catch (Exception ex)
//            {

//                ex.LogError(this);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Save User
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        public bool SaveUser(SignUp user, out string message)
//        {
//            message = "";
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    var data = (from us in context.Users
//                                where us.UserName.Equals(user.UserName)
//                                select us).FirstOrDefault<Users>();
//                    if (data != null)
//                    {
//                        message = "UserName already exists. Please choose a different UserName";
//                        return false;
//                    }
//                    else
//                    {
//                        if (user.ParentAssociateCode != "")
//                        {
//                            Users CheckParentAssociateCode = context.Users.Where(x => x.AssociateCode.Equals(user.ParentAssociateCode.ToUpper())).FirstOrDefault();
//                            if (CheckParentAssociateCode == null)
//                            {
//                                message = "Parent associatecode not match";
//                                return false;
//                            }
//                        }


//                        user.CreatedDate = DateTime.Now;
//                        user.LastUpadateDate = DateTime.Now;

//                        user.SubscriptionType = "Demo";
//                        int PaymentType = -1;
//                        if (user.SubscriptionType == "Demo")
//                            PaymentType = -1;
//                        else if (user.SubscriptionType == "Monthly")
//                            PaymentType = 0;
//                        else if (user.SubscriptionType == "Quarterly")
//                            PaymentType = 1;
//                        else if (user.SubscriptionType == "Halfyearly")
//                            PaymentType = 2;
//                        else if (user.SubscriptionType == "Yearly")
//                            PaymentType = 3;



//                        context.Users.Add(new Users()
//                        {
//                            CreatedDate = user.CreatedDate,
//                            LastUpadateDate = user.LastUpadateDate,
//                            FirstName = user.FirstName,
//                            LastName = user.LastName,
//                            MobileNumber = user.MobileNumber,
//                            UserId = 0,
//                            UserName = user.UserName,
//                            IsExpire = false,

//                            PaymentType = PaymentType,
//                            LastPaymentDate = DateTime.Now,
//                            Email = user.Email,
//                            IsAssociate = false,
//                            ParentAssociateCode = user.ParentAssociateCode,
//                            AssociateCode = null,
//                            SubscriptionType = user.SubscriptionType,
//                            // BirthDate = user.BirthDate,
//                            AllowedEquity = true,
//                            AllowedCommodity = true,
//                            AllowedCurrency = true,
//                            AllowedHotStocks = true,
//                            AllowedSuperCallsEquity = true,
//                            AllowedSuperCallsCommodity = true,
//                            AllowedSuperCallsCurrency = true,
//                            IsAdmin = false
//                        });
//                        context.SaveChanges();
//                        var guid = Guid.NewGuid().ToString();
//                        var getuser = (from us in context.Users
//                                       where us.UserName.Equals(user.UserName)
//                                       select us).FirstOrDefault<Users>();
//                        getuser.UserId = getuser.Id;
//                        context.Entry(getuser).State = EntityState.Modified;

//                        webpages_Membership membership = new webpages_Membership();
//                        membership.UserId = getuser.Id;
//                        membership.CreateDate = DateTime.Now;
//                        membership.PasswordFailuresSinceLastSuccess = 0;
//                        membership.IsConfirmed = false;
//                        membership.Password = PasswordHashUtill.HashPassword(user.Password);
//                        membership.PasswordSalt = "0";
//                        membership.ConfirmationToken = guid;
//                        membership.PasswordChangedDate = DateTime.Now;
//                        context.webpages_Membership.Add(membership);
//                        context.SaveChanges();


//                        string body = string.Empty;
//                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Confirmation.html");
//                        using (StreamReader reader = new StreamReader(path))
//                        {

//                            body = reader.ReadToEnd();

//                        }
//                        body = body.Replace("{name}", user.FirstName);

//                        body = body.Replace("{id}", getuser.Id.ToString());

//                        body = body.Replace("{guid}", guid);

//                        string subject = "Email Confirmation Mail";
//                        Email.SendEmail(user.Email, subject, body);

//                        string body1 = string.Empty;
//                        string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Admin.html");
//                        using (StreamReader reader = new StreamReader(path1))
//                        {

//                            body1 = reader.ReadToEnd();

//                        }
//                        body1 = body1.Replace("{user}", "Client");

//                        body1 = body1.Replace("{firstname}", user.FirstName);

//                        body1 = body1.Replace("{username}", user.UserName);

//                        body1 = body1.Replace("{email}", user.Email);

//                        body1 = body1.Replace("{mobilenumber}", user.MobileNumber);

//                        string subject1 = "New Registration";
//                        Email.SendEmail("welcome@bhagirathfincare.in", subject1, body1);

//                        message = "Successfully Sign up, Kindly check your email to confirm account";
//                        return true;

//                    }
//                }

//            }
//            catch (Exception ex)
//            {

//                ex.LogError(this);
//                message = "Something Went Wrong";
//                return false;
//            }
//        }



//        public object GetuserbyId(int id)
//        {
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    List<Users> GetUser = context.Users.Where(x => x.Id.Equals(id)).ToList();
//                    if (GetUser != null)
//                    {
//                        return GetUser;
//                    }
//                    else
//                    {
//                        return null;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        public object GetIsConfirmUser(int id)
//        {
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    webpages_Membership GetIsConfirm = context.webpages_Membership.Where(x => x.UserId.Equals(id)).FirstOrDefault();
//                    if (GetIsConfirm != null)
//                    {
//                        return GetIsConfirm;
//                    }
//                    else
//                    {
//                        return null;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }
//        /// <summary>
//        /// Edit User
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public bool EditUser(SignUp editUser)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                var id = editUser.Id;
//                Users userObj = context.Users.Find(id);
//                userObj.FirstName = editUser.FirstName;
//                userObj.LastName = editUser.LastName;
//                userObj.Email = editUser.Email;
//                userObj.MobileNumber = editUser.MobileNumber;
//                userObj.LastPaymentDate = editUser.LastPaymentDate;
//                userObj.IsExpire = editUser.IsExpire;
//                userObj.AllowedEquity = editUser.AllowedEquity;
//                userObj.AllowedCommodity = editUser.AllowedCommodity;
//                userObj.AllowedCurrency = editUser.AllowedCurrency;
//                userObj.AllowedHotStocks = editUser.AllowedHotStocks;
//                userObj.AllowedSuperCallsEquity = editUser.AllowedSuperCallsEquity;
//                userObj.AllowedSuperCallsCommodity = editUser.AllowedSuperCallsCommodity;
//                userObj.AllowedSuperCallsCurrency = editUser.AllowedSuperCallsCurrency;
//                userObj.SubscriptionType = editUser.SubscriptionType;


//                if (editUser.CreatedDate == null)
//                    userObj.CreatedDate = DateTime.Now;

//                userObj.LastUpadateDate = editUser.LastPaymentDate;

//                int PaymentType = -1;
//                if (editUser.SubscriptionType == "Demo")
//                    PaymentType = -1;
//                else if (editUser.SubscriptionType == "Monthly")
//                    PaymentType = 0;
//                else if (editUser.SubscriptionType == "Quarterly")
//                    PaymentType = 1;
//                else if (editUser.SubscriptionType == "Halfyearly")
//                    PaymentType = 2;
//                else if (editUser.SubscriptionType == "Yearly")
//                    PaymentType = 3;
//                else if (editUser.SubscriptionType == "Lifetime")
//                    PaymentType = 4;

//                if (string.IsNullOrEmpty(editUser.SessionId))
//                    userObj.SessionId = null;
//                else
//                    userObj.SessionId = editUser.SessionId;
//                if (userObj.PaymentType != PaymentType)
//                {
//                    userObj.LastPaymentDate = userObj.LastPaymentDate;
//                    userObj.IsExpire = false;
//                    DateTime validTillDate = DateTime.Now;
//                    switch (PaymentType)
//                    {
//                        case -1:
//                            validTillDate = userObj.LastPaymentDate.AddDays(6);
//                            break;
//                        case 0:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(1);
//                            break;
//                        case 1:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(3);
//                            break;
//                        case 2:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(6);
//                            break;
//                        case 3:
//                            validTillDate = userObj.LastPaymentDate.AddYears(1);
//                            break;
//                        case 4:
//                            string body1 = string.Empty;
//                            string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_PackegeExtend.html");
//                            using (StreamReader reader = new StreamReader(path1))

//                            {

//                                body1 = reader.ReadToEnd();

//                            }
//                            body1 = body1.Replace("{name}", editUser.FirstName);

//                            body1 = body1.Replace("{subscription}", "Lifetime");

//                            string subject1 = "Subscription Extended ";
//                            Email.SendEmail(userObj.Email, subject1, body1);
//                            break;
//                        default:
//                            validTillDate = userObj.LastPaymentDate;
//                            break;
//                    }

//                    userObj.PaymentType = PaymentType;
//                    if (PaymentType != 4)
//                    {
//                        string body = string.Empty;
//                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_PackegeExtend.html");
//                        using (StreamReader reader = new StreamReader(path))

//                        {

//                            body = reader.ReadToEnd();

//                        }
//                        body = body.Replace("{name}", editUser.FirstName);

//                        body = body.Replace("{subscription}", validTillDate.ToString("dd-MM-yyyy"));

//                        string subject = "Subscription Extended ";
//                        Email.SendEmail(userObj.Email, subject, body);
//                    }

//                }
//                else
//                {
//                    userObj.PaymentType = PaymentType;
//                }

//                context.Entry(userObj).State = EntityState.Modified;

//                webpages_Membership membership = context.webpages_Membership.Find(editUser.Id);
//                membership.IsConfirmed = editUser.IsConfirmed;
//                context.Entry(membership).State = EntityState.Modified;



//                context.SaveChanges();
//                return true;

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Delete User
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public bool DeleteUser(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users userObj = context.Users.Find(id);
//                context.Users.Remove(userObj);

//                webpages_Membership MembershipObj = context.webpages_Membership.Find(id);
//                context.webpages_Membership.Remove(MembershipObj);

//                Invoice invoiceobj = context.Invoice.Where(x => x.UserId.Equals(id)).FirstOrDefault();
//                if (invoiceobj != null)
//                {
//                    context.Invoice.Remove(invoiceobj);
//                }


//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Save Associate
//        /// </summary>
//        /// <param name="associate"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        public bool SaveAssociate(SignUp associate, out string message)
//        {
//            message = "";
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    var data = (from us in context.Users
//                                where us.UserName.Equals(associate.UserName)
//                                select us).FirstOrDefault<Users>();
//                    if (data != null)
//                    {
//                        message = "UserName already exists. Please choose a different UserName";
//                        return false;
//                    }
//                    else
//                    {
//                        associate.CreatedDate = DateTime.Now;
//                        associate.LastUpadateDate = DateTime.Now;

//                        associate.SubscriptionType = "Demo";
//                        int PaymentType = -1;
//                        if (associate.SubscriptionType == "Demo")
//                            PaymentType = -1;
//                        else if (associate.SubscriptionType == "Monthly")
//                            PaymentType = 0;
//                        else if (associate.SubscriptionType == "Quarterly")
//                            PaymentType = 1;
//                        else if (associate.SubscriptionType == "Halfyearly")
//                            PaymentType = 2;
//                        else if (associate.SubscriptionType == "Yearly")
//                            PaymentType = 3;



//                        context.Users.Add(new Users()
//                        {
//                            CreatedDate = associate.CreatedDate,
//                            LastUpadateDate = associate.LastUpadateDate,
//                            FirstName = associate.FirstName,
//                            LastName = associate.LastName,
//                            MobileNumber = associate.MobileNumber,
//                            UserId = 0,
//                            UserName = associate.UserName,
//                            IsExpire = true,
//                            PaymentType = 0,
//                            LastPaymentDate = DateTime.Now,
//                            Email = associate.Email,
//                            IsAssociate = true,
//                            ParentAssociateCode = associate.ParentAssociateCode,
//                            AssociateCode = GetAssociateCode(),
//                            SubscriptionType = associate.SubscriptionType,
//                            IsAdmin = false
//                        });
//                        context.SaveChanges();
//                        var guid = Guid.NewGuid().ToString();
//                        var getuser = (from us in context.Users
//                                       where us.UserName.Equals(associate.UserName)
//                                       select us).FirstOrDefault<Users>();
//                        getuser.UserId = getuser.Id;
//                        context.Entry(getuser).State = EntityState.Modified;

//                        webpages_Membership membership = new webpages_Membership();
//                        membership.UserId = getuser.Id;
//                        membership.CreateDate = DateTime.Now;
//                        membership.PasswordFailuresSinceLastSuccess = 0;
//                        membership.IsConfirmed = false;
//                        membership.Password = PasswordHashUtill.HashPassword(associate.Password);
//                        membership.PasswordSalt = "0";
//                        membership.ConfirmationToken = guid;
//                        membership.PasswordChangedDate = DateTime.Now;
//                        context.webpages_Membership.Add(membership);
//                        context.SaveChanges();


//                        string body = string.Empty;
//                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Confirmation.html");
//                        using (StreamReader reader = new StreamReader(path))

//                        {

//                            body = reader.ReadToEnd();

//                        }
//                        body = body.Replace("{name}", associate.FirstName);

//                        body = body.Replace("{id}", getuser.Id.ToString());

//                        body = body.Replace("{guid}", guid);

//                        string subject = "Email Confirmation Mail";
//                        Email.SendEmail(associate.Email, subject, body);

//                        string body1 = string.Empty;
//                        string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Admin.html");
//                        using (StreamReader reader = new StreamReader(path1))

//                        {

//                            body1 = reader.ReadToEnd();

//                        }
//                        body1 = body1.Replace("{user}", "Associate");

//                        body1 = body1.Replace("{firstname}", associate.FirstName);

//                        body1 = body1.Replace("{username}", associate.UserName);

//                        body1 = body1.Replace("{email}", associate.Email);

//                        body1 = body1.Replace("{mobilenumber}", associate.MobileNumber);

//                        string subject1 = "New Registration";
//                        Email.SendEmail("welcome@bhagirathfincare.in", subject1, body1);

//                        message = "Successfully Sign up, Kindly check your email to confirm account";
//                        return true;

//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }

//        }

//        /// <summary>
//        /// GetAssociateCode
//        /// </summary>
//        /// <returns></returns>
//        private string GetAssociateCode()
//        {
//            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//            var random = new Random();
//            var result = new string(
//                Enumerable.Repeat(chars, 4)
//                          .Select(s => s[random.Next(s.Length)])
//                          .ToArray());
//            return result;
//        }

//        public bool EditAssociate(SignUp editAssociate)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                var id = editAssociate.Id;
//                Users userObj = context.Users.Find(id);
//                userObj.FirstName = editAssociate.FirstName;
//                userObj.LastName = editAssociate.LastName;
//                userObj.Email = editAssociate.Email;
//                userObj.MobileNumber = editAssociate.MobileNumber;
//                userObj.LastPaymentDate = editAssociate.LastPaymentDate;
//                userObj.IsExpire = editAssociate.IsExpire;
//                userObj.AllowedEquity = editAssociate.AllowedEquity;
//                userObj.AllowedCommodity = editAssociate.AllowedCommodity;
//                userObj.AllowedCurrency = editAssociate.AllowedCurrency;
//                userObj.SubscriptionType = editAssociate.SubscriptionType;


//                if (editAssociate.CreatedDate == null)
//                    userObj.CreatedDate = DateTime.Now;

//                userObj.LastUpadateDate = DateTime.Now;

//                int PaymentType = -1;
//                if (editAssociate.SubscriptionType == "Demo")
//                    PaymentType = -1;
//                else if (editAssociate.SubscriptionType == "Monthly")
//                    PaymentType = 0;
//                else if (editAssociate.SubscriptionType == "Quarterly")
//                    PaymentType = 1;
//                else if (editAssociate.SubscriptionType == "Halfyearly")
//                    PaymentType = 2;
//                else if (editAssociate.SubscriptionType == "Yearly")
//                    PaymentType = 3;
//                else if (editAssociate.SubscriptionType == "Lifetime")
//                    PaymentType = 4;

//                if (string.IsNullOrEmpty(editAssociate.SessionId))
//                    userObj.SessionId = null;
//                else
//                    userObj.SessionId = editAssociate.SessionId;

//                if (userObj.PaymentType != PaymentType)
//                {
//                    userObj.LastPaymentDate = editAssociate.LastPaymentDate;
//                    userObj.IsExpire = false;
//                    DateTime validTillDate = DateTime.Now;
//                    switch (PaymentType)
//                    {
//                        case -1:
//                            validTillDate = userObj.LastPaymentDate.AddDays(6);
//                            break;
//                        case 0:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(1);
//                            break;
//                        case 1:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(3);
//                            break;
//                        case 2:
//                            validTillDate = userObj.LastPaymentDate.AddMonths(6);
//                            break;
//                        case 3:
//                            validTillDate = userObj.LastPaymentDate.AddYears(1);
//                            break;
//                        case 4:
//                            string body1 = string.Empty;
//                            string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_PackegeExtend.html");
//                            using (StreamReader reader = new StreamReader(path1))

//                            {

//                                body1 = reader.ReadToEnd();

//                            }
//                            body1 = body1.Replace("{name}", editAssociate.FirstName);

//                            body1 = body1.Replace("{subscription}", "Lifetime");

//                            string subject1 = "Subscription Extended ";
//                            Email.SendEmail(userObj.Email, subject1, body1);
//                            break;
//                        default:
//                            validTillDate = userObj.LastPaymentDate;
//                            break;
//                    }
//                    userObj.PaymentType = PaymentType;
//                    if (PaymentType != 4)
//                    {
//                        string body = string.Empty;
//                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_PackegeExtend.html");
//                        using (StreamReader reader = new StreamReader(path))
//                        {
//                            body = reader.ReadToEnd();
//                        }
//                        body = body.Replace("{name}", editAssociate.FirstName);

//                        body = body.Replace("{subscription}", validTillDate.ToString("dd-MM-yyyy"));

//                        string subject = "Subscription Extended ";
//                        Email.SendEmail(userObj.Email, subject, body);
//                    }

//                }
//                else
//                {
//                    userObj.PaymentType = PaymentType;
//                }

//                context.Entry(userObj).State = EntityState.Modified;

//                webpages_Membership membership = context.webpages_Membership.Find(editAssociate.Id);
//                membership.IsConfirmed = editAssociate.IsConfirmed;
//                context.Entry(membership).State = EntityState.Modified;
//                context.SaveChanges();
//                return true;

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }
//        /// <summary>
//        /// Delete Associate
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public bool DeleteAssociate(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users associateObj = context.Users.Find(id);
//                context.Users.Remove(associateObj);

//                webpages_Membership WebpagesMembershipObj = context.webpages_Membership.Find(id);
//                context.webpages_Membership.Remove(WebpagesMembershipObj);

//                Invoice invoiceobj = context.Invoice.Where(x => x.UserId.Equals(id)).FirstOrDefault();
//                if (invoiceobj != null)
//                {
//                    context.Invoice.Remove(invoiceobj);
//                }
//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Login User
//        /// </summary>
//        /// <param name="login"></param>
//        /// <returns></returns>
//        public object LoginUser(Login login, out string message)
//        {
//            message = "";
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {

//                    var getuser = (from us in context.Users
//                                   where us.UserName.Equals(login.LoginUserName)
//                                   select us).FirstOrDefault<Users>();
//                    if (getuser != null)
//                    {
//                        if (getuser.SessionId == null || getuser.IsAdmin == true)
//                        {
//                            var u = getuser.UserId;

//                            webpages_Membership member = context.webpages_Membership.Where(x => x.UserId.Equals(u) && x.IsConfirmed == true).FirstOrDefault();
//                            if (member != null)
//                            {
//                                Users checkisexpire = context.Users.Where(x => x.Id.Equals(u)).FirstOrDefault();
//                                if (checkisexpire.IsExpire == true)
//                                {
//                                    message = "Your Account is not active, Please Contact to Administator.";
//                                    return null;
//                                }
//                                else
//                                {
//                                    DateTime validTillDate = checkisexpire.LastPaymentDate;
//                                    switch (checkisexpire.PaymentType)
//                                    {
//                                        case -1:
//                                            validTillDate = checkisexpire.LastPaymentDate.AddDays(6);
//                                            break;
//                                        case 0:
//                                            validTillDate = checkisexpire.LastPaymentDate.AddMonths(1);
//                                            break;
//                                        case 1:
//                                            validTillDate = checkisexpire.LastPaymentDate.AddMonths(3);
//                                            break;
//                                        case 2:
//                                            validTillDate = checkisexpire.LastPaymentDate.AddMonths(6);
//                                            break;
//                                        case 3:
//                                            validTillDate = checkisexpire.LastPaymentDate.AddYears(1);
//                                            break;
//                                        default:
//                                            validTillDate = checkisexpire.LastPaymentDate;
//                                            break;
//                                    }
//                                    if (DateTime.Now > validTillDate && checkisexpire.IsAdmin != true && checkisexpire.PaymentType != 4)
//                                    {
//                                        checkisexpire.IsExpire = true;
//                                        context.Entry(checkisexpire).State = EntityState.Modified;
//                                        context.SaveChanges();
//                                        //mail to user
//                                        string body = string.Empty;
//                                        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_ExpireUser.html");
//                                        using (StreamReader reader = new StreamReader(path))
//                                        {
//                                            body = reader.ReadToEnd();
//                                        }
//                                        body = body.Replace("{name}", getuser.FirstName);
//                                        string subject = "Account Expire Mail";
//                                        Email.SendEmail(getuser.Email, subject, body);

//                                        //mail to admin
//                                        string body1 = string.Empty;
//                                        string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_ExpireUserToAdmin.html");
//                                        using (StreamReader reader = new StreamReader(path1))
//                                        {
//                                            body1 = reader.ReadToEnd();
//                                        }
//                                        body1 = body1.Replace("{username}", getuser.UserName);
//                                        body1 = body1.Replace("{mobilenumber}", getuser.MobileNumber);
//                                        body1 = body1.Replace("{email}", getuser.Email);
//                                        string subject1 = "User Account Expire Mail";
//                                        Email.SendEmail("welcome@bhagirathfincare.in", subject1, body1);
//                                        message = "Your Package has expired. Kindly Contact Administator.";
//                                        return null;
//                                    }
//                                    else
//                                    {
//                                        var pass = member.Password;
//                                        if (PasswordHashUtill.VerifyHashedPassword(pass, login.LoginPassword))
//                                        {
//                                            UserInfo userinfo = new UserInfo();

//                                            userinfo = new UserInfo()
//                                            {
//                                                Id = getuser.Id,
//                                                UserId = getuser.UserId,
//                                                FirstName = getuser.FirstName,
//                                                LastName = getuser.LastName,
//                                                UserName = getuser.UserName,
//                                                MobileNumber = getuser.MobileNumber,
//                                                SessionId = getuser.SessionId,
//                                                CreatedDate = getuser.CreatedDate,
//                                                LastUpadateDate = getuser.LastUpadateDate,
//                                                IsExpire = getuser.IsExpire,
//                                                LastPaymentDate = getuser.LastPaymentDate,
//                                                Email = getuser.Email,
//                                                SubscriptionType = getuser.SubscriptionType,
//                                                IsAssociate = getuser.IsAssociate,
//                                                ParentAssociateCode = getuser.ParentAssociateCode,
//                                                AssociateCode = getuser.AssociateCode,
//                                                BirthDate = getuser.BirthDate,
//                                                AllowedEquity = getuser.AllowedEquity,
//                                                AllowedCommodity = getuser.AllowedCommodity,
//                                                AllowedCurrency = getuser.AllowedCurrency,
//                                                AllowedHotStocks = getuser.AllowedHotStocks,
//                                                AllowedSuperCallsEquity = getuser.AllowedSuperCallsEquity,
//                                                AllowedSuperCallsCommodity = getuser.AllowedSuperCallsCommodity,
//                                                AllowedSuperCallsCurrency = getuser.AllowedSuperCallsCurrency,
//                                                IPAddress = getuser.IPAddress,
//                                                IsAdmin = getuser.IsAdmin,
//                                            };

//                                            if (checkisexpire.PaymentType == 4)
//                                            {
//                                                getuser.LastUpadateDate = DateTime.Now;
//                                                context.Entry(getuser).State = EntityState.Modified;
//                                                context.SaveChanges();
//                                                message = "Successfully Login";
//                                                return userinfo;

//                                            }
//                                            else
//                                            {
//                                                var noofdays = Convert.ToInt16((validTillDate.Date - DateTime.Now.Date).TotalDays);
//                                                userinfo.NoOfDays = noofdays;

//                                                getuser.LastUpadateDate = DateTime.Now;
//                                                context.Entry(getuser).State = EntityState.Modified;
//                                                context.SaveChanges();
//                                                message = "Successfully Login";
//                                                return userinfo;
//                                            }




//                                        }
//                                        else
//                                        {
//                                            message = "Incorrect Password";
//                                            return null;
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                message = "Please Confirm your email first";
//                                return null;
//                            }

//                        }
//                        else
//                        {
//                            message = "You are already login";
//                            return null;
//                        }

//                    }
//                    else
//                    {
//                        message = "Username Not Found";
//                        return null;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                message = "Something Went Wrong";
//                return null;

//            }
//        }


//        /// <summary>
//        /// Change Password
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        public bool ChangePassword(LocalPasswordModel model)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();

//                var Result = (from membership in context.webpages_Membership
//                              where membership.UserId.Equals(model.UserId)
//                              select membership).FirstOrDefault<webpages_Membership>();

//                if (Result != null)
//                {

//                    if (PasswordHashUtill.VerifyHashedPassword(Result.Password, model.OldPassword))
//                    {

//                        Result.Password = PasswordHashUtill.HashPassword(model.NewPassword);
//                        Result.PasswordChangedDate = DateTime.Now;
//                        context.Entry(Result).State = EntityState.Modified;
//                        context.SaveChanges();
//                        return true;
//                    }

//                    else
//                    {
//                        return false;
//                    }

//                }
//                else
//                {
//                    return false;
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        public bool forgotpassword(string username)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users result = context.Users.Where(x => x.UserName.Equals(username)).FirstOrDefault();
//                if (result != null)
//                {
//                    webpages_Membership member = context.webpages_Membership.Where(x => x.UserId.Equals(result.Id)).FirstOrDefault();

//                    string body = string.Empty;
//                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Forgotpassword.html");
//                    using (StreamReader reader = new StreamReader(path))

//                    {

//                        body = reader.ReadToEnd();

//                    }
//                    body = body.Replace("{name}", result.FirstName);

//                    body = body.Replace("{userid}", member.UserId.ToString());

//                    body = body.Replace("{guid}", member.ConfirmationToken.ToString());

//                    string subject = "Forgot Password Confirmation Mail";
//                    Email.SendEmail(result.Email, subject, body);

//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        public bool renew(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users result = context.Users.Find(id);
//                result.IsExpire = false;
//                context.Entry(result).State = EntityState.Modified;
//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        public object UserConfirmation(int Id, string Guid)
//        {
//            try
//            {
//                BhagirathFinCareEntities db = new BhagirathFinCareEntities();
//                webpages_Membership result = db.webpages_Membership.Where(x => x.UserId.Equals(Id) && x.ConfirmationToken.Equals(Guid)).FirstOrDefault();
//                if (result != null && result.IsConfirmed == false)
//                {
//                    result.IsConfirmed = true;
//                    db.Entry(result).State = EntityState.Modified;
//                    db.SaveChanges();
//                    // webpages_Membership data = db.webpages_Membership.Where(x => x.UserId.Equals(Id)).FirstOrDefault();
//                    webpages_Membership data1 = new webpages_Membership()
//                    {
//                        IsConfirmed = false
//                    };
//                    return data1;
//                }
//                else
//                {
//                    webpages_Membership data = db.webpages_Membership.Where(x => x.UserId.Equals(Id)).FirstOrDefault();
//                    return data;
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        public bool ResetPassword(long id, string newpassword)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                webpages_Membership result = context.webpages_Membership.Find(id);
//                result.Password = PasswordHashUtill.HashPassword(newpassword); ;
//                result.PasswordChangedDate = DateTime.Now;

//                context.Entry(result).State = EntityState.Modified;
//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        public object GetParticularAssociateUsers(SignUp user)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                List<Users> data = context.Users.Where(x => x.ParentAssociateCode.Equals(user.AssociateCode)).ToList();
//                if (data != null)
//                {
//                    return data;
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        public object ChartForAssociate(string associatecode, DateTime startdate, DateTime enddate)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                enddate = enddate.AddHours(23).AddMinutes(59).AddSeconds(59);

//                string sqlQuery;
//                SqlParameter[] sqlParams;

//                sqlQuery = "GetChartData @associateCode, @startDate,@endDate";

//                sqlParams = new SqlParameter[]
//                {
//                new SqlParameter { ParameterName = "@associateCode",  Value =associatecode , Direction = System.Data.ParameterDirection.Input,SqlDbType=SqlDbType.NVarChar},
//                new SqlParameter { ParameterName = "@startDate",  Value =startdate, Direction = System.Data.ParameterDirection.Input,SqlDbType=SqlDbType.DateTime },
//                new SqlParameter { ParameterName = "@endDate",  Value =enddate, Direction = System.Data.ParameterDirection.Input,SqlDbType=SqlDbType.DateTime },
//                };

//                using (BhagirathFinCareEntities dbcontext = new BhagirathFinCareEntities())
//                {
//                    List<ChartData> data = dbcontext.Database.SqlQuery<ChartData>(sqlQuery, sqlParams).ToList();
//                    //var data = dbContext.Database.ExecuteSqlCommand(sqlQuery, sqlParams);
//                    return data;
//                }


//                // var data = context.GetChartData(associatecode, startdate, enddate);
//                //return null;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        public object GetUserForAssociate(string associatecode, DateTime startdate, DateTime enddate)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                enddate = enddate.AddDays(1);
//                List<Users> data = context.Users.Where(x => x.ParentAssociateCode.Equals(associatecode) && x.CreatedDate >= startdate && x.CreatedDate <= enddate).ToList();
//                if (data != null)
//                {
//                    return data;
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        //public bool CalculateInvoice(InvoiceViewModel user, out string message)
//        //{
//        //    double subtotal = 0;
//        //    double discount = 0;
//        //    double gst = 0;
//        //    double grandtotal = 0;
//        //    string subscription = "";
//        //    string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddhhmmss");
//        //    message = "";
//        //    try
//        //    {
//        //        BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//        //        if (user.SubscriptionType == "Demo")
//        //        {
//        //            message = "Kindly Change Subscription type.";
//        //            return false;
//        //        }
//        //        else
//        //        {
//        //            if (user.SubscriptionType == "Monthly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 8925;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Quarterly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 25200;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "HalfYearly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 47250;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Yearly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 88200;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Lifetime")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 94999;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }


//        //            if (user.Gstper != 0)
//        //            {
//        //                gst = subtotal * (user.Gstper / 100);
//        //            }
//        //            double totalamount = 0;
//        //            if (user.Discountper != 0)
//        //            {
//        //                totalamount = user.Discountper;

//        //            }

//        //            grandtotal = subtotal - totalamount + gst;

//        //            Invoice invoice = new Invoice();
//        //            invoice.UserId = user.UserId;
//        //            //invoice.InvoiceNo = invoiceNo;
//        //            invoice.FirstName = user.FirstName;
//        //            invoice.LastName = user.LastName;
//        //            invoice.UserName = user.UserName;
//        //            invoice.Email = user.Email;
//        //            invoice.MobileNumber = user.MobileNumber;
//        //            invoice.SubscriptionType = user.SubscriptionType;
//        //            invoice.AllowedEquity = user.AllowedEquity;
//        //            invoice.AllowedCommodity = user.AllowedCommodity;
//        //            invoice.AllowedCurrency = user.AllowedCurrency;
//        //            invoice.SubTotal = subtotal;
//        //            invoice.Gstper = user.Gstper;
//        //            invoice.Discountper = user.Discountper;
//        //            invoice.GrandTotal = grandtotal;
//        //            invoice.IsPaid = false;
//        //            invoice.CreatedDate = DateTime.Now;
//        //            invoice.LastUpdatedDate = DateTime.Now;
//        //            context.Invoice.Add(invoice);
//        //            context.SaveChanges();


//        //            string body = string.Empty;


//        //            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Invoice.html");
//        //            using (StreamReader reader = new StreamReader(path))

//        //            {

//        //                body = reader.ReadToEnd();

//        //            }
//        //            body = body.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//        //            body = body.Replace("{invoiceno}", invoiceNo);
//        //            body = body.Replace("{name}", invoice.FirstName);

//        //            body = body.Replace("{transaction}", "Unpaid");
//        //            body = body.Replace("{string}", subscription);
//        //            body = body.Replace("{subscription}", user.SubscriptionType);
//        //            body = body.Replace("{subtotal}", subtotal.ToString());
//        //            body = body.Replace("{gst}", user.Gstper.ToString());
//        //            body = body.Replace("{discount}", user.Discountper.ToString());
//        //            body = body.Replace("{total}", grandtotal.ToString());

//        //            string subject = "Unpaid Invoice Mail";
//        //            Email.SendEmail(user.Email, subject, body);

//        //            string body1 = string.Empty;
//        //            string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_AdminInvoice.html");
//        //            using (StreamReader reader = new StreamReader(path1))

//        //            {

//        //                body1 = reader.ReadToEnd();

//        //            }
//        //            body1 = body1.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//        //            body1 = body1.Replace("{invoiceno}", invoiceNo);
//        //            body1 = body1.Replace("{firstname}", invoice.FirstName);
//        //            body1 = body1.Replace("{lastname}", invoice.LastName);

//        //            body1 = body1.Replace("{transaction}", "Unpaid");
//        //            body1 = body1.Replace("{string}", subscription);
//        //            body1 = body1.Replace("{subscription}", user.SubscriptionType);
//        //            body1 = body1.Replace("{subtotal}", subtotal.ToString());
//        //            body1 = body1.Replace("{gst}", user.Gstper.ToString());
//        //            body1 = body1.Replace("{discount}", user.Discountper.ToString());
//        //            body1 = body1.Replace("{total}", grandtotal.ToString());

//        //            string subject1 = "Unpaid Invoice Mail";
//        //            Email.SendEmail("welcome@bhagirathfincare.in", subject1, body1);

//        //            message = "Unpaid Invoice Send Successfully.";
//        //            return true;
//        //        }

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        ex.LogError(this);
//        //        message = ex.Message;
//        //        return false;
//        //    }
//        //}

//        //public bool CalculateInvoice(InvoiceViewModel user, out string message)
//        //{
//        //    double subtotal = 0;
//        //    double discount = 0;
//        //    double gst = 0;
//        //    double grandtotal = 0;
//        //    string subscription = "";
//        //    string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddhhmmss");
//        //    message = "";
//        //    try
//        //    {
//        //        BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//        //        if (user.SubscriptionType == "Demo")
//        //        {
//        //            message = "Kindly Change Subscription type.";
//        //            return false;
//        //        }
//        //        else
//        //        {
//        //            if (user.SubscriptionType == "Monthly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 3500;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 6300;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 8925;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Quarterly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 9450;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 25200;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "HalfYearly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 17850;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 47250;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Yearly")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 33600;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 52500;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 88200;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }
//        //            else if (user.SubscriptionType == "Lifetime")
//        //            {
//        //                if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Equity";
//        //                }
//        //                else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Commodity";
//        //                }
//        //                else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//        //                {
//        //                    subtotal = subtotal + 49999;
//        //                    subscription = "Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Equity Commodity";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Equity Currency";
//        //                }
//        //                else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 74999;
//        //                    subscription = "Commodity Currency";
//        //                }
//        //                else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//        //                {
//        //                    subtotal = subtotal + 94999;
//        //                    subscription = "Equity Commodity Currency";
//        //                }
//        //            }


//        //            if (user.Gstper != 0)
//        //            {
//        //                gst = subtotal * (user.Gstper / 100);
//        //            }
//        //            double totalamount = 0;
//        //            if (user.Discountper != 0)
//        //            {
//        //                totalamount = user.Discountper;

//        //            }

//        //            grandtotal = subtotal - totalamount + gst;

//        //            Invoice invoice = new Invoice();
//        //            invoice.UserId = user.UserId;
//        //            //invoice.InvoiceNo = invoiceNo;
//        //            invoice.FirstName = user.FirstName;
//        //            invoice.LastName = user.LastName;
//        //            invoice.UserName = user.UserName;
//        //            invoice.Email = user.Email;
//        //            invoice.MobileNumber = user.MobileNumber;
//        //            invoice.SubscriptionType = user.SubscriptionType;
//        //            invoice.AllowedEquity = user.AllowedEquity;
//        //            invoice.AllowedCommodity = user.AllowedCommodity;
//        //            invoice.AllowedCurrency = user.AllowedCurrency;
//        //            invoice.SubTotal = subtotal;
//        //            invoice.Gstper = user.Gstper;
//        //            invoice.Discountper = user.Discountper;
//        //            invoice.GrandTotal = grandtotal;
//        //            invoice.IsPaid = false;
//        //            invoice.CreatedDate = DateTime.Now;
//        //            invoice.LastUpdatedDate = DateTime.Now;
//        //            context.Invoice.Add(invoice);
//        //            context.SaveChanges();


//        //            string body = string.Empty;
//        //            // StringBuilder sb = new StringBuilder();

//        //            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_Invoice.html");
//        //            using (StreamReader reader = new StreamReader(path))

//        //            {

//        //                body = reader.ReadToEnd();

//        //            }
//        //            body = body.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//        //            body = body.Replace("{invoiceno}", invoiceNo);
//        //            body = body.Replace("{name}", invoice.FirstName);

//        //            body = body.Replace("{transaction}", "Unpaid");
//        //            body = body.Replace("{string}", subscription);
//        //            body = body.Replace("{subscription}", user.SubscriptionType);
//        //            body = body.Replace("{subtotal}", subtotal.ToString());
//        //            body = body.Replace("{gst}", user.Gstper.ToString());
//        //            body = body.Replace("{discount}", user.Discountper.ToString());
//        //            body = body.Replace("{total}", grandtotal.ToString());

//        //            StringBuilder sa = new StringBuilder(body);
//        //            StringReader sr = new StringReader(sa.ToString());
//        //            // StringReader sa = new StringReader("<p>Hello World</p>");
//        //            string path0 = Path.Combine("F:\\Github_Project\\Bhagirath\\BhagirathPhase3\\BhagirathFincareApi\\Invoices\\", invoiceNo + ".pdf");
//        //            var output = new FileStream(path0, FileMode.Create);
//        //            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//        //            // pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4);
//        //            pdfDoc.SetMargins(10F, 10F, 10F, 10F);

//        //            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
//        //            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, output);
//        //            pdfDoc.Open();
//        //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
//        //            pdfDoc.Close();
//        //            string subject = "Unpaid Invoice Mail";
//        //            Email.SendInvoiceEmail(user.Email, subject, path0);





//        //            string body1 = string.Empty;
//        //            string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/EmailForm_AdminInvoice.html");
//        //            using (StreamReader reader = new StreamReader(path1))

//        //            {

//        //                body1 = reader.ReadToEnd();

//        //            }
//        //            body1 = body1.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//        //            body1 = body1.Replace("{invoiceno}", invoiceNo);
//        //            body1 = body1.Replace("{firstname}", invoice.FirstName);
//        //            body1 = body1.Replace("{lastname}", invoice.LastName);

//        //            body1 = body1.Replace("{transaction}", "Unpaid");
//        //            body1 = body1.Replace("{string}", subscription);
//        //            body1 = body1.Replace("{subscription}", user.SubscriptionType);
//        //            body1 = body1.Replace("{subtotal}", subtotal.ToString());
//        //            body1 = body1.Replace("{gst}", user.Gstper.ToString());
//        //            body1 = body1.Replace("{discount}", user.Discountper.ToString());
//        //            body1 = body1.Replace("{total}", grandtotal.ToString());

//        //            string subject1 = "Unpaid Invoice Mail";
//        //            Email.SendEmail("welcome@bhagirathfincare.in", subject1, body1);

//        //            message = "Unpaid Invoice Send Successfully.";
//        //            return true;
//        //        }

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        ex.LogError(this);
//        //        message = ex.Message;
//        //        return false;
//        //    }
//        //}

//        public bool CalculateInvoice(InvoiceViewModel user, out string message)
//        {
//            double subtotal = 0;
//            double discount = 0;
//            double gst = 0;
//            double grandtotal = 0;
//            string subscription = "";
//            string body = string.Empty;
//            string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddhhmmss");
//            message = "";
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                if (user.SubscriptionType == "Demo")
//                {
//                    message = "Kindly Change Subscription type.";
//                    return false;
//                }
//                else
//                {
//                    if (user.SubscriptionType == "Monthly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 8925;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = user.LastPaymentDate.AddMonths(1);
//                    }
//                    else if (user.SubscriptionType == "Quarterly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 25200;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = user.LastPaymentDate.AddMonths(3);
//                    }
//                    else if (user.SubscriptionType == "HalfYearly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 47250;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = user.LastPaymentDate.AddMonths(6);
//                    }
//                    else if (user.SubscriptionType == "Yearly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 52500;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 52500;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 52500;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 88200;
//                            subscription = "Equity Commodity Currency";
//                        }
//                        user.InvoiceValidity = user.LastPaymentDate.AddYears(1);
//                    }
//                    else if (user.SubscriptionType == "Lifetime")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 94999;
//                            subscription = "Equity Commodity Currency";
//                        }
//                    }



//                    if (user.Gstper != 0)
//                    {
//                        gst = subtotal * (user.Gstper / 100);
//                    }
//                    double totalamount = 0;
//                    if (user.Discountper != 0)
//                    {
//                        totalamount = user.Discountper;

//                    }

//                    grandtotal = subtotal - totalamount + gst;

//                    Invoice invoice = new Invoice();
//                    invoice.UserId = user.UserId;
//                    //invoice.InvoiceNo = invoiceNo;
//                    invoice.FirstName = user.FirstName;
//                    invoice.LastName = user.LastName;
//                    invoice.UserName = user.UserName;
//                    invoice.Email = user.Email;
//                    invoice.MobileNumber = user.MobileNumber;
//                    invoice.SubscriptionType = user.SubscriptionType;
//                    invoice.AllowedEquity = user.AllowedEquity;
//                    invoice.AllowedCommodity = user.AllowedCommodity;
//                    invoice.AllowedCurrency = user.AllowedCurrency;
//                    invoice.SubTotal = subtotal;
//                    invoice.Gstper = user.Gstper;
//                    invoice.Discountper = user.Discountper;
//                    invoice.GrandTotal = grandtotal;
//                    invoice.IsPaid = false;
//                    invoice.CreatedDate = DateTime.Now;
//                    invoice.LastUpdatedDate = user.LastPaymentDate;
//                    context.Invoice.Add(invoice);
//                    context.SaveChanges();

//                    // StringBuilder sb = new StringBuilder();
//                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index.html");
//                    string cssText = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/style.css"));
//                    using (StreamReader reader = new StreamReader(path))

//                    {

//                        body = reader.ReadToEnd();

//                    }
//                    body = body.Replace("{date}", user.LastPaymentDate.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{invoiceno}", invoiceNo);
//                    body = body.Replace("{name}", invoice.FirstName);
//                    body = body.Replace("{transaction}", "Unpaid");
//                    body = body.Replace("{string}", subscription);
//                    body = body.Replace("{subscription}", user.SubscriptionType);
//                    body = body.Replace("{subtotal}", subtotal.ToString());
//                    body = body.Replace("{gst}", user.Gstper.ToString());
//                    body = body.Replace("{discount}", user.Discountper.ToString());
//                    body = body.Replace("{total}", grandtotal.ToString());

//                    string locationfolder = System.Web.HttpContext.Current.Server.MapPath("~/Invoices"); ;
//                    if (!Directory.Exists(locationfolder))
//                    {
//                        System.IO.Directory.CreateDirectory(locationfolder);
//                    }
//                    StringBuilder sa = new StringBuilder(body);
//                    var myString = sa.ToString();
//                    var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);
//                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc.SetMargins(10F, 10F, 10F, 10F);
//                    string path0 = Path.Combine(locationfolder, invoiceNo + ".pdf");
//                    var output = new FileStream(path0, FileMode.Create);
//                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, output);
//                    pdfDoc.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc.Close();
//                    string subject = "Unpaid Invoice Mail";
//                    Email.SendInvoiceEmail(user.Email, subject, path0, user.FirstName, invoiceNo);


//                    string body1 = string.Empty;
//                    string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index2.html");
//                    using (StreamReader reader = new StreamReader(path1))

//                    {

//                        body1 = reader.ReadToEnd();

//                    }
//                    body1 = body1.Replace("{date}", user.LastPaymentDate.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{invoiceno}", invoiceNo);
//                    body1 = body1.Replace("{firstname}", invoice.FirstName);
//                    body1 = body1.Replace("{lastname}", invoice.LastName);

//                    body1 = body1.Replace("{transaction}", "Unpaid");
//                    body1 = body1.Replace("{string}", subscription);
//                    body1 = body1.Replace("{subscription}", user.SubscriptionType);
//                    body1 = body1.Replace("{subtotal}", subtotal.ToString());
//                    body1 = body1.Replace("{gst}", user.Gstper.ToString());
//                    body1 = body1.Replace("{discount}", user.Discountper.ToString());
//                    body1 = body1.Replace("{total}", grandtotal.ToString());

//                    StringBuilder sa1 = new StringBuilder(body1);
//                    var myString1 = sa1.ToString();
//                    var myByteArray1 = System.Text.Encoding.UTF8.GetBytes(myString1);
//                    iTextSharp.text.Document pdfDoc1 = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc1.SetMargins(10F, 10F, 10F, 10F);
//                    string path01 = System.Web.HttpContext.Current.Server.MapPath("~/Invoices");
//                    var file = Directory.EnumerateFiles(path01).SingleOrDefault(f => f.Contains(invoiceNo));
//                    var output1 = new FileStream(file, FileMode.Create);
//                    PdfWriter writer1 = PdfWriter.GetInstance(pdfDoc1, output1);
//                    pdfDoc1.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray1))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer1, pdfDoc1, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc1.Close();
//                    string subject1 = "Unpaid Invoice Mail";
//                    Email.SendInvoiceEmail("welcome@bhagirathfincare.in", subject1, file, user.FirstName + " " + user.LastName, invoiceNo);

//                    message = "Unpaid Invoice Send Successfully.";
//                    return true;
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                message = ex.Message;
//                return false;
//            }
//        }

//        public bool ResendInvoice(InvoiceViewModel user, out string message)
//        {
//            double subtotal = 0;
//            double discount = 0;
//            double gst = 0;
//            double grandtotal = 0;
//            string subscription = "";
//            string body = string.Empty;
//            string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddhhmmss");
//            message = "";
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                var lastUpdatedDate = context.Invoice.Where(x => x.UserId == user.UserId).Select(x => x.LastUpdatedDate).FirstOrDefault();

//                if (user.SubscriptionType == "Demo")
//                {
//                    message = "Kindly Change Subscription type.";
//                    return false;
//                }
//                else
//                {
//                    if (user.SubscriptionType == "Monthly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 3500;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 6300;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 8925;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = lastUpdatedDate.AddMonths(1);
//                    }
//                    else if (user.SubscriptionType == "Quarterly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 9450;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 25200;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = lastUpdatedDate.AddMonths(3);
//                    }
//                    else if (user.SubscriptionType == "HalfYearly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 17850;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 47250;
//                            subscription = "Equity Commodity Currency";
//                        }

//                        user.InvoiceValidity = lastUpdatedDate.AddMonths(6);
//                    }
//                    else if (user.SubscriptionType == "Yearly")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 33600;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 52500;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 52500;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 52500;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != 0)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 88200;
//                            subscription = "Equity Commodity Currency";
//                        }
//                        user.InvoiceValidity = lastUpdatedDate.AddYears(1);
//                    }
//                    else if (user.SubscriptionType == "Lifetime")
//                    {
//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 49999;
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 74999;
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            if (user.CustomizedPrice != null)
//                                subtotal = user.CustomizedPrice;
//                            else
//                                subtotal = subtotal + 94999;
//                            subscription = "Equity Commodity Currency";
//                        }
//                    }



//                    if (user.Gstper != 0)
//                    {
//                        gst = subtotal * (user.Gstper / 100);
//                    }
//                    double totalamount = 0;
//                    if (user.Discountper != 0)
//                    {
//                        totalamount = user.Discountper;

//                    }

//                    grandtotal = subtotal - totalamount + gst;
//                    Invoice invoice = new Invoice();
//                    invoice.UserId = user.UserId;
//                    //invoice.InvoiceNo = invoiceNo;
//                    invoice.FirstName = user.FirstName;
//                    invoice.LastName = user.LastName;
//                    invoice.UserName = user.UserName;
//                    invoice.Email = user.Email;
//                    invoice.MobileNumber = user.MobileNumber;
//                    invoice.SubscriptionType = user.SubscriptionType;
//                    invoice.AllowedEquity = user.AllowedEquity;
//                    invoice.AllowedCommodity = user.AllowedCommodity;
//                    invoice.AllowedCurrency = user.AllowedCurrency;
//                    invoice.SubTotal = subtotal;
//                    invoice.Gstper = user.Gstper;
//                    invoice.Discountper = user.Discountper;
//                    invoice.GrandTotal = grandtotal;
//                    invoice.IsPaid = false;
//                    invoice.CreatedDate = DateTime.Now;
//                    invoice.LastUpdatedDate = lastUpdatedDate;
//                    context.Invoice.Add(invoice);
//                    context.SaveChanges();

//                    // StringBuilder sb = new StringBuilder();
//                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index.html");
//                    string cssText = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/style.css"));
//                    using (StreamReader reader = new StreamReader(path))

//                    {

//                        body = reader.ReadToEnd();

//                    }
//                    body = body.Replace("{date}", lastUpdatedDate.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{invoiceno}", invoiceNo);
//                    body = body.Replace("{name}", invoice.FirstName);
//                    body = body.Replace("{transaction}", "Unpaid");
//                    body = body.Replace("{string}", subscription);
//                    body = body.Replace("{subscription}", user.SubscriptionType);
//                    body = body.Replace("{subtotal}", subtotal.ToString());
//                    body = body.Replace("{gst}", user.Gstper.ToString());
//                    body = body.Replace("{discount}", user.Discountper.ToString());
//                    body = body.Replace("{total}", grandtotal.ToString());

//                    string locationfolder = System.Web.HttpContext.Current.Server.MapPath("~/Invoices"); ;
//                    if (!Directory.Exists(locationfolder))
//                    {
//                        System.IO.Directory.CreateDirectory(locationfolder);
//                    }
//                    StringBuilder sa = new StringBuilder(body);
//                    var myString = sa.ToString();
//                    var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);
//                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc.SetMargins(10F, 10F, 10F, 10F);
//                    string path0 = Path.Combine(locationfolder, invoiceNo + ".pdf");
//                    var output = new FileStream(path0, FileMode.Create);
//                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, output);
//                    pdfDoc.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc.Close();
//                    string subject = "Unpaid Invoice Mail";
//                    Email.SendInvoiceEmail(user.Email, subject, path0, user.FirstName, invoiceNo);


//                    string body1 = string.Empty;
//                    string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index2.html");
//                    using (StreamReader reader = new StreamReader(path1))

//                    {

//                        body1 = reader.ReadToEnd();

//                    }
//                    body1 = body1.Replace("{date}", lastUpdatedDate.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{invoiceno}", invoiceNo);
//                    body1 = body1.Replace("{firstname}", invoice.FirstName);
//                    body1 = body1.Replace("{lastname}", invoice.LastName);

//                    body1 = body1.Replace("{transaction}", "Unpaid");
//                    body1 = body1.Replace("{string}", subscription);
//                    body1 = body1.Replace("{subscription}", user.SubscriptionType);
//                    body1 = body1.Replace("{subtotal}", subtotal.ToString());
//                    body1 = body1.Replace("{gst}", user.Gstper.ToString());
//                    body1 = body1.Replace("{discount}", user.Discountper.ToString());
//                    body1 = body1.Replace("{total}", grandtotal.ToString());

//                    StringBuilder sa1 = new StringBuilder(body1);
//                    var myString1 = sa1.ToString();
//                    var myByteArray1 = System.Text.Encoding.UTF8.GetBytes(myString1);
//                    iTextSharp.text.Document pdfDoc1 = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc1.SetMargins(10F, 10F, 10F, 10F);
//                    string path01 = System.Web.HttpContext.Current.Server.MapPath("~/Invoices");
//                    var file = Directory.EnumerateFiles(path01).SingleOrDefault(f => f.Contains(invoiceNo));
//                    var output1 = new FileStream(file, FileMode.Create);
//                    PdfWriter writer1 = PdfWriter.GetInstance(pdfDoc1, output1);
//                    pdfDoc1.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray1))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer1, pdfDoc1, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc1.Close();
//                    string subject1 = "Unpaid Invoice Mail";
//                    Email.SendInvoiceEmail("welcome@bhagirathfincare.in", subject1, file, user.FirstName + " " + user.LastName, invoiceNo);

//                    message = "Unpaid Invoice Send Successfully.";
//                    return true;
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                message = ex.Message;
//                return false;
//            }
//        }

//        public object ReCalculateInvoice(InvoiceViewModel invoice, out string message)
//        {
//            double subtotal = 0;
//            double discount = 0;
//            double gst = 0;
//            double grandtotal = 0;
//            message = "";
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                if (invoice.SubscriptionType == "Demo")
//                {
//                    message = "Kindly Change Subscription type.";
//                    return false;
//                }
//                else
//                {
//                    if (invoice.SubscriptionType == "Monthly")
//                    {
//                        if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 3500;
//                        }
//                        else if (invoice.AllowedCommodity == true && invoice.AllowedEquity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 3500;
//                        }
//                        else if (invoice.AllowedCurrency == true && invoice.AllowedEquity == false && invoice.AllowedCommodity == false)
//                        {
//                            subtotal = subtotal + 3500;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 6300;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 6300;
//                        }
//                        else if (invoice.AllowedEquity == false && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 6300;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 8925;
//                        }
//                    }
//                    else if (invoice.SubscriptionType == "Quarterly")
//                    {
//                        if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 9450;
//                        }
//                        else if (invoice.AllowedCommodity == true && invoice.AllowedEquity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 9450;
//                        }
//                        else if (invoice.AllowedCurrency == true && invoice.AllowedEquity == false && invoice.AllowedCommodity == false)
//                        {
//                            subtotal = subtotal + 9450;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedEquity == false && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 25200;
//                        }
//                    }
//                    else if (invoice.SubscriptionType == "Halfyearly")
//                    {
//                        if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedCommodity == true && invoice.AllowedEquity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedCurrency == true && invoice.AllowedEquity == false && invoice.AllowedCommodity == false)
//                        {
//                            subtotal = subtotal + 17850;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedEquity == false && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 47250;
//                        }
//                    }
//                    else if (invoice.SubscriptionType == "Yearly")
//                    {
//                        if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedCommodity == true && invoice.AllowedEquity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedCurrency == true && invoice.AllowedEquity == false && invoice.AllowedCommodity == false)
//                        {
//                            subtotal = subtotal + 33600;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 52500;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 52500;
//                        }
//                        else if (invoice.AllowedEquity == false && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 52500;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 88200;
//                        }
//                    }
//                    else if (invoice.SubscriptionType == "Lifetime")
//                    {
//                        if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 49999;
//                        }
//                        else if (invoice.AllowedCommodity == true && invoice.AllowedEquity == false && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 49999;
//                        }
//                        else if (invoice.AllowedCurrency == true && invoice.AllowedEquity == false && invoice.AllowedCommodity == false)
//                        {
//                            subtotal = subtotal + 49999;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == false)
//                        {
//                            subtotal = subtotal + 74999;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == false && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 74999;
//                        }
//                        else if (invoice.AllowedEquity == false && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 74999;
//                        }
//                        else if (invoice.AllowedEquity == true && invoice.AllowedCommodity == true && invoice.AllowedCurrency == true)
//                        {
//                            subtotal = subtotal + 94999;
//                        }
//                    }


//                    if (invoice.Gstper != 0)
//                    {
//                        gst = subtotal * (invoice.Gstper / 100);
//                    }
//                    double totalamount = 0;
//                    if (invoice.Discountper != 0)
//                    {
//                        totalamount = invoice.Discountper;

//                    }

//                    grandtotal = subtotal - totalamount + gst;
//                    invoice.SubTotal = subtotal;
//                    invoice.GrandTotal = grandtotal;


//                    message = "Data Calculated Successfully";
//                    return invoice;
//                }

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                message = ex.Message;
//                return null;
//            }
//        }

//        public List<Invoice> GetInvoiceData()
//        {
//            try
//            {
//                using (var context = new BhagirathFinCareEntities())
//                {
//                    List<Invoice> invoiceuserlist = context.Invoice.OrderByDescending(x => x.LastUpdatedDate).ToList();
//                    return invoiceuserlist;

//                }
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return null;
//            }
//        }

//        public bool CalculatePaidInvoice(InvoiceViewModel user, out string message)
//        {
//            try
//            {
//                string subscription = "";
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Invoice invoiceobj = context.Invoice.Find(user.Id);
//                string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddhhmmss");
//                if (invoiceobj != null)
//                {
//                    //modification in invoice table

//                    invoiceobj.SubscriptionType = user.SubscriptionType;
//                    // invoiceobj.InvoiceNo = user.InvoiceNo;
//                    invoiceobj.AllowedEquity = user.AllowedEquity;
//                    invoiceobj.AllowedCommodity = user.AllowedCommodity;
//                    invoiceobj.AllowedCurrency = user.AllowedCurrency;
//                    invoiceobj.SubTotal = user.SubTotal;
//                    invoiceobj.Gstper = user.Gstper;
//                    invoiceobj.Discountper = user.Discountper;
//                    invoiceobj.GrandTotal = user.GrandTotal;
//                    invoiceobj.IsPaid = true;
//                    invoiceobj.LastUpdatedDate = DateTime.Now;
//                    context.Entry(invoiceobj).State = EntityState.Modified;

//                    //modification in user table
//                    var userobj = context.Users.Where(x => x.Id.Equals(user.UserId)).FirstOrDefault();
//                    if (userobj != null)
//                    {
//                        userobj.LastUpadateDate = DateTime.Now;
//                        userobj.LastPaymentDate = DateTime.Now;
//                        userobj.SubscriptionType = user.SubscriptionType;
//                        userobj.AllowedEquity = user.AllowedEquity;
//                        userobj.AllowedCommodity = user.AllowedCommodity;
//                        userobj.AllowedCurrency = user.AllowedCurrency;
//                        userobj.IsExpire = false;
//                        int PaymentType = -1;
//                        if (userobj.SubscriptionType == "Demo")
//                            PaymentType = -1;
//                        else if (userobj.SubscriptionType == "Monthly")
//                        {
//                            PaymentType = 0;
//                            user.InvoiceValidity = DateTime.Now.AddMonths(1);
//                        }

//                        else if (userobj.SubscriptionType == "Quarterly")
//                        {
//                            PaymentType = 1;
//                            user.InvoiceValidity = DateTime.Now.AddMonths(3);
//                        }

//                        else if (userobj.SubscriptionType == "Halfyearly")
//                        {
//                            PaymentType = 2;
//                            user.InvoiceValidity = DateTime.Now.AddMonths(6);
//                        }

//                        else if (userobj.SubscriptionType == "Yearly")
//                        {
//                            PaymentType = 3;
//                            user.InvoiceValidity = DateTime.Now.AddYears(1);
//                        }

//                        else if (userobj.SubscriptionType == "Lifetime")
//                            PaymentType = 4;
//                        userobj.PaymentType = PaymentType;

//                        if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == false)
//                        {
//                            subscription = "Equity";
//                        }
//                        else if (user.AllowedCommodity == true && user.AllowedEquity == false && user.AllowedCurrency == false)
//                        {
//                            subscription = "Commodity";
//                        }
//                        else if (user.AllowedCurrency == true && user.AllowedEquity == false && user.AllowedCommodity == false)
//                        {
//                            subscription = "Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == false)
//                        {
//                            subscription = "Equity Commodity";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == false && user.AllowedCurrency == true)
//                        {
//                            subscription = "Equity Currency";
//                        }
//                        else if (user.AllowedEquity == false && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            subscription = "Commodity Currency";
//                        }
//                        else if (user.AllowedEquity == true && user.AllowedCommodity == true && user.AllowedCurrency == true)
//                        {
//                            subscription = "Equity Commodity Currency";
//                        }

//                        context.Entry(userobj).State = EntityState.Modified;
//                    }
//                    context.SaveChanges();
//                    string body = string.Empty;
//                    string locationfolder = System.Web.HttpContext.Current.Server.MapPath("~/Invoices");
//                    if (!Directory.Exists(locationfolder))
//                    {
//                        System.IO.Directory.CreateDirectory(locationfolder);
//                    }
//                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index.html");
//                    string cssText = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/style.css"));
//                    using (StreamReader reader = new StreamReader(path))

//                    {

//                        body = reader.ReadToEnd();

//                    }
//                    body = body.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body = body.Replace("{invoiceno}", invoiceNo);
//                    body = body.Replace("{name}", user.FirstName);

//                    body = body.Replace("{transaction}", "paid");
//                    body = body.Replace("{string}", subscription);
//                    body = body.Replace("{subscription}", user.SubscriptionType);
//                    body = body.Replace("{subtotal}", user.SubTotal.ToString());
//                    body = body.Replace("{gst}", user.Gstper.ToString());
//                    body = body.Replace("{discount}", user.Discountper.ToString());
//                    body = body.Replace("{total}", user.GrandTotal.ToString());


//                    StringBuilder sa = new StringBuilder(body);
//                    var myString = sa.ToString();
//                    var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);
//                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc.SetMargins(10F, 10F, 10F, 10F);
//                    string path0 = Path.Combine(locationfolder, invoiceNo + ".pdf");
//                    var output = new FileStream(path0, FileMode.Create);
//                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, output);
//                    pdfDoc.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc.Close();
//                    string subject = "paid Invoice Mail";
//                    Email.SendPaidInvoiceEmail(user.Email, subject, path0, user.FirstName, invoiceNo, user.GrandTotal);

//                    string body1 = string.Empty;
//                    string path1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Emailtemplete/index2.html");
//                    using (StreamReader reader = new StreamReader(path1))

//                    {

//                        body1 = reader.ReadToEnd();

//                    }
//                    body1 = body1.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{InvoiceValidity}", user.InvoiceValidity.ToString("dd/MM/yyyy"));
//                    body1 = body1.Replace("{invoiceno}", invoiceNo);
//                    body1 = body1.Replace("{firstname}", user.FirstName);
//                    body1 = body1.Replace("{lastname}", user.LastName);

//                    body1 = body1.Replace("{transaction}", "paid");
//                    body1 = body1.Replace("{string}", subscription);
//                    body1 = body1.Replace("{subscription}", user.SubscriptionType);
//                    body1 = body1.Replace("{subtotal}", user.SubTotal.ToString());
//                    body1 = body1.Replace("{gst}", user.Gstper.ToString());
//                    body1 = body1.Replace("{discount}", user.Discountper.ToString());
//                    body1 = body1.Replace("{total}", user.GrandTotal.ToString());


//                    StringBuilder sa1 = new StringBuilder(body1);
//                    var myString1 = sa1.ToString();
//                    var myByteArray1 = System.Text.Encoding.UTF8.GetBytes(myString1);
//                    iTextSharp.text.Document pdfDoc1 = new iTextSharp.text.Document(PageSize.A4, 10F, 10F, 100F, 0F);
//                    pdfDoc1.SetMargins(10F, 10F, 10F, 10F);
//                    //string path00 = Path.Combine(locationfolder, invoiceNo + ".pdf");
//                    string path00 = System.Web.HttpContext.Current.Server.MapPath("~/Invoices");
//                    var file = Directory.EnumerateFiles(path00).SingleOrDefault(f => f.Contains(invoiceNo));
//                    var output1 = new FileStream(file, FileMode.Create);
//                    PdfWriter writer1 = PdfWriter.GetInstance(pdfDoc1, output1);
//                    pdfDoc1.Open();
//                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
//                    {
//                        using (var htmlMemoryStream = new MemoryStream(myByteArray1))
//                        {
//                            XMLWorkerHelper.GetInstance().ParseXHtml(writer1, pdfDoc1, htmlMemoryStream, cssMemoryStream);
//                        }
//                    }

//                    pdfDoc1.Close();


//                    string subject1 = "paid Invoice Mail";
//                    Email.SendPaidInvoiceEmail("welcome@bhagirathfincare.in", subject1, file, user.FirstName + " " + user.LastName, invoiceNo, user.GrandTotal);

//                }
//                else
//                {
//                    message = "No User Found.";
//                    return false;
//                }
//                message = "Successfully Paid.";
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                message = ex.Message;
//                return false;
//            }
//        }

//        public bool DeleteInvoice(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Invoice invoiceobj = context.Invoice.Find(id);
//                context.Invoice.Remove(invoiceobj);
//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }
//        }

//        public bool generatesessionid(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users user = context.Users.Find(id);
//                var guid = Guid.NewGuid();
//                user.SessionId = guid.ToString();
//                context.Entry(user).State = EntityState.Modified;
//                context.SaveChanges();
//                return true;

//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }

//        }

//        public bool removesessionid(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users user = context.Users.Find(id);
//                user.SessionId = null;
//                context.Entry(user).State = EntityState.Modified;
//                context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }

//        }

//        public object GetUserInfo(int id)
//        {
//            try
//            {
//                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
//                Users user = context.Users.Find(id);
//                return user;
//            }
//            catch (Exception ex)
//            {
//                ex.LogError(this);
//                return false;
//            }

//        }

//    }
//}
