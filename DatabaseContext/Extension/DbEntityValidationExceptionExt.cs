using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace PureSmileUI.Extension
{
    public static class DbEntityValidationExceptionExt
    {
        public static string ToDescription(this DbEntityValidationException e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var validationErrors in e.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    sb.AppendFormat("Property: {0} Error: {1}",
                        validationError.PropertyName,
                        validationError.ErrorMessage);
                }
            }
            return sb.ToString();
        }
    }
}