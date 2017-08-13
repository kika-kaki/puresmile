using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DatabaseContext.Models;
using PureSmileUI.Models.Dto;

namespace PureSmileUI
{
    public static class AutoMapper
    {
        public static void ConfigureMapper()
        {
            Mapper.Initialize(cfg => { cfg.CreateMap<Payment, PaymentItemView>(); });
        }
    }
}