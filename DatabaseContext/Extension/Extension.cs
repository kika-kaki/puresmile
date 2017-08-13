using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DatabaseContext.Models;

namespace DatabaseContext.Extension
{
    public static class Extension
    {
        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static string Serialize(this Payment payment)
        {
            return string.Format("Pyament. Id: {0}, BookingId: {1}, CreatedOn: {2}, IsRefund: {3}, RefundByUserId: {4}, StatusId: {5}, Total: {6}, TransactionCode: {7}",
                payment.Id,
                payment.BookingId,
                payment.CreatedOn,
                payment.IsRefund,
                payment.RefundByUserId,
                payment.StatusId,
                payment.Total,
                payment.TransactionCode
            );
        }

        public static string Description(this Enum @enum)
        {
            Type enumType = @enum.GetType();
            FieldInfo field = enumType.GetField(@enum.ToString());
            if (field == null)
                return @enum.ToString();

            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : @enum.ToString();
        }
    }
}
